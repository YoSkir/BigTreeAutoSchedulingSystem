using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Date_", menuName = "Date")]

public class Date : ScriptableObject
{
    private int year, month, day;
    private string weekDay;
    private bool dateError=true;

    public string DateString => $"{year}年{month}月{day}日 星期{weekDay}";
    public string WeekDay
    {
        get => weekDay;
        set
        {
            weekDay = value;
        }
    }
    public int Year
    {
        get => year;
        set
        {
            year = value;
        }
    }
    public int Month
    {
        get => month;
        set
        {
            month = value;
        }
    }
    public int Day
    {
        get => day;
        set
        {
            day = value;
        }
    }
    public bool DateError
    {
        get => dateError;
        set
        {
            dateError = value;
        }
    }
}

          