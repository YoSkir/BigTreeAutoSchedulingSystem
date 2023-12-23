using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShiftController : MonoBehaviour
{
    public void SetDaysOff()
    {
        DateController dateController = CentralProcessor.Instance.DateController;
        List<ShiftData> shiftDatas = CentralProcessor.ASSData.MonthlyShiftData;
        for(int j=0;j<shiftDatas.Count;j++)
        {
            for(int i = 0; i < shiftDatas[j].ShiftTimes.Length; i++)
            {
                if (dateController.ContainsDate(CentralProcessor.ASSData.StoreStaffData[i].DaysOff,shiftDatas[j].Date))
                {
                    shiftDatas[j].ShiftTimes[i].ShiftTimeText = SetShiftText(ShiftTimeData.ShiftStatusEnum.指休);
                }
                else
                {
                    shiftDatas[j].ShiftTimes[i].ShiftTimeText = "";
                }
            }
        }
    }
    public string SetShiftText(ShiftTimeData shiftTimeData)
    {
        string result="";
        switch (shiftTimeData.StartTime)
        {

        }
        shiftTimeData.ShiftTimeText = result;
        return result;
    }
    public string SetShiftText(ShiftTimeData.ShiftStatusEnum shiftStatusEnum)
    {
        switch (shiftStatusEnum)
        {
            case ShiftTimeData.ShiftStatusEnum.特休:
                return "特休";
            case ShiftTimeData.ShiftStatusEnum.指休:
                return "指休";
            case ShiftTimeData.ShiftStatusEnum.排休:
                return "排休";
            default:
                return "";
        }
    }
    public void SetShiftTime(int openHour,int closeHour) //24hr  半小時一個單位：早上九點半=19;
    {
        ASSData assData= CentralProcessor.ASSData;
        assData.OpenHour = openHour;
        assData.CloseHour = closeHour;
        assData.TimeDuration = closeHour - openHour;
        for(int i = 0; i < assData.MonthlyShiftData.Count; i++)
        {
            StaffData[][] workHour = new StaffData[assData.TimeDuration][];
            assData.MonthlyShiftData[i].WorkHour = workHour;
        }
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
            for(int i = 0; i < assData.MonthlyShiftData.Count; i++)
            {
                for(int j = 0; j < assData.MonthlyShiftData[0].WorkHour.Length; j++)
                {
                    assData.MonthlyShiftData[i].WorkHour[j] = new StaffData[everyHourStaffCount[j]];
                }
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
                    assData.MonthlyShiftData[i].WorkHour[j] = new StaffData[staffCount];
                }
            }
        }
    }
    private int TimeToShiftTimeIndex(int time)
    {
        return time - CentralProcessor.ASSData.OpenHour;
    }
}

