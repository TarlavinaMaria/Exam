using System.Collections.Generic;
using UnityEngine;

public class Scaner : MonoBehaviour
{
    [SerializeField] private float _scaneRadius; // Радиус сканирования для обнаружения ресурсов

    public Queue<Resurs> Scane(Queue<Resurs> resurses) // Метод для сканирования ресурсов в заданном радиусе
    {
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, _scaneRadius); // Получаем все коллайдеры в радиусе сканирования

        foreach (Collider collider in triggerColliders) // Проходим по каждому коллайдеру
        {
            if (collider.gameObject.TryGetComponent<Resurs>(out Resurs resurs)) // Проверяем, является ли объект ресурсом
            {
                if (!resurses.Contains(resurs)) // Проверяем, находится ли ресурс уже в очереди
                {
                    if (!resurs.IsIncludeFree) // Проверяем, не включен ли ресурс уже в обработку
                    {
                        resurs.SetInclude(); // Устанавливаем ресурс как включенный в обработку
                        resurses.Enqueue(resurs); // Добавляем ресурс в очередь для обработки
                    }
                }
            }
        }

        return resurses; // Возвращаем очередь ресурсов, которые были обнаружены в радиусе сканирования
    }
    private void OnDrawGizmos() // Метод для отрисовки Gizmos в редакторе Unity, видимый только в редакторе
    {
        Gizmos.color = Color.yellow; // Устанавливаем цвет Gizmos в желтый
        Gizmos.DrawWireSphere(transform.position, _scaneRadius); // Отрисовываем сферу вокруг позиции сканера с заданным радиусом
    }
}
