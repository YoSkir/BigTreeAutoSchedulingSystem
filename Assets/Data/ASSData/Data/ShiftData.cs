using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ShiftData_",menuName ="ShiftData")]

public class ShiftData: ScriptableObject
{
    private Date date;
    private string calender = "";
    private ShiftTimeData[] shiftTimes; //每格代表每個員工今天的班表
    private StaffData[][] WorkHour;  //[工作小時][上班者]
    private int line;

    public Date Date
    {
        get => date;
        set
        {
            date = value;
        }
    }
    public string Calender
    {
        get => calender;
        set
        {
            calender = value;
        }
    }
    public int Line
    {
        get => line;
        set
        {
            line = value;
        }
    }
    public ShiftTimeData[] ShiftTimes
    {
        get => shiftTimes;
        set
        {
            shiftTimes = value;
        }
    }
        
}