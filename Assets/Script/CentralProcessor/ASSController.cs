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
    public StaffData[] GetStaffPriorityRate(StaffData[] storeStaffs)
    {
        foreach(int statusIndex in Enum.GetValues(typeof(StaffController.StaffStatus)))
        {
            storeStaffs=GetStatusRate((StaffController.StaffStatus)statusIndex, storeStaffs);
            for(int i = 0;i < storeStaffs.Length; i++)
            {
                storeStaffs[i].PriorityScore += i + 1; //記得歸零 與 判斷倒敘
            }
        }
        
        
    }
    //必須判斷是否有同數值
    public StaffData[] GetStatusRate(StaffController.StaffStatus status, StaffData[] storeStaffs)
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
}
