using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftController : MonoBehaviour
{    
    //在主邏輯確保排班時間
    public StoreData SetMonthlyShiftDate(Date startDate,Date endDate)
    {
        StoreData storeData = ScriptableObject.CreateInstance<StoreData>();
        storeData.ShiftDuration = setShiftDuration(startDate, endDate);
        List<Date> rangeDates = CentralProcessor.Instance.DateController.GetRangeDate(startDate, endDate);
        List<ShiftData> tempMonthlyShift = new List<ShiftData>();
        for (int i = 0; i < storeData.ShiftDuration; i++)
        {
            ShiftData shiftData = ScriptableObject.CreateInstance<ShiftData>();
            shiftData.Date = rangeDates[i];
            shiftData.Line = i;
            //
            shiftData.ShiftTimes = new ShiftTimeData[5];
            //
            tempMonthlyShift.Add(shiftData);
        }
        storeData.MonthlyShiftData = tempMonthlyShift;
        return storeData;
    }
    private int setShiftDuration(Date startDate,Date endDate)
    {
        return CentralProcessor.Instance.DateController.GetDaysCount(startDate, endDate);
    }
}
