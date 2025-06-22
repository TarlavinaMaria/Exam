using UnityEngine;

public class ResursCounter : MonoBehaviour
{
    // Параметры
    [SerializeField] DrowCounter _drowCounter;

    // Переменные
    private int _resursCounter = 0; // Счетчик ресурсов

    public void AddResurs(int count = 1) // Метод для добавления ресурсов
    {
        _resursCounter += count; // Увеличиваем счетчик ресурсов на заданное количество
        _drowCounter.DrowCountResurs(_resursCounter); // Обновляем отображение счетчика ресурсов
    }
    public void RemoveResurs(int count) // Метод для удаления ресурсов
    {
        _resursCounter -= count; // Уменьшаем счетчик ресурсов на заданное количество
        if (_resursCounter < 0) _resursCounter = 0; // Проверяем, чтобы счетчик не стал отрицательным
        _drowCounter.DrowCountResurs(_resursCounter); // Обновляем отображение счетчика ресурсов
    }
}
