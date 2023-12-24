using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

public class SystemSetup : MonoBehaviour
{
    //暫時設定區
    string[] staffName,a,b;
    int[] date;
    //
    public void testSetup()
    {
        //第一次開啟，設定員工
        staffName = new string[] { "吳玟頤", "阿蛙", "被被", "伍佑群", "臭臭" };
        a = new string[] { "12311", "41156", "78229", "11321", "22212" };
        b = new string[] { "店長", "店員", "藥師", "店員", "店員" };
        SetStaff();
        //設定日期
        date = new int[] { 2023, 10, 29, 2023, 11, 25 };
        SetDate(date);
        //設定指休
        RandomDaysOff(5);
        foreach(StaffData s in CentralProcessor.ASSData.StoreStaffData)
        {
            Debug.Log(s.StaffName + "休假日："+s.DaysOff.Count+"天");
            string dd="";
            foreach(Date d in s.DaysOff)
            {
                dd += d.DateString + " ";
            }
            Debug.Log(dd);
        }
        CentralProcessor.ASSData.FirstOpen = false;
        CentralProcessor.Instance.ShiftController.SetDaysOff();
        //設定店營業時間
        CentralProcessor.Instance.ShiftController.SetShiftTime(18, 44);
        CentralProcessor.Instance.TimeShow(18);
        CentralProcessor.Instance.TimeShow(44);
        int[] staffCount = { 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2, 2, 2 };
        CentralProcessor.Instance.ShiftController.SetShiftStaffCount(staffCount);
        CentralProcessor.Instance.ShiftController.SetShiftStaffCount(ShiftController.WeekDay.六, 40,43, 3);
        CentralProcessor.Instance.ShiftController.SetShiftStaffCount(ShiftController.WeekDay.日, 40,43, 3);
    }
    private void SetStaff()
    {
        CentralProcessor.ASSData.StoreStaffData =
            CentralProcessor.Instance.StaffController.SetStaffList(staffName, a, b);
    }
    private void SetDaysOff(StaffData staff,Date date)
    {
        staff.DaysOff.Add(date);
    }
    private void SetDate(int[] date)
    {
        Date startDate= CentralProcessor.Instance.DateController.SetDate(date[0], date[1], date[2]);
        Date endDate= CentralProcessor.Instance.DateController.SetDate(date[3], date[4], date[5]);
        CentralProcessor.ASSData.StartDate = startDate;
        CentralProcessor.ASSData.EndDate = endDate;
        CentralProcessor.Instance.ASSController.SetMonthlyShiftDate(startDate,endDate);
    }
    private Date RandomDate(Date startDate,Date endDate)  ///for test only
    {
        DateController dateController = CentralProcessor.Instance.DateController;
        int year = (int)Random.Range(startDate.Year, endDate.Year+1);
        int month = (int)Random.Range(startDate.Month, endDate.Month+1);
        int day;
        if (month == startDate.Month)
        {
            day = Random.Range(startDate.Day, startDate.LastDate+1);
        }
        else if(month==endDate.Month)
        {
            day= Random.Range(1, endDate.Day + 1);
        }
        else
        {
            int tempLastDate = dateController.DaysOfMonth(year, month)+1;
            day = Random.Range(1, tempLastDate);
        }
        Date tempDate = ScriptableObject.CreateInstance<Date>();
        tempDate = dateController.SetDate(year, month, day);
        return tempDate;
    }
    private void RandomDaysOff(int daysCount) ///for test only
    {
        for(int i =0;i< CentralProcessor.ASSData.StoreStaffData.Length; i++)
        {
            List<Date> daysOffTemp = new List<Date>();
            for (int j = 0; j < daysCount; j++)
            {
                daysOffTemp.Add(RandomDate(CentralProcessor.ASSData.StartDate, CentralProcessor.ASSData.EndDate));
            }
            CentralProcessor.ASSData.StoreStaffData[i].DaysOff = daysOffTemp;
        }
    }
}

