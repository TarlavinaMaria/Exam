using UnityEngine;

public class BuldingNewBase : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private RaycastHit _raycastHit;
    private Ray _ray;

    private void Update()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(_ray, out _raycastHit);

        Debug.DrawRay(_ray.origin, _ray.direction * 1000);
    }
}
