using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DateController : MonoBehaviour
{
    private readonly List<int> bigMonth = new List<int>() { 1, 3, 5, 7, 8, 10, 12 };

    public string GetWeekDay(int year,int month,int day)
    {
        int k = year % 100, j = year / 100;
        int weekDay;
        weekDay = (day + (13 * (month + 1)) / 5 + k + k / 4 + j / 4 - 2 * j) % 7;
        switch (weekDay)
        {
            case 0:
                return "六";
            case 1:
                return "日";
            case 2:
                return "一";
            case 3:
                return "二";
            case 4:
                return "三";
            case 5:
                return "四";
            case 6:
                return "五";
            default:
                return "?";

        }
    }
    //可改為只記錄初始日期，這樣就不用實例化整個月的Date
    public List<Date> GetRangeDate(Date startDate, Date endDate)
    {
        if (GetLaterDate(startDate, endDate) == startDate)
        {
            Date temp = startDate;
            startDate = endDate;
            endDate = temp;
        }
        List<Date> rangeDates = new List<Date>() { startDate };
        int range = GetDaysCount(startDate, endDate);
        for(int i=0;i<range;i++)
        {
            Date date = AddOneDay(rangeDates[i]);
            rangeDates.Add(date);
        }
        return rangeDates;
    }
    public int GetDaysCount(Date date1, Date date2)
    {
        int daysCount = 0;
        if (GetLaterDate(date1, date2) == date1)
        {
            Date temp = date1;
            date1 = date2;
            date2 = temp;
        }
        for (int i = date1.Year; i < date2.Year; i++)
        {
            daysCount += IsLeapYear(i) ? 366 : 365;
        }
        for (int i = date1.Month > date2.Month ? date2.Month : date1.Month
            ; i < (i == date2.Month ? date1.Month : date2.Month);
            i += i == date2.Month ? -1 : 1)
        {
            int days;
            if (i == 2)
            {
                days = FebLastDay(date2.Year);
            }
            else
            {
                days = bigMonth.Contains(i) ? 31 : 30;
            }
            daysCount += date1.Month > date2.Month ? -days : days;
        }
        daysCount += date2.Day - date1.Day;
        return ++daysCount;
    }
    public Date SetDate(int year,int month,int day)
    {
        Date date=ScriptableObject.CreateInstance<Date>();
        if (CheckDateError(year, month, day))
        {
            date.DateError = true;
        }
        else
        {
            date.DateError = false;
            date.Year = year;
            date.Month = month;
            date.Day = day;
            date.WeekDay = GetWeekDay(year, month, day);
            date.LastDate = DaysOfMonth(date);
        }
        return date;
    }
    public bool ContainsDate(List<Date> dates,Date date)
    {
        foreach(Date d in dates)
        {
            if (SameDate(date, d)) return true;
        }
        return false;
    }
    
    //inside
    private bool SameDate(Date date1,Date date2)
    {
        if (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day)
        {
            return true;
        }
        return false;
    }
    private int DaysOfMonth(Date date)
    {
        int days;
        if (date.Month == 2)
        {
            days = IsLeapYear(date.Year) ? 29 : 28;
        }
        else
        {
            days = bigMonth.Contains(date.Month) ? 31 : 30;
        }
        return days;
    }

    public int DaysOfMonth(int year,int month)
    {
        int days;
        if (month == 2)
        {
            days = IsLeapYear(year) ? 29 : 28;
        }
        else
        {
            days = bigMonth.Contains(month) ? 31 : 30;
        }
        return days;
    }

    private Date AddOneDay(Date inputDate)
    {
        int year = inputDate.Year, month = inputDate.Month, day = inputDate.Day;

        day++;
        if (month == 2)
        {
            if (day > FebLastDay(year))
            {
                day = 1;
                month++;
            }
        }
        else if (bigMonth.Contains(month))
        {
            if (day > 31)
            {
                day = 1;
                if (month == 12)
                {
                    month = 1;
                    year++;
                }
                else
                {
                    month++;
                }
            }
        }
        else
        {
            if (day > 30)
            {
                day = 1;
                month++;
            }
        }
        return SetDate(year, month, day);
    }
    private bool IsLeapYear(int year)
    {
        if (year % 4 == 0 && year % 100 > 0 || year % 400 == 0 || year % 1000 == 0)
        {;
            return true;
        }
        return false;
    }
    private int FebLastDay(int year)
    {
        if (IsLeapYear(year))
        {
            return 29;
        }
        return 28;
    }
    private Date GetLaterDate(Date date1, Date date2)
    {
        if (date1.Year == date2.Year)
        {
            if (date1.Month == date2.Month)
            {
                if (date1.Day > date2.Day)
                {
                    return date1;
                }
                else
                {
                    return date2;
                }
            }
            else
            {
                if (date1.Month > date2.Month)
                {
                    return date1;
                }
                else
                {
                    return date2;
                }
            }
        }
        else
        {
            if (date1.Year > date2.Year)
            {
                return date1;
            }
            else
            {
                return date2;
            }
        }
    }
    private bool CheckDateError(int year, int month, int day)
    {
        if (month is < 1 or > 12 || day is < 1 or > 31 || year < 2023)
        {
            Debug.Log("時間輸入錯誤！");
            return true;
        }
        else
        {
            if (day > 30 && !bigMonth.Contains(month))
            {
                if (month == 2)
                {
                    if (day > FebLastDay(year))
                    {
                        Debug.Log("時間輸入錯誤 閏年錯誤");
                        return true;
                    }
                }
                if (day == 31 && !bigMonth.Contains(month))
                {
                    Debug.Log("時間輸入錯誤 大小月錯誤");
                    return true;
                }
            }
        }
        return false;
    }
    

}
