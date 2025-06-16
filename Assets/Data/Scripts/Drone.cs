using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] float _speed;

    [SerializeField] private Transform[] _pointPatrul; // массив точек
    private int _currentPoint = 0;

    private void Update()
    {
        FreeMove();
    }
    private void FreeMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, _pointPatrul[_currentPoint].position, _speed * Time.deltaTime);
        transform.LookAt(_pointPatrul[_currentPoint]);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PointPatrul>(out PointPatrul pointPatrul))
        {
            _currentPoint = ++_currentPoint % _pointPatrul.Length;
        }
    }
}
