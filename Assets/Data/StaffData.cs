using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="StaffData_",menuName ="StaffData")]

public class StaffData : ScriptableObject
{
    List<Date> daysOff;
    string staffName,staffNumber,staffLevel;

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
