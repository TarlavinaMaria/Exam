using UnityEngine;



public class BuildingNewBase : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ComandCenter _comandCenter;
    [SerializeField] private Transform _containerBase;
    [SerializeField] private float _scaneRadius;

    [SerializeField] private Color _colorCantBuld;
    [SerializeField] private Color _colorCanBuld;


    private RaycastHit _raycastHit;
    private Ray _ray;
    private ComandCenter _tempComandCenter = null;
    private MeshRenderer _meshRenderer;

    private bool _isHaveBuildBase = false;
    private bool _isChangeColor = false;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(_ray, out _raycastHit);

        Debug.DrawRay(_ray.origin, _ray.direction * 1000);
        SelectionBase();
        BuildBase();
    }
    private void SelectionBase()
    {
        if (Input.GetMouseButtonDown(0) && !_isHaveBuildBase)
        {
            if (Physics.Raycast(_ray, out _raycastHit))
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

    }

    public void BuildBase()
    {
        Debug.Log("1");
        if (_isHaveBuildBase && _tempComandCenter == null)
        {
            _tempComandCenter = Instantiate(_comandCenter);
            Debug.Log("База создалась");
        }
        else if (_tempComandCenter != null)
        {
            _tempComandCenter.transform.position = new Vector3(_raycastHit.point.x, 1, _raycastHit.point.z);
        }

        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, _scaneRadius); // Получаем все коллайдеры в радиусе сканирования

        foreach (Collider collider in triggerColliders) // Проходим по каждому коллайдеру
        {
            Debug.Log("2");
            if (collider.gameObject.TryGetComponent<ComandCenter>(out ComandCenter center)) // Проверяем, является ли объект ресурсом
            {
                _isChangeColor = true;
                Debug.Log("База!");
            }
        }
        //if (_isChangeColor)
        //{
        //    _tempComandCenter.ChangeColor(_colorCantBuld);
        //}
        //else
        //{
        //    _tempComandCenter.ChangeColor(_colorCanBuld);
        //}
    }

}
