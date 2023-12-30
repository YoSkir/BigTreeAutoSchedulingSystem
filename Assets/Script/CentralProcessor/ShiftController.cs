using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

}

