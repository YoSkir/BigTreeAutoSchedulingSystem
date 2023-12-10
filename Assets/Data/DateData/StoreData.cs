using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StoreData_", menuName = "StoreData")]

public class StoreData : ScriptableObject
{
    private List<ShiftData> monthlyShiftData;
    private List<StaffData> storeStaffData;
    private int shiftDuration;

    public List<ShiftData> MonthlyShiftData
    {
        get => monthlyShiftData;
        set
        {
            monthlyShiftData = value;
        }
    }
    public int ShiftDuration
    {
        get => shiftDuration;
        set
        {
            shiftDuration = value;
        }
    }
    public List<StaffData> StoreStaffData
    {
        get => storeStaffData;
        set
        {
            storeStaffData = value;
        }
    }
}

