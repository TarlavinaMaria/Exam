using UnityEngine;
using UnityEngine.UI;

public class DrowCounter : MonoBehaviour
{
    [SerializeField] private Text _text; // Текст который воводится на экран
    public void DrowCountResurs(int count) // Метод для показания количества ресурсов на экране
    {
        _text.text = count.ToString(); // Устанавливаем текстовое значение в компоненте Text
    }
}
