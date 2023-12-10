using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffController : MonoBehaviour
{
    public StaffData SetStaffInfo(string name,string number,string level)
    {
        StaffData staff = ScriptableObject.CreateInstance<StaffData>();
        staff.StaffName = name.Length<5?name:"";
        staff.StaffNumber = number.Length==5?number:"";
        staff.StaffLevel = level.Length<3?level:"";
        return staff;
    }
    public List<Date> AddDaysOff(List<Date> currentDaysOff,Date newDaysOff)
    {
        currentDaysOff.Add(newDaysOff);
        return currentDaysOff;
    }
}

