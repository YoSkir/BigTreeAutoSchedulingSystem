using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StaffData_",menuName ="StaffData")]

public class StaffData : ScriptableObject
{    
    //all one hour down here =2 in code.
    List<Date> daysOff;
    int colum; //方便此員工位於第幾行
    int totalDaysOff, totalWorkHours; 
    int continuousWorkDays, continuousDayOff; //計算連續上班日與連續休假日 降低或增加排班優先度
    int continuousWorkHours, continuousOffHours; //Counting the contiuous working or resting hours of the staff
    int priorityScore;
    bool lastDayCloseShift,isManager; //檢查前一天是否為關班 盡量避免關班接早班,確認是否為主管職
    string staffName,staffNumber,staffLevel;
    List<SpecialShiftData> specialShifts; // 

    public int StaffStatus(StaffController.StaffStatus staffStatus)
    {
        switch(staffStatus)
        {
            case StaffController.StaffStatus.TotalDaysOff: return totalDaysOff;
            case StaffController.StaffStatus.TotalWorkHours: return totalWorkHours;
            case StaffController.StaffStatus.ContinuousWorkDays: return continuousWorkDays;
            case StaffController.StaffStatus.ContinuousDayOff: return continuousDayOff;
            case StaffController.StaffStatus.ContinuousWorkHours: return continuousWorkHours;
            case StaffController.StaffStatus.ContinuousOffHours: return continuousOffHours;
            case StaffController.StaffStatus.PriorityScore: return priorityScore;
            default: return -1;
        }
    }
    public int PriorityScore
    {
        get => priorityScore; set { priorityScore = value; }
    }
    public int ContinuousWorkHours
    {
        get => continuousWorkHours; set => continuousWorkHours = value;
    }
    public int ContinuousOffHours
    {
        get => continuousOffHours; set => continuousOffHours = value;
    }
    public bool IsManager
    {
        get => isManager;
        set
        {
            isManager = value;
        }
    }
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
