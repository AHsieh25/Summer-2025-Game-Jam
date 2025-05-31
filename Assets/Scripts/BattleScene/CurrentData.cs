using UnityEngine;

public class CurrentData : MonoBehaviour
{
    [SerializeField] public currentData currentData = new currentData();
}

[System.Serializable]
public class currentData
{
    public UnitStats stats;
}