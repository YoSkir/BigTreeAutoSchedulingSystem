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
}

