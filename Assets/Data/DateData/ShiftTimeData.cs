using UnityEngine;
using System.Collections;

public class ShiftTimeData : ScriptableObject 
{
    public enum ShiftStatusEnum { 特休,指休,排休,早班,晚班,全班};
    double startTime, endTime,shiftDuration, breakTime;
    string shiftTimeText,shiftStatus;

    public double StartTime
    {
        get => startTime;
        set
        {
            startTime = value;
        }
    }
    public double EndTime
    {
        get => endTime;
        set
        {
            endTime = value;
            shiftDuration = endTime - startTime;
            breakTime = shiftDuration > 8 ? 1 : 0.5;
        }
    }
    public double ShiftDuration => shiftDuration;
    public double BreakTime => breakTime;
    public string ShiftStatus => shiftStatus;
}

