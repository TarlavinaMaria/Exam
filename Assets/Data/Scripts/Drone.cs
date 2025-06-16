using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] float _speed;

    private Transform[] _pointPatrul; // массив точек
    private int _currentPoint = 0; // 
    private Transform _comandCenterPoint;
    private bool _isReady = false;
    private bool _isHaveTarget = false;
    private bool _haveResurs = false;
    private Transform _target;

    private void Update()
    {
        if (_isReady == true)
        {
            if (_isHaveTarget == true)
            {
                // Если есть цель, двигается к цели
                MoveToTarget(_target);
            }
            else
            {
                if (_haveResurs == true)
                {
                    // возврат на базу
                    MoveToTarget(_comandCenterPoint);
                }
                else
                {
                    FreeMove();
                }
            }
        }
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

    private void MoveToTarget(Transform target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);
        transform.LookAt(target.position);
    }
    public void TakePositionComandCenter(Transform comandCenter)
    {
        _comandCenterPoint = comandCenter;
    }
    public void TakePatrulPoint(Transform[] pointPatrul)
    {
        _pointPatrul = pointPatrul;
        _isReady = true;
    }
    public void TakeTarget(Transform tagret)
    {
        _target = tagret;
        _isHaveTarget = true;
    }
}
