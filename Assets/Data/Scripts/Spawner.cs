using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _startPointSpawnPosition;
    [SerializeField] private Transform _endPointSpawnPosition;
    [SerializeField] private Transform _container;
    [SerializeField] private Resurs _prefabResurs;
    [SerializeField] private float _delay;

    private WaitForSeconds _wait;

    private void Start()
    {
        _wait = new WaitForSeconds(_delay);
        StartCoroutine(SpawnResurs());
    }
    private IEnumerator SpawnResurs()
    {
        while (enabled)
        {
            Instantiate(_prefabResurs,
                new Vector3(
                    Random.Range(_startPointSpawnPosition.position.x, _endPointSpawnPosition.position.x),
                    2,
                    Random.Range(_startPointSpawnPosition.position.z, _endPointSpawnPosition.position.z)),
                    Quaternion.identity,
                    _container);
            yield return _wait;
        }
    }
}
