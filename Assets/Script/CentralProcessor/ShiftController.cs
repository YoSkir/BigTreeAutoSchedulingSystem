using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShiftController : MonoBehaviour
{
    public void SetShiftTime(int openHour,int closeHour) //24hr  半小時一個單位：早上九點半=19;
    {
        ASSData assData= CentralProcessor.ASSData;
        assData.OpenHour = openHour;
        assData.CloseHour = closeHour;
        assData.TimeDuration = closeHour - openHour;
    }
    public void SetShiftStaffCount(int[] everyHourStaffCount)
    {
        ASSData assData = CentralProcessor.ASSData;
        if (everyHourStaffCount.Length != assData.TimeDuration)
        {
            Debug.Log("進行班時間人數設置時班時間長度輸入錯誤");
        }
        else
        {
            foreach(ShiftData shift in assData.MonthlyShiftData)
            {
                shift.RequireStaffs = everyHourStaffCount;
            }
        }
    }
    public enum WeekDay { 一,二,三,四,五,六,日};
    public void SetShiftStaffCount(WeekDay weekDay,int startTime,int endTime,int staffCount)
    {
        if (startTime>endTime||startTime < CentralProcessor.ASSData.OpenHour || endTime >= CentralProcessor.ASSData.CloseHour)
        {
            Debug.Log("時間輸入錯誤");
        }
        ASSData assData = CentralProcessor.ASSData;
        for(int i=0;i<assData.MonthlyShiftData.Count;i++)
        {
            if (weekDay.ToString().Equals(assData.MonthlyShiftData[i].Date.WeekDay))
            {
                for(int j =  TimeToShiftTimeIndex(startTime); j <= TimeToShiftTimeIndex(endTime); j++)
                {
                   assData.MonthlyShiftData[i].RequireStaffs[TimeToShiftTimeIndex(j)]=staffCount;
                }
            }
        }
    }
    private int TimeToShiftTimeIndex(int time)
    {
        return time - CentralProcessor.ASSData.OpenHour;
    }
    /// <summary>
    /// 設定第一優先表、第二優先表、與整個月關班bool
    /// </summary>
    /// <param name="shiftData"></param>
    public void SetShiftAvailibleStaff(ShiftData shiftData)
    {
        SetAvailibleStaff(shiftData);
        SetSecondAvailibleStaff(shiftData);
        SetLastDayCloseShift();
    }
    /// <summary>
    /// 為整個月班表設定staffdata的lastDayCloseShift,未來新增班表第一天的設定
    /// </summary>
    /// <param name="shiftData"></param>
    private void SetLastDayCloseShift()
    {
        ASSData aSSData = CentralProcessor.ASSData;
        int lastHourIndex = aSSData.TimeDuration;
        for(int dayIndex = 1;dayIndex<aSSData.MonthlyShiftData.Count;dayIndex++) 
        {
            foreach(StaffData staff in aSSData.MonthlyShiftData[dayIndex].AvailibleStaff)
            {
                if (aSSData.MonthlyShiftData[dayIndex - 1].WorkHour[lastHourIndex].Contains(staff))
                {
                    staff.LastDayCloseShift = true;
                    Debug.Log("員工 " + staff.StaffName +" " + aSSData.MonthlyShiftData[dayIndex-1].Date.DateString +"關班");
                }
            }
        }
    }
    private void SetSecondAvailibleStaff(ShiftData shiftData)
    {
        StaffController staffController = CentralProcessor.Instance.StaffController;
        foreach(StaffData staff in shiftData.AvailibleStaff)
        {
            if (staffController.GetContinuousWorkDay(staff,shiftData) > 2)
            {
                Debug.Log(staff.StaffName + " 連續上班超過3天");
                shiftData.AvailibleStaff.Remove(staff);
                shiftData.SecondAvailibleStaff.Add(staff);
            }
        }
    }
    private void SetAvailibleStaff(ShiftData shiftData)
    {       
        foreach (StaffData staff in CentralProcessor.ASSData.StoreStaffData)
        {
            if (!CentralProcessor.Instance.ASSController.CheckStaffDayOff(staff,shiftData.Date))
            {
                shiftData.AvailibleStaff.Add(staff) ;
            }
        }
    }
}

