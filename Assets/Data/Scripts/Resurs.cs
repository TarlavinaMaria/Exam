using System.Collections;
using UnityEngine;

public class Resurs : MonoBehaviour
{
    // Переменные
    public bool IsIncludeFree { get; private set; } = false; // Флаг, указывающий, включен ли ресурс в обработку

    public void SetInclude() // Метод для установки флага включения ресурса в обработку
    {
        IsIncludeFree = true; // Устанавливаем флаг в true, что означает, что ресурс включен в обработку
        StartCoroutine(AutoReset()); // Запускаем авто-сброс
    }
    public void StandartSetting() // Метод для сброса флага включения ресурса в обработку
    {
        IsIncludeFree = false; // Сбрасываем флаг включения ресурса в обработку
    }
    private IEnumerator AutoReset()
    {
        yield return new WaitForSeconds(10f); // Ждём время до сброса
        StandartSetting(); // Сбрасываем флаг включения ресурса в обработку
    }
}
