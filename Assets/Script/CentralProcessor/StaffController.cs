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
        staff.IsManager = !level.Equals("店員");
        staff.StaffLevel = level.Length<3?level:"";
        return staff;
    }
    public void AddDaysOff(StaffData staff,Date newDaysOff)
    {
        staff.DaysOff.Add(newDaysOff);
    }
    public StaffData[] SetStaffList(string[] nameList, string[] numberList, string[] levelList)
    {
        StaffData[] staffDatas = new StaffData[nameList.Length];
        for(int i = 0; i < staffDatas.Length; i++) //i=colum of staff in store staffs
        {
            staffDatas[i] = SetStaffInfo(nameList[i], numberList[i], levelList[i]);
            staffDatas[i].Colum = i;
            staffDatas[i].ContinuousDayOff = 0;
            staffDatas[i].ContinuousWorkDays = 0;
            staffDatas[i].LastDayCloseShift = false;
            staffDatas[i].TotalDaysOff = 0;
            staffDatas[i].TotalWorkHours = 0;
            staffDatas[i].ContinuousWorkHours = 0;
            staffDatas[i].ContinuousOffHours = 0;
        }
        return staffDatas;
    }
    public enum StaffStatus { ContinuousDayOff, ContinuousWorkDays,TotalDaysOff,TotalWorkHours, ContinuousOffHours, ContinuousWorkHours,PriorityScore}
    
}

