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
    [SerializeField] private ResursCounter _resursCounter; // 
    [SerializeField] private bool _isTemplate = false;

    // Переменные
    private Queue<Resurs> _resursers = new Queue<Resurs>(); // Очередь для хранения ресурсов, которые будут обрабатываться дронами
    private Queue<Drone> _drons = new Queue<Drone>(); // Очередь для хранения дронов, которые будут выполнять задачи
    private Drone _tempDrone; // Временный дрон для обработки задач
    private MeshRenderer _meshRenderer;
    public bool IsReadyToExpand => _storage.Count >= 5;
    public bool IsActive => _isBilding;
    private ComandCenter _nextBaseTarget;
    private bool _isSupplyingNextBase = false;
    private int _resursToSend = 5;
    private int _totalDrones = 0;

    // Флаги
    private bool _isBilding = false;

    private void Start()
    {
        if (!_isTemplate)
        {
            Builded();
        }
    }
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

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
                if (_resursers.Count > 0)
                {
                    SentDron(); // Отправляем дрон к ресурсу
                }
            }
            if (_isSupplyingNextBase && _storage.Count >= _resursToSend)
            {
                _isSupplyingNextBase = false;

                // Забираем 5 ресурсов
                List<Resurs> supply = _storage.GetRange(_storage.Count - _resursToSend, _resursToSend);
                _storage.RemoveRange(_storage.Count - _resursToSend, _resursToSend);
                _resursCounter.RemoveResurs(_resursToSend); // если есть

                // Создаём одного дрона-поставщика
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
    public void StoreResurs(Resurs resurs)
    {
        if (resurs != null)
        {
            resurs.gameObject.SetActive(false);
        }

        _storage.Add(resurs);
        _resursCounter.AddResurs();

        if (!_isSupplyingNextBase && _storage.Count >= 3)
        {
            CreateNewDrone();
        }
    }
    public void ChangeColor(Color color)
    {
        Material material = _meshRenderer.material;
        material.color = color;
        _meshRenderer.material = material;
    }
    public void SetLayer()
    {
        transform.gameObject.layer = 0;
    }
    public void Builded()
    {
        _isBilding = true;
        Debug.Log($"Командный центр построен: {gameObject.name}, шаблон: {_isTemplate}");
    }
    public void SendSupplyTo(ComandCenter targetBase)
    {
        _nextBaseTarget = targetBase;
        _isSupplyingNextBase = true;
    }
    public bool HasOnlyOneDrone()
    {
        return _totalDrones <= 1;
    }
    public Queue<Resurs> GetResQueue()
    {
        return _resursers;
    }
    public Scaner Scaner
    {
        get
        {
            return _scaner;
        }
    }
    public void ReceiveSupply(int resourceCount)
    {
        // Добавляем только количество ресурсов
        for (int i = 0; i < resourceCount; i++)
        {
            _storage.Add(null); // Или можно создать пустой Resurs
            _resursCounter.AddResurs();
        }

        Debug.Log($"Получено {resourceCount} ресурсов. Всего: {_storage.Count}");

        CheckDroneCreation();
    }
    private void CheckDroneCreation()
    {
        // Проверяем без учёта флага поставки
        if (_storage.Count >= 3)
        {
            CreateNewDrone();
            Debug.Log($"Создан дрон на базе: {gameObject.name}. Всего дронов: {_totalDrones}");
        }
    }
    private void CreateNewDrone(bool ignoreResources = false)
    {
        if (!ignoreResources && _storage.Count < 3) return;

        Drone newDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner);
        newDrone.TakePositionComandCenter(transform);
        newDrone.TakePatrulPoint(_patrulPoint);
        newDrone.TakeCommandCenter(this);
        newDrone.TakeScanner(_scaner);
        newDrone.TakeResurserQueue(_resursers);
        _drons.Enqueue(newDrone);
        _totalDrones++;

        if (!ignoreResources)
        {
            _storage.RemoveRange(_storage.Count - 3, 3);
            _resursCounter.RemoveResurs(3);
        }

        Debug.Log($"Создан дрон на базе: {gameObject.name}, позиция: {_spawnPositionDron.position}, текущее количество дронов: {_totalDrones}");
    }
    private void SentDron() // Метод для отправки дрона к ресурсу
    {
        _tempDrone = _drons.Dequeue(); // Извлекаем дрон из очереди
        _tempDrone.TakeTarget(_resursers.Dequeue().transform); // Устанавливаем цель для дрона
    }
}
