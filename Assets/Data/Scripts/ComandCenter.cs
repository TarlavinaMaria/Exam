using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

public class ComandCenter : MonoBehaviour
{
    // Параметры
    [SerializeField] private Scaner _scaner; // Сканер для обнаружения ресурсов
    [SerializeField] private Transform[] _patrulPoint; // Точки патрулирования для дронов
    [SerializeField] private Drone _dronePrefab; // Префаб дрона
    [SerializeField] private Transform _spawnPositionDron; // Позиция спавна дронов
    [SerializeField] private Transform _droneConteiner; // Контейнер для дронов
    [SerializeField] private List<Resurs> _storage = new List<Resurs>(); // Список для хранения ресурсов на командном центре
    [SerializeField] private ResursCounter _resursCounter; // Счетчик ресурсов на базе
    [SerializeField] private bool _isTemplate = false; // Флаг, указывающий, является ли этот командный центр шаблоном (для создания новых баз)

    // Переменные
    private Queue<Resurs> _resursers = new Queue<Resurs>(); // Очередь для хранения ресурсов, которые будут обрабатываться дронами
    private Queue<Drone> _drons = new Queue<Drone>(); // Очередь для хранения дронов, которые будут выполнять задачи
    private Drone _tempDrone; // Временный дрон для обработки задач
    private MeshRenderer _meshRenderer; // Компонент MeshRenderer для изменения цвета командного центра
    public bool IsReadyToExpand => _storage.Count >= 5; // Проверка, готов ли командный центр к расширению
    public bool IsActive => _isBilding; // Проверка, активен ли командный центр
    private ComandCenter _nextBaseTarget;  // Целевая база для поставки ресурсов
    private int _resursToSend = 5; // Количество ресурсов для отправки на следующую базу
    private int _totalDrones = 0; // Общее количество дронов на базе

    // Флаги
    private bool _isBilding = false; // Флаг, указывающий, построен ли командный центр
    private bool _isSupplyingNextBase = false; // Флаг, указывающий, идет ли поставка ресурсов на следующую базу

    private void Start() // Метод Start вызывается при инициализации объекта
    {
        if (!_isTemplate)
        {
            Builded(); // Если это не шаблон, то сразу вызываем метод Builded для инициализации командного центра
        }
    }
    private void Awake() // Метод Awake вызывается при создании объекта
    {
        _meshRenderer = GetComponent<MeshRenderer>(); // Получаем компонент MeshRenderer для изменения цвета командного центра
    }
    private void Update() // Метод Update вызывается каждый кадр
    {
        //Если база построена
        if (_isBilding)
        {
            // Проверяем нажатие клавиши Space для сканирования ресурсов
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(_scaner.Scane(_resursers).Count);
                //_scaner.Scane(_resursers);
            }
            // Проверяем нажатие клавиши E для создания дрона, если его еще нет
            if (Input.GetKeyDown(KeyCode.E) && _totalDrones == 0)
            {
                CreateNewDrone(ignoreResources: true); // Первый дрон без списания ресурсов
            }

            // Проверяем, есть ли дроны и ресурсы для отправки
            if (_drons.Count > 0) 
            {
                if (_resursers.Count > 0) // Если есть ресурсы
                {
                    SentDron(); // Отправляем дрон к ресурсу
                }
            }
            if (_isSupplyingNextBase && _storage.Count >= _resursToSend) // Если поставка ресурсов на следующую базу
            {
                _isSupplyingNextBase = false; // Сбрасываем флаг

                // Забираем 5 ресурсов
                List<Resurs> supply = _storage.GetRange(_storage.Count - _resursToSend, _resursToSend);
                _storage.RemoveRange(_storage.Count - _resursToSend, _resursToSend); // удаляем ресурсы
                _resursCounter.RemoveResurs(_resursToSend); // удаляем ресурсы

                // Создаём одного дрона-поставщика и инициализируем его
                Drone supplierDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner);
                supplierDrone.TakeCommandCenter(_nextBaseTarget);
                supplierDrone.DeliverResursesToBase(
                    supply,
                    _nextBaseTarget,
                    _patrulPoint,
                    _nextBaseTarget.GetComponent<Scaner>(),
                    _nextBaseTarget.GetComponent<ComandCenter>().GetResQueue());

                Debug.Log("Отправлены ресурсы на новую базу!");
            }

        }
    }
    // Для одиночного ресурса
    public void StoreResurs(Resurs resurs) // Метод для хранения ресурса
    {
        if (resurs != null) // Если ресурс не null
        {
            resurs.gameObject.SetActive(false); // Скрываем ресурс
        }

        _storage.Add(resurs); // Добавляем ресурс
        _resursCounter.AddResurs(); // Увеличиваем счетчик ресурсов

        if (!_isSupplyingNextBase && _storage.Count >= 3) // Если поставка ресурсов на следующую базу не идет и количество ресурсов достигло 3
        {
            CreateNewDrone(); // Создаем новый дрон
        }
    }
    public void ChangeColor(Color color) // Метод для изменения цвета
    {
        Material material = _meshRenderer.material; // Получаем текущий материал
        material.color = color; // Изменяем цвет
        _meshRenderer.material = material; // Применяем изменения
    }
    public void SetLayer() // Метод для установки слоя
    {
        transform.gameObject.layer = 0; // Устанавливаем слой
    }
    public void Builded() // Метод для установки флага построенности
    {
        _isBilding = true; // Устанавливаем флаг
        Debug.Log($"Командный центр построен: {gameObject.name}, шаблон: {_isTemplate}");
    }
    public void SendSupplyTo(ComandCenter targetBase) // Метод для отправки ресурсов на следующую базу
    {
        _nextBaseTarget = targetBase; // Устанавливаем следующую базу
        _isSupplyingNextBase = true; // Устанавливаем флаг
    }
    public bool HasOnlyOneDrone() // Метод для проверки, есть ли только один дрон
    {
        return _totalDrones <= 1; // Возвращаем true, если только один дрон
    }
    public Queue<Resurs> GetResQueue() // Метод для получения очереди ресурсов
    {
        return _resursers;
    }
    public Scaner Scaner // Свойство для получения сканера
    {
        get
        {
            return _scaner;
        }
    }
    public void ReceiveSupply(int resourceCount) // Метод для получения ресурсов
    {
        // Добавляем только количество ресурсов
        for (int i = 0; i < resourceCount; i++)
        {
            _storage.Add(null); 
            _resursCounter.AddResurs();
        }

        Debug.Log($"Получено {resourceCount} ресурсов. Всего: {_storage.Count}");

        CheckDroneCreation();
    }
    private void CheckDroneCreation() // Метод для проверки создания дрона
    {
        // Проверяем без учёта флага поставки
        if (_storage.Count >= 3)
        {
            CreateNewDrone();
            Debug.Log($"Создан дрон на базе: {gameObject.name}. Всего дронов: {_totalDrones}");
        }
    }
    private void SentDron() // Метод для отправки дрона к ресурсу
    {
        _tempDrone = _drons.Dequeue(); // Извлекаем дрон из очереди
        _tempDrone.TakeTarget(_resursers.Dequeue().transform); // Устанавливаем цель для дрона
    }
    private void CreateNewDrone(bool ignoreResources = false) // Метод для создания нового дрона
    {
        if (!ignoreResources && _storage.Count < 3) return; // Если ресурсов не достаточно, возвращаем
        // Создаем новый дрон
        Drone newDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner);
        newDrone.TakePositionComandCenter(transform);
        newDrone.TakePatrulPoint(_patrulPoint);
        newDrone.TakeCommandCenter(this);
        newDrone.TakeScanner(_scaner);
        newDrone.TakeResurserQueue(_resursers);
        _drons.Enqueue(newDrone);
        _totalDrones++;

        if (!ignoreResources) // Если ресурсы не игнорируются
        {
            _storage.RemoveRange(_storage.Count - 3, 3); // Удаляем ресурсы
            _resursCounter.RemoveResurs(3); // Уменьшаем количество ресурсов
        }

        Debug.Log($"Создан дрон на базе: {gameObject.name}, позиция: {_spawnPositionDron.position}, текущее количество дронов: {_totalDrones}");
    }
}
