using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASSData_", menuName = "ASSData")]

public class ASSData : ScriptableObject
{
    Date startDate, endDate;
    int openHour, closeHour,timeDuration; //1=0.5hour
    bool firstOpen=true; //
    List<ShiftData> monthlyShiftData;
    StaffData[] storeStaffData;
    int shiftDuration;

    public int TimeDuration
    {
        get => timeDuration;
        set
        {
            timeDuration = value;
        }
    }
    public int OpenHour
    {
        get => openHour;
        set
        {
            openHour=value;
        }
    }
    public int CloseHour
    {
        get => closeHour;
        set
        {
            closeHour = value;
        }
    }
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
