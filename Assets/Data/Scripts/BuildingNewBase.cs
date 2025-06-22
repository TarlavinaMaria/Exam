using UnityEngine;

public class BuildingNewBase : MonoBehaviour
{
    // Параметры
    [SerializeField] private Camera _camera; // Камера для получения луча из мыши
    [SerializeField] private ComandCenter _prefabComandCenter; // Префаб командного центра
    [SerializeField] private Transform _containerBase; // Контейнер для баз
    [SerializeField] private float _scaneRadius; // Радиус сканирования для проверки коллизий
    [SerializeField] private Color _colorCantBuld; // Цвет, если нельзя строить
    [SerializeField] private Color _colorCanBuld; // Цвет, если можно строить

    // Переменные
    private RaycastHit _raycastHit; // Переменная для хранения информации о попадании луча
    private Ray _ray; // Луч, который будет использоваться для определения позиции клика мыши
    // Флаги 
    private ComandCenter _tempComandCenter = null; // Временный командный центр, который будет отображаться при строительстве
    private bool _isHaveBuildBase = false; // Флаг, указывающий, что пользователь хочет построить базу

    private void Update() // Метод Update вызывается каждый кадр
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition); // Получаем луч из камеры по позиции мыши
        Physics.Raycast(_ray, out _raycastHit); // Выполняем лучевое сканирование и сохраняем информацию о попадании

        Debug.DrawRay(_ray.origin, _ray.direction * 1000); // Отрисовываем луч в редакторе для отладки
        SelectionBase(); // Вызываем метод для обработки выбора базы
        BuildBase(); // Вызываем метод для построения базы
    }
    public void BuildBase() // Метод для построения базы
    {
        if (_isHaveBuildBase && _tempComandCenter == null) // Если пользователь хочет построить базу и временный командный центр не создан
        {
            _tempComandCenter = Instantiate(_prefabComandCenter); // Создаем временный командный центр
            _tempComandCenter.SetLayer(); // Устанавливаем слой для временного командного центра
        }
        else if (_tempComandCenter != null) // Если временный командный центр уже есть
        {
            _tempComandCenter.transform.position = new Vector3(_raycastHit.point.x, 1, _raycastHit.point.z); // Устанавливаем позицию временного командного центра на позицию попадания луча
        }

        if (IsCollated() && _tempComandCenter != null) // Если есть коллизии с другими базами
        {
            _tempComandCenter.ChangeColor(_colorCantBuld); // Меняем цвет временного командного центра на цвет, если нельзя строить
        }
        else if (_tempComandCenter != null) // Если нет коллизий
        {
            _tempComandCenter.ChangeColor(_colorCanBuld); // Меняем цвет временного командного центра на цвет, если можно строить
        }
    }
    private bool IsCollated() // Метод для проверки коллизий с другими базами
    {
        Collider[] triggerColliders = Physics.OverlapSphere(_raycastHit.point, _scaneRadius); // Получаем все коллайдеры в радиусе сканирования

        foreach (Collider collider in triggerColliders) // Проходим по каждому коллайдеру
        {
            if (collider.gameObject.TryGetComponent<ComandCenter>(out ComandCenter center)) // Проверяем, является ли объект ресурсом
            {
                if (_tempComandCenter != center) // Если временный командный центр не совпадает с текущим центром
                {
                    return true; // Возвращаем true, если есть коллизия с другим центром
                }
            }
        }
        return false; // Возвращаем false, если коллизий нет
    }
    private void SelectionBase() // Метод для строитества базы
    {
        // Если нет временного командного центра и ЛКМ нажата по базе
        if (!_isHaveBuildBase && Input.GetMouseButtonDown(0))
        {
            // Проверяем, попал ли луч в объект
            if (_raycastHit.transform.TryGetComponent<ComandCenter>(out ComandCenter center))
            {
                _isHaveBuildBase = true;
            }
        }

        // ПКМ — снять выделение базы
        else if (Input.GetMouseButtonDown(1))
        {
            if (_tempComandCenter != null)  // Если временный центр не нулевой 
            {
                Destroy(_tempComandCenter.gameObject); // Удаляем временный командный центр
                _isHaveBuildBase = false; // Сбрасываем флаг строительства базы
            }
        }

        // Второй ЛКМ — разместить шаблон, если нет коллизий
        else if (_isHaveBuildBase && Input.GetMouseButtonDown(0) && !IsCollated())
        {
            if (_tempComandCenter != null) // Если временный центр не нулевой 
            {
                ComandCenter[] allBases = Object.FindObjectsByType<ComandCenter>(FindObjectsSortMode.None); // Получаем все базы в сцене
                foreach (var baseCandidate in allBases) // Проходим по всем базам
                {
                    if (baseCandidate != _tempComandCenter && baseCandidate.IsActive) // Если база не является временным центром и активна
                    {
                        if (baseCandidate.HasOnlyOneDrone()) // Если у базы всего один дрон
                        {
                            Debug.Log("Нельзя строить базу — на старой базе всего один дрон!");
                            return;
                        }

                        // Отправляем ресурсы на новую базу
                        baseCandidate.SendSupplyTo(_tempComandCenter);
                        break;
                    }
                }
                _tempComandCenter.Builded(); // Вызываем метод построения временного командного центра
                _tempComandCenter = null; // Сбрасываем временный командный центр
                _isHaveBuildBase = false; // Сбрасываем флаг строительства базы
            }
        }
    }
}
