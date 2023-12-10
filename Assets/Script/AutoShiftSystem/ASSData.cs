using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASSData_", menuName = "ASSData")]

public class ASSData : ScriptableObject
{
    Date startDate, endDate;
    StoreData storeData;

    public Date StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
        }
    }
    public Date EndDate
    {
        get => endDate;
        set
        {
            endDate = value;
        }
    }
    public StoreData StoreData
    {
        get => storeData;
        set
        {
            storeData = value;
        }
    }
}
