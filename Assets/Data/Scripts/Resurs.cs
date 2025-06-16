using UnityEngine;

public class Resurs : MonoBehaviour
{
    public bool IsIncludeFree { get; private set; } = false;

    public void SetInclude()
    {
        IsIncludeFree = true;
    }
    public void StandartSetting()
    {
        IsIncludeFree = false;
    }

}
