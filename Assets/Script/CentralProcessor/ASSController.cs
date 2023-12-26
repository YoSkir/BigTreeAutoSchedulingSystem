using System;
using System.Collections;
using System.Collections.Generic;
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
            shiftData.ShiftTimes = SetShiftTimeData(CentralProcessor.ASSData.StoreStaffData.Length);
            shiftData.AvailibleStaff = GetStaffsAvailibleForWork(CentralProcessor.ASSData.StoreStaffData, rangeDates[i]);
            tempMonthlyShift.Add(shiftData);
        }
        CentralProcessor.ASSData.MonthlyShiftData = tempMonthlyShift;
    }
    private int setShiftDuration(Date startDate,Date endDate)
    {
        return CentralProcessor.Instance.DateController.GetDaysCount(startDate, endDate);
    }

    private ShiftTimeData[] SetShiftTimeData(int StaffCounts)
    {
        ShiftTimeData[] shiftTimeDatas = new ShiftTimeData[StaffCounts];
        for(int i = 0; i < StaffCounts; i++)
        {
            ShiftTimeData shiftTimeData = ScriptableObject.CreateInstance<ShiftTimeData>();
            shiftTimeDatas[i] = shiftTimeData;
        }
        return shiftTimeDatas;
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
                case StaffController.StaffStatus.ContinuousDayOff:
                case StaffController.StaffStatus.TotalDaysOff:
                case StaffController.StaffStatus.ContinuousOffHours://High priority: High value
                    for (int i = 0; i < storeStaffs.Length; i++)
                    {
                        storeStaffs[i].PriorityScore += storeStaffs.Length-i;
                    }
                    break;
                case StaffController.StaffStatus.ContinuousWorkDays:
                case StaffController.StaffStatus.TotalWorkHours:
                case StaffController.StaffStatus.ContinuousWorkHours://High priority: Low value
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
    private List<StaffData> GetStaffsAvailibleForWork(StaffData[] storeStaffs, Date date)
    {
        List<StaffData> staffsAvailible = new List<StaffData>();
        foreach (StaffData staff in storeStaffs)
        {
            if (!CheckStaffDayOff(staff, date))
            {
                staffsAvailible.Add(staff);
            }
        }
        return staffsAvailible;
    }
    private bool CheckStaffDayOff(StaffData staff, Date date)
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
}
