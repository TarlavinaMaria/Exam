using UnityEngine;

public class ResursCounter : MonoBehaviour
{
    [SerializeField] DrowCounter _drowCounter;

    private int _resursCounter = 0;
    public void AddResurs(int count = 1)
    {
        _resursCounter += count;
        _drowCounter.DrowCountResurs(_resursCounter);
    }
    public void RemoveResurs(int count)
    {
        _resursCounter -= count;
        if (_resursCounter < 0) _resursCounter = 0;
        _drowCounter.DrowCountResurs(_resursCounter);
    }

}
