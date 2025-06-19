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
    private List<Resurs> _cargo;
    private ComandCenter _targetBase;

    private void Update()
    {
        if (_isReady)
        {
            // 1. Несёт ресурсы на новую базу
            if (_haveResurs && _targetBase != null)
            {
                MoveToTarget(_targetBase.transform);
                Debug.Log("Дрон снабжения движется к новой базе!");

                if (Vector3.Distance(transform.position, _targetBase.transform.position) < 0.5f)
                {
                    foreach (var res in _cargo)
                        _targetBase.StoreResurs(res);

                    _cargo = null;
                    _haveResurs = false;

                    TakeCommandCenter(_targetBase);
                    TakeScanner(_scaner); // ← если переопределялся
                    TakeResurserQueue(_resursers);
                    TakePositionComandCenter(_targetBase.transform);

                    _targetBase = null;
                }
            }
            // 2. Есть активная цель (ресурс)
            else if (_isHaveTarget)
            {
                if (_target == null || !_target.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning($"{name}: цель недействительна");
                    _isHaveTarget = false;
                    _target = null;
                    return;
                }

                MoveToTarget(_target);

                if (Vector3.Distance(transform.position, _target.position) < 0.5f)
                {
                    _carriedResurs = _target.GetComponent<Resurs>();

                    if (_carriedResurs != null)
                    {
                        _carriedResurs.gameObject.SetActive(false);
                        _haveResurs = true;
                        _isHaveTarget = false;
                    }
                    else
                    {
                        Debug.LogWarning($"{name}: цель {_target.name} не содержит Resurs");
                        _isHaveTarget = false;
                        _target = null;
                    }
                }
            }
            // 3. Несёт ресурс на свою базу
            else if (_haveResurs)
            {
                if (_comandCenterPoint == null)
                {
                    Debug.LogError($"{name}: _comandCenterPoint отсутствует!");
                    return;
                }

                MoveToTarget(_comandCenterPoint);

                if (Vector3.Distance(transform.position, _comandCenterPoint.position) < 0.5f && !_isWaiting)
                {
                    _comandCenter.StoreResurs(_carriedResurs);
                    _carriedResurs = null;
                    _haveResurs = false;

                    StartCoroutine(AfterDelivery());
                }
            }
            // 4. Пытаемся найти ресурсы
            else if (!_isHaveTarget && !_haveResurs && !_isWaiting)
            {
                _scaner.Scane(_resursers);

                while (_resursers.Count > 0)
                {
                    Resurs next = _resursers.Dequeue();
                    if (next != null && next.gameObject.activeInHierarchy)
                    {
                        TakeTarget(next.transform);
                        break;
                    }
                }

                // Переход к патрулированию, если ничего не нашли
                if (!_isHaveTarget)
                {
                    FreeMove();
                }
            }
            // 5. Ожидаем — или патрулируем
            else
            {
                FreeMove();
            }
        }
        // Если дрон ещё не готов — просто патрулируем
        else
        {
            FreeMove();
        }
    }

    private IEnumerator AfterDelivery() // Метод для ожидания после доставки ресурса
    {
        _isWaiting = true; // Устанавливаем флаг ожидания
        yield return new WaitForSeconds(2f); // Ждем 2 секунды

        // Проверяем, есть ли ресурсы после сканирования и извлекаем следующий ресурс
        _nextResurs = _scaner.Scane(_resursers).Count > 0 ? _resursers.Dequeue() : null;

        if (_nextResurs != null)
        {
            TakeTarget(_nextResurs.transform);
        }
        else
        {
            Debug.LogWarning($"{name}: извлечённый ресурс оказался null");
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
            if (_pointPatrul != null && _pointPatrul.Length > 0)
            {
                _currentPoint = ++_currentPoint % _pointPatrul.Length;
            }
            else
            {
                Debug.LogWarning($"Дрон {name} не получил точки патрулирования!");
            }
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
    public void TakeTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning($"{name}: Попытка назначить null как цель");
            return;
        }

        if (target.GetComponent<Resurs>() == null)
        {
            Debug.LogWarning($"{name}: Попытка назначить цель {target.name}, но она не Resurs");
            return;
        }

        _target = target;
        _isHaveTarget = true;
        Debug.Log($"{name}: Цель установлена — {target.name}");
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
    public void DeliverResursesToBase(
    List<Resurs> cargo,
    ComandCenter target,
    Transform[] patrol,
    Scaner scaner,
    Queue<Resurs> resQueue)
    {
        _cargo = cargo;
        _targetBase = target;

        TakePatrulPoint(patrol);
        TakeCommandCenter(target);
        TakeScanner(scaner);              // 💡 Важно
        TakeResurserQueue(resQueue);      // 💡 Важно
        TakePositionComandCenter(target.transform);

        TakeTarget(target.transform);
        _isHaveTarget = true;
        _haveResurs = true;

        Debug.Log($"[Снабжение] Дрон готов: цель — {_targetBase.name}, ресурсов: {_cargo.Count}");
    }




}
