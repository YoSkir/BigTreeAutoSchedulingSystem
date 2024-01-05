using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
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
    public void CountTotalWorkHours()
    {
        ASSData aSSData = CentralProcessor.ASSData;
        foreach(StaffData staff in aSSData.StoreStaffData)
        {
            staff.TotalWorkHours = 0;
        }
        foreach (ShiftData shift in aSSData.MonthlyShiftData)
        {
            foreach (List<StaffData> hour in shift.WorkHour)
            {
                foreach(StaffData staff in hour)
                {
                    staff.TotalWorkHours++;
                }
            }
        }
    }
    public void CountTotalDayOff()
    {
        //currently i still count non-scheduled day as day off
        ASSData aSSData = CentralProcessor.ASSData;
        foreach (StaffData staff in aSSData.StoreStaffData)
        {
            staff.TotalDaysOff = 0;
        }
        foreach (ShiftData shift in aSSData.MonthlyShiftData)
        {
            foreach (StaffData staff in aSSData.StoreStaffData)
            {
                if (GetWorkHoursADay(staff, shift) == 0)
                {
                    staff.TotalDaysOff++;
                }
            }
        }
    }
    private int GetWorkHoursADay(StaffData staff, ShiftData shift)
    {
        int workHour = 0;
        foreach (List<StaffData> workingHour in shift.WorkHour)
        {
            if (workingHour.Contains(staff))
            {
                workHour++;
            }
        }
        return workHour;
    }
    public enum StaffStatus { TotalDaysOff,TotalWorkHours,PriorityScore}
    //ContinuousDayOff, ContinuousWorkDays, ContinuousOffHours, ContinuousWorkHours,

    public int GetContinuousWorkDay(StaffData staff,ShiftData shift)
    {
        ASSData aSSData=CentralProcessor.ASSData;
        int workdayCount= 0;
        int indexOfShift=aSSData.MonthlyShiftData.IndexOf(shift);
        //未來取得前一個月班表資料
        while (indexOfShift-workdayCount>0)
        {
            if(GetWorkHoursADay(staff, aSSData.MonthlyShiftData[indexOfShift - 1 - workdayCount]) > 0)
            {
                workdayCount++;
            }
            else
            {
                break;
            }
        }
        return workdayCount;
    }

}

