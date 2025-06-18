using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _startPointSpawnPosition; // Позиция начала спавна ресурсов
    [SerializeField] private Transform _endPointSpawnPosition; // Позиция конца спавна ресурсов
    [SerializeField] private Transform _container; // Контейнер, в который будут помещаться созданные ресурсы
    [SerializeField] private Resurs _prefabResurs; // Префаб ресурса, который будет создаваться
    [SerializeField] private float _delay; // Задержка между спавном ресурсов

    private WaitForSeconds _wait; // Переменная для хранения задержки между спавном ресурсов

    private void Start() // Метод Start вызывается при запуске скрипта
    {
        _wait = new WaitForSeconds(_delay); // Инициализация задержки
        StartCoroutine(SpawnResurs()); // Запуск корутины для спавна ресурсов
    }
    private IEnumerator SpawnResurs() // Корутина для спавна ресурсов
    {
        while (enabled) // Пока скрипт включен
        {
            // Создание нового ресурса в случайной позиции между начальной и конечной точками спавна
            Instantiate(_prefabResurs,
                new Vector3(
                    Random.Range(_startPointSpawnPosition.position.x, _endPointSpawnPosition.position.x),
                    1,
                    Random.Range(_startPointSpawnPosition.position.z, _endPointSpawnPosition.position.z)),
                    Quaternion.identity,
                    _container);
            yield return _wait; // Ждем указанное время перед следующим спавном
        }
    }
}
