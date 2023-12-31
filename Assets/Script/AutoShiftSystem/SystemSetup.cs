﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using System;
public class SystemSetup : MonoBehaviour
{
    //暫時設定區
    string[] staffName,a,b;
    int[] date;
    //
    public void testSetup()
    {
        //第一次開啟，設定員工
        staffName = new string[] { "吳玟頤", "阿蛙", "被被", "伍佑群", "阿瑞","妮妮" };
        a = new string[] { "12311", "41156", "78229", "11321", "22212","12345" };
        b = new string[] { "店長", "店員", "藥師", "店員", "店員" ,"店員"};
        SetStaff();
        //設定店營業時間
        CentralProcessor.Instance.ShiftController.SetShiftTime(18, 44);
        //設定日期
        date = new int[] { 2023, 10, 29, 2023, 11, 25 };
        SetDate(date);
        //設定指休
        RandomDaysOff(3);//
        foreach (StaffData s in CentralProcessor.ASSData.StoreStaffData)
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
        CentralProcessor.ASSData.NoManagerOK = true;//左營大路不須額外人力補主管缺

        CentralProcessor.Instance.TimeShow(18);
        CentralProcessor.Instance.TimeShow(44);
        int[] staffCount = { 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2, 2, 2 };
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
        int year = (int)UnityEngine.Random.Range(startDate.Year, endDate.Year+1);
        int month = (int)UnityEngine.Random.Range(startDate.Month, endDate.Month+1);
        int day;
        if (month == startDate.Month)
        {
            day = UnityEngine.Random.Range(startDate.Day, startDate.LastDate+1);
        }
        else if(month==endDate.Month)
        {
            day= UnityEngine.Random.Range(1, endDate.Day + 1);
        }
        else
        {
            int tempLastDate = dateController.DaysOfMonth(year, month)+1;
            day = UnityEngine.Random.Range(1, tempLastDate);
        }
        Date tempDate = ScriptableObject.CreateInstance<Date>();
        tempDate = dateController.SetDate(year, month, day);
        return tempDate;
    }
    private void RandomDaysOff(int daysCount) ///for test only
    {
        List<Date> repeatedDate = new List<Date>();
        for(int i =0;i< CentralProcessor.ASSData.StoreStaffData.Length; i++)
        {
            List<Date> daysOffTemp = new List<Date>();
            Date date;
            for (int j = 0; j < daysCount; j++)
            {
                do
                {
                    date = RandomDate(CentralProcessor.ASSData.StartDate, CentralProcessor.ASSData.EndDate);
                } while (repeatedDate.IndexOf(date) != repeatedDate.LastIndexOf(date));
                daysOffTemp.Add(date);
                repeatedDate.Add(date);
            }
            CentralProcessor.ASSData.StoreStaffData[i].DaysOff = daysOffTemp;
        }
    }
}

