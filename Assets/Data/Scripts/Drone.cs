using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    // Параметры
    [SerializeField] float _speed; // Скорость движения дрона

    // Переменные
    private Transform[] _pointPatrul; // Переменная для хранения точек патрулирования
    private int _currentPoint = 0; // Текущая точка патрулирования
    private Transform _comandCenterPoint; // Переменная для хранения точки командного центра
    private Transform _target; // Переменная для хранения цели, к которой движется дрон
    private Resurs _carriedResurs; // Переменная для хранения ресурса
    private Scaner _scaner; // Переменная для хранения сканера
    private Queue<Resurs> _resursers; // Очередь для хранения ресурсов, которые дрон может забрать
    private ComandCenter _comandCenter; // Переменная для хранения командного центра, к которому принадлежит дрон
    private Resurs _nextResurs; // Текущий выбранный ресурс

    // Флаги
    private bool _isReady = false; // Флаг, указывающий, готов ли дрон к работе (имеет ли точки патрулирования и командный центр)
    private bool _isHaveTarget = false; // Флаг, указывающий, есть ли у дрона цель для движения
    private bool _haveResurs = false; // Флаг, указывающий, несет ли дрон ресурс
    private bool _isWaiting = false; // Флаг, указывающий, ждет ли дрон выполнения каких-либо действий


    private void Update() // Метод Update вызывается каждый кадр
    {
        // Если дрон готов
        if (_isReady == true)
        {
            // Если есть цель
            if (_isHaveTarget == true)
            {
                // Если есть цель, двигается к цели
                MoveToTarget(_target);

                // Только если дрон близко к ресурсу
                if (Vector3.Distance(transform.position, _target.position) < 0.5f)
                {
                    _carriedResurs = _target.GetComponent<Resurs>(); // Получаем компонент ресурса из цели
                    _carriedResurs.gameObject.SetActive(false); // Делаем ресурс невидимым
                    _haveResurs = true; // Устанавливаем флаг, что дрон несет ресурс
                    _isHaveTarget = false; // Сбрасываем флаг наличия цели
                }
            }
            else
            {
                // Если цели нет, проверяет, несет ли дрон ресурс
                if (_haveResurs == true)
                {
                    MoveToTarget(_comandCenterPoint); // Двигается к командному центру

                    // Если дрон близко к командному центру
                    if (Vector3.Distance(transform.position, _comandCenterPoint.position) < 0.5f && !_isWaiting)
                    {
                        // Если несет ресурс, двигается к командному центру
                        _comandCenter.StoreResurs(_carriedResurs); // Отправляем ресурс в командный центр
                        _carriedResurs = null; // Сбрасываем переменную ресурса
                        _haveResurs = false; // Сбрасываем флаг, что дрон несет ресурс

                        StartCoroutine(AfterDelivery()); // Подождать 2 секунды на базе
                    }
                }
                else
                {
                    _scaner.Scane(_resursers); // Сканирует ресурсы в радиусе действия

                    if (_resursers.Count > 0)
                    {
                        _nextResurs = _resursers.Dequeue(); // Извлекает следующий ресурс из очереди
                        TakeTarget(_nextResurs.transform); // Устанавливает цель для дрона
                    }
                    else
                    {
                        // Если нет ресурсов, дрон патрулирует
                        FreeMove();
                    }

                }
            }
        }
    }
    private IEnumerator AfterDelivery() // Метод для ожидания после доставки ресурса
    {
        _isWaiting = true; // Устанавливаем флаг ожидания
        yield return new WaitForSeconds(2f); // Ждем 2 секунды

        // Проверяем, есть ли ресурсы после сканирования и извлекаем следующий ресурс
        _nextResurs = _scaner.Scane(_resursers).Count > 0 ? _resursers.Dequeue() : null;

        if (_nextResurs != null) // Если есть следующий ресурс
        {
            TakeTarget(_nextResurs.transform); // Устанавливаем его как цель
        }

        _isWaiting = false; // Сбрасываем флаг ожидания
    }


    private void FreeMove() // Метод для патрулирования дрона
    {
        // Если дрон не имеет цели, двигается к следующей точке патрулирования
        transform.position = Vector3.MoveTowards(transform.position, _pointPatrul[_currentPoint].position, _speed * Time.deltaTime);
        transform.LookAt(_pointPatrul[_currentPoint]); // Поворачивается к точке патрулирования
    }
    private void OnTriggerEnter(Collider other) //Метод тригера 
    {
        // Если дрон входит в триггер другой точки патрулирования, переключает на следующую точку
        if (other.gameObject.TryGetComponent<PointPatrul>(out PointPatrul pointPatrul))
        {
            /*
             ++_currentPoint — сначала увеличиваем индекс на 1
                % _pointPatrul.Length — потом берём остаток от деления, 
                чтобы вернуться к началу, когда достигнут конец массива
             */
            _currentPoint = ++_currentPoint % _pointPatrul.Length;
        }
    }

    private void MoveToTarget(Transform target) // Метод для движения дрона к цели
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime); // Двигается к цели с заданной скоростью
        transform.LookAt(target.position); // Поворачивается к цели
    }
    public void TakePositionComandCenter(Transform comandCenter) // Метод для установки позиции командного центра
    {
        _comandCenterPoint = comandCenter; // Устанавливает позицию командного центра для дрона
    }
    public void TakePatrulPoint(Transform[] pointPatrul) // Метод для установки точек патрулирования
    {
        _pointPatrul = pointPatrul; // Устанавливает точки патрулирования для дрона
        _isReady = true; // Устанавливает флаг готовности дрона
    }
    public void TakeTarget(Transform tagret) // Метод для установки цели для дрона
    {
        _target = tagret; // Устанавливает цель для дрона
        _isHaveTarget = true; // Устанавливает флаг наличия цели
    }
    public void TakeScanner(Scaner scaner) // Метод для установки сканера для дрона
    {
        _scaner = scaner; // Устанавливает сканер для обнаружения ресурсов
    }
    public void TakeResurserQueue(Queue<Resurs> resursers) // Метод для установки очереди ресурсов для дрона
    {
        _resursers = resursers; // Устанавливает очередь ресурсов, которые дрон может забрать
    }
    public void TakeCommandCenter(ComandCenter commandCenter) // Метод для установки командного центра для дрона
    {
        _comandCenter = commandCenter; // Устанавливает ссылку на командный центр, к которому принадлежит дрон
    }

}
