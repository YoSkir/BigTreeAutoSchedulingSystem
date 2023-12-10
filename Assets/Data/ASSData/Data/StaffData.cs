using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StaffData_",menuName ="StaffData")]

public class StaffData : ScriptableObject
{    
    List<Date> daysOff;
    int colum; //方便此員工位於第幾行
    int totalDaysOff, totalWorkHours; 
    int continuousWorkDays, continuousDayOff; //計算連續上班日與連續休假日 降低或增加排班優先度
    bool lastDayCloseShift; //檢查前一天是否為關班 盡量避免關班接早班
    string staffName,staffNumber,staffLevel;

    public int Colum
    {
        get => colum;
        set
        {
            colum = value;
        }
    }
    public int TotalDaysOff
    {
        get => totalDaysOff;
        set
        {
            totalDaysOff = value;
        }
    }
    public int TotalWorkHours
    {
        get => totalWorkHours;
        set
        {
            totalWorkHours = value;
        }
    }
    public int ContinuousWorkDays
    {
        get => continuousWorkDays;
        set
        {
            continuousWorkDays = value;
        }
    }
    public int ContinuousDayOff
    {
        get => continuousDayOff;
        set
        {
            continuousDayOff = value;
        }
    }
    public bool LastDayCloseShift
    {
        get => lastDayCloseShift;
        set
        {
            lastDayCloseShift = value;
        }
    }
    public List<Date> DaysOff
    {
        get => daysOff;
        set
        {
            daysOff = value;
        }
    }
    public string StaffName
    {
        get => staffName;
        set
        {
            staffName = value;
        }
    }
    public string StaffNumber
    {
        get => staffNumber;
        set
        {
            staffNumber = value;
        }
    }
    public string StaffLevel
    {
        get => staffLevel;
        set
        {
            staffLevel = value;
        }
    }
}
