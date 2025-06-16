using System.Collections.Generic;
using UnityEngine;

public class ComandCenter : MonoBehaviour
{
    [SerializeField] private Scaner _scaner;
    [SerializeField] private Transform[] _patrulPoint;
    [SerializeField] private Drone _dronePrefab;
    [SerializeField] private Transform _spawnPositionDron;
    [SerializeField] private Transform _droneConteiner;

    private Queue<Resurs> _resursers = new Queue<Resurs>();
    private Queue<Drone> _drons = new Queue<Drone>();

    private Drone _tempDrone;
    private bool _isHaveDrone = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_scaner.Scane(_resursers).Count);
            //_scaner.Scane(_resursers);
        }
        if (Input.GetKeyDown(KeyCode.E) && _isHaveDrone == false)
        {
            CreateDrons();
            _isHaveDrone = true;
        }

        if (_drons.Count > 0)
        {
            if (_resursers.Count > 0)
            {
                SentDron();
            }
        }
    }
    private void SentDron()
    {
        _tempDrone = _drons.Dequeue();
        _tempDrone.TakeTarget(_resursers.Dequeue().transform);
    }
    private void CreateDrons()
    {
        _tempDrone = Instantiate(_dronePrefab, _spawnPositionDron.position, Quaternion.identity, _droneConteiner);
        _tempDrone.TakePositionComandCenter(this.transform);
        _tempDrone.TakePatrulPoint(_patrulPoint);
        _drons.Enqueue(_tempDrone);
    }
}
