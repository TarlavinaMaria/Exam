using System.Collections.Generic;
using UnityEngine;

public class ComandCenter : MonoBehaviour
{
    [SerializeField] private Scaner _scaner;
    [SerializeField] private Transform[] _patrulPoint;

    private Queue<Resurs> _resursers = new Queue<Resurs>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_scaner.Scane(_resursers).Count);
            //_scaner.Scane(_resursers);
        }
    }
}
