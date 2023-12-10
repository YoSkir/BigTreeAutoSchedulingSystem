using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASSData_", menuName = "ASSData")]

public class ASSData : ScriptableObject
{
    Date startDate, endDate;
    bool firstOpen=true; //
    List<ShiftData> monthlyShiftData;
    StaffData[] storeStaffData;
    int shiftDuration;

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
    public StaffData[] StoreStaffData
    {
        get => storeStaffData;
        set
        {
            storeStaffData = value;
        }
    }

    public bool FirstOpen
    {
        get => firstOpen;
        set
        {
            firstOpen = value;
        }
    }
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
}
