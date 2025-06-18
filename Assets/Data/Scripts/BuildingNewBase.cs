using UnityEngine;

public class BuildingNewBase : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ComandCenter _prefabComandCenter;
    [SerializeField] private Transform _containerBase;
    [SerializeField] private float _scaneRadius;

    [SerializeField] private Color _colorCantBuld;
    [SerializeField] private Color _colorCanBuld;


    private RaycastHit _raycastHit;
    private Ray _ray;
    private ComandCenter _tempComandCenter = null;
    private MeshRenderer _meshRenderer;

    private bool _isHaveBuildBase = false;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(_ray, out _raycastHit);

        Debug.DrawRay(_ray.origin, _ray.direction * 100);
        SelectionBase();
        BuildBase();
    }
    private void SelectionBase()
    {
        if (Input.GetMouseButtonDown(0) && !_isHaveBuildBase)
        {
            if (_raycastHit.transform.TryGetComponent<ComandCenter>(out ComandCenter center))
            {
                _isHaveBuildBase = true;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (_tempComandCenter != null)
            {
                Destroy(_tempComandCenter.gameObject);
                _isHaveBuildBase = false;
            }
        }
        else if (Input.GetMouseButtonDown(0) && _isHaveBuildBase && !IsCollated())
        {
            _tempComandCenter = null;
            _isHaveBuildBase = false;
        }

    }

    public void BuildBase()
    {
        if (_isHaveBuildBase && _tempComandCenter == null)
        {
            _tempComandCenter = Instantiate(_prefabComandCenter);
            _tempComandCenter.SetLayer();
        }
        else if (_tempComandCenter != null)
        {
            _tempComandCenter.transform.position = new Vector3(_raycastHit.point.x, 1, _raycastHit.point.z);
        }

        if (IsCollated() && _tempComandCenter != null)
        {
            _tempComandCenter.ChangeColor(_colorCantBuld);
        }
        else if (_tempComandCenter != null)
        {
            _tempComandCenter.ChangeColor(_colorCanBuld);
        }
    }
    private bool IsCollated()
    {
        Collider[] triggerColliders = Physics.OverlapSphere(_raycastHit.point, _scaneRadius); // Получаем все коллайдеры в радиусе сканирования

        foreach (Collider collider in triggerColliders) // Проходим по каждому коллайдеру
        {
            if (collider.gameObject.TryGetComponent<ComandCenter>(out ComandCenter center)) // Проверяем, является ли объект ресурсом
            {
                if (_tempComandCenter != center)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
