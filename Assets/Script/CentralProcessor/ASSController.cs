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
}
