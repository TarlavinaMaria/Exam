using UnityEngine;

public class Resurs : MonoBehaviour
{
    public bool IsIncludeFree { get; private set; } = false; // Флаг, указывающий, включен ли ресурс в обработку

    public void SetInclude() // Метод для установки флага включения ресурса в обработку
    {
        IsIncludeFree = true; // Устанавливаем флаг в true, что означает, что ресурс включен в обработку
    }
    public void StandartSetting() // Метод для сброса флага включения ресурса в обработку
    {
        IsIncludeFree = false; // Сбрасываем флаг включения ресурса в обработку
    }

}
