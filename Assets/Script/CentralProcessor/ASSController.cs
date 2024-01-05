using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ASSController : MonoBehaviour
{    
    //在主邏輯確保排班時間    
    public void SetMonthlyShiftDate(Date startDate,Date endDate)
    {
        CentralProcessor.ASSData.ShiftDuration = setShiftDuration(startDate, endDate);
        List<Date> rangeDates = CentralProcessor.Instance.DateController.GetRangeDate(startDate, endDate);
        List<ShiftData> tempMonthlyShift = new List<ShiftData>();
        for (int i = 0; i < CentralProcessor.ASSData.ShiftDuration; i++)
        {
            ShiftData shiftData = ScriptableObject.CreateInstance<ShiftData>();
            shiftData.Date = rangeDates[i];
            shiftData.Line = i;
            shiftData.WorkHour = new List<StaffData>[CentralProcessor.ASSData.TimeDuration];
            for(int time=0;time<shiftData.WorkHour.Length;time++)
            {
                shiftData.WorkHour[time] = new List<StaffData>();
            }
            shiftData.AvailibleStaff = new List<StaffData>();
            shiftData.SecondAvailibleStaff = new List<StaffData>();
            tempMonthlyShift.Add(shiftData);
        }
        CentralProcessor.ASSData.MonthlyShiftData = tempMonthlyShift;
    }
    private int setShiftDuration(Date startDate,Date endDate)
    {
        return CentralProcessor.Instance.DateController.GetDaysCount(startDate, endDate);
    }
    public StaffData[] GetStaffPriorityRate(StaffData[] storeStaffs) //priority score : no.1 +1, no.6 +6
    {
        for (int colum = 0; colum < storeStaffs.Length; colum++) //Set priority score to 0.
        {
            storeStaffs[colum].PriorityScore = 0;
        }
        foreach (int statusIndex in Enum.GetValues(typeof(StaffController.StaffStatus)))
        {
            StaffController.StaffStatus status = (StaffController.StaffStatus)statusIndex;
            storeStaffs = GetStatusRate(status, storeStaffs);
            switch (status)
            {
                //case StaffController.StaffStatus.ContinuousDayOff:
                case StaffController.StaffStatus.TotalDaysOff:
                //case StaffController.StaffStatus.ContinuousOffHours://High priority: High value
                    for (int i = 0; i < storeStaffs.Length; i++)
                    {
                        storeStaffs[i].PriorityScore += storeStaffs.Length-i;
                    }
                    break;
                //case StaffController.StaffStatus.ContinuousWorkDays:
                case StaffController.StaffStatus.TotalWorkHours:
                //case StaffController.StaffStatus.ContinuousWorkHours://High priority: Low value
                    for (int i = 0; i < storeStaffs.Length; i++)
                    {
                        storeStaffs[i].PriorityScore += i + 1;
                    }
                    break;
                case StaffController.StaffStatus.PriorityScore:break;
                default:break;
            }
        }
        return storeStaffs;
    }
    //必須判斷是否有同數值
    //high priority: h          l                h            l                l                    h
    //ContinuousDayOff, ContinuousWorkDays,TotalDaysOff,TotalWorkHours,ContinuousWorkHours,ContinuousOffHours,priorityscore
    private StaffData[] GetStatusRate(StaffController.StaffStatus status, StaffData[] storeStaffs) //low => high
    {
        for(int i=1;i<storeStaffs.Length; i++)
        {
            for (int j=0;j<storeStaffs.Length-i;j++)
            {
                StaffData temp;
                if (storeStaffs[j].StaffStatus(status) > storeStaffs[j + 1].StaffStatus(status))
                {
                    temp = storeStaffs[j];
                    storeStaffs[j] = storeStaffs[j +1];
                    storeStaffs[j +1]=temp;
                }
            }
        }
        return storeStaffs;
    }
    public bool CheckStaffDayOff(StaffData staff, Date date)
    {
        foreach (Date dayOff in staff.DaysOff)
        {
            if (dayOff.DateString.Equals(date.DateString))
            {
                return true;
            }
        }
        return false;
    }
    public int GetTodayWorkHours(StaffData staff, ShiftData shift)
    {
        int todayWorkHours = 0;
        foreach (List<StaffData> staffOnWork in shift.WorkHour)
        {
            todayWorkHours += staffOnWork.Contains(staff) ? 1 : 0;
        }
        return todayWorkHours;
    }
    public bool CheckShiftError(StaffData staff,ShiftData shift)//時數1~13 分段時數 
    {
        if(GetTodayWorkHours(staff, shift) >0 && GetTodayWorkHours(staff, shift) < 13)
        {
            Debug.Log("檢測到錯誤時數");
            return true;
        }
        else if (CheckSeperateShift(staff,shift))
        {
            Debug.Log("檢測到分離班表");
            return true;
        }
        else { return false; }
    }
    private bool CheckSeperateShift(StaffData staff,ShiftData shift)
    {
        bool firstShiftStart=false,firstShiftEnd = false;
        foreach(List<StaffData> hour in shift.WorkHour)
        {
            if(hour.Contains(staff))
            {
                if (firstShiftEnd)
                {
                    return true;
                }
                else
                {
                    if(!firstShiftStart)
                    {
                        firstShiftStart = true; 
                    }
                }
            }
            else
            {
                if (firstShiftStart)
                {
                    firstShiftEnd = true;
                }
            }
        }
        return false;
    }

    internal string GetShiftText(StaffData staff, ShiftData shift)
    {
        string shiftText = "";
        int shiftLength=GetTodayWorkHours(staff,shift);
        string breakText = shiftLength > 19 ? "休1" : "休0.5";
        for(int time =0;time<shift.WorkHour.Count();time++)
        {
            if (shift.WorkHour[time].Contains(staff))
            {             
                shiftText += GetTimeText(time)+"-"+GetTimeText(time+shiftLength)+breakText;
                break;
            }
        }
        return shiftText;
    }
    private string GetTimeText(int timeIndex)
    {
        string timeText = "";
        int storeOpenTime = CentralProcessor.ASSData.OpenHour;
        timeText = ((storeOpenTime + timeIndex) / 2).ToString();
        if ((storeOpenTime + timeIndex) % 2 > 0)
        {
            timeText += "30";
        }
        else
        {
            timeText += "00";
        }
        return timeText;
    }
}
