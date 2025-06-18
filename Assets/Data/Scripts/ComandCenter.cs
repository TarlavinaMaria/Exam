using System.Collections.Generic;
using UnityEngine;

public class ComandCenter : MonoBehaviour
{
    // Параметры
    [SerializeField] private Scaner _scaner; // Сканер для обнаружения ресурсов
    [SerializeField] private Transform[] _patrulPoint; // Точки патрулирования для дронов
    [SerializeField] private Drone _dronePrefab; // Префаб дрона
    [SerializeField] private Transform _spawnPositionDron; // Позиция спавна дронов
    [SerializeField] private Transform _droneConteiner; // Контейнер для дронов
    [SerializeField] private List<Resurs> _storage = new List<Resurs>(); // Список для хранения ресурсов на командном центре
    [SerializeField] private ResursCounter _resursCounter;

    // Переменные
    private Queue<Resurs> _resursers = new Queue<Resurs>(); // Очередь для хранения ресурсов, которые будут обрабатываться дронами
    private Queue<Drone> _drons = new Queue<Drone>(); // Очередь для хранения дронов, которые будут выполнять задачи
    private Drone _tempDrone; // Временный дрон для обработки задач

    // Флаги
    private bool _isHaveDrone = false; // Флаг, указывающий, есть ли уже дрон на базе

    private void Update() // Метод Update вызывается каждый кадр
    {
        // Проверяем нажатие клавиши Space для сканирования ресурсов
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_scaner.Scane(_resursers).Count);
            //_scaner.Scane(_resursers);
        }
        // Проверяем нажатие клавиши E для создания дрона, если его еще нет
        if (Input.GetKeyDown(KeyCode.E) && _isHaveDrone == false)
        {
            CreateDrons(); // Создаем дрон, если его еще нет
            _isHaveDrone = true; // Устанавливаем флаг, что дрон уже создан

            _tempDrone.TakeScanner(_scaner); // Передаем сканер дрону для обнаружения ресурсов
            _tempDrone.TakeResurserQueue(_resursers); // Передаем очередь ресурсов дрону
        }
        // Проверяем, есть ли дроны и ресурсы для отправки
        if (_drons.Count > 0)
        {
            if (_resursers.Count > 0)
            {
                SentDron(); // Отправляем дрон к ресурсу
            }
        }
    }
    private void SentDron() // Метод для отправки дрона к ресурсу
    {
        _tempDrone = _drons.Dequeue(); // Извлекаем дрон из очереди
        _tempDrone.TakeTarget(_resursers.Dequeue().transform); // Устанавливаем цель для дрона
    }
    private void CreateDrons() // Метод для создания дронов
    {
        _tempDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner); // Создаем дрон из префаба
        _tempDrone.TakePositionComandCenter(this.transform); // Устанавливаем позицию командного центра для дрона
        _tempDrone.TakePatrulPoint(_patrulPoint); // Устанавливаем точки патрулирования для дрона
        _tempDrone.TakeCommandCenter(this); // Передаем ссылку на командный центр дрону
        _drons.Enqueue(_tempDrone); // Добавляем дрон в очередь дронов
    }
    public void StoreResurs(Resurs resurs) // Метод для хранения ресурса в командном центре
    {
        _storage.Add(resurs); // Добавляем ресурс в список хранения
        Debug.Log($"Поступили ресурсы! На базе {_storage.Count} ресурсов");
        _resursCounter.AddResurs();

        //// Если количество ресурсов кратно 3, создаем нового дрона
        //if (_storage.Count % 3 == 0)
        //{
        //    // Создаем нового дрона по аналогии с использованием префаба
        //    Drone newDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner);
        //    newDrone.TakePositionComandCenter(this.transform);
        //    newDrone.TakePatrulPoint(_patrulPoint);
        //    newDrone.TakeCommandCenter(this);
        //    newDrone.TakeScanner(_scaner);
        //    newDrone.TakeResurserQueue(_resursers);
        //    _drons.Enqueue(newDrone);

        //    // Удаляем 3 ресурса, использованные на создание
        //    _storage.RemoveRange(_storage.Count - 3, 3);

        //    Debug.Log("Создан новый дрон!");
        //}
    }
}
