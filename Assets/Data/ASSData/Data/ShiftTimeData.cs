using UnityEngine;
using System.Collections;

public class ShiftTimeData : ScriptableObject 
{
    public enum ShiftStatusEnum { 特休,指休,排休,早班,晚班,全班};
    public enum ShiftStartTime { 九點,十點,一點半,三點半};
    public enum ShiftType { 六小,八小,十小,十二小};
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
        }
    }
    public double ShiftDuration
    {
        get => shiftDuration;
        set
        {
            shiftDuration = value;
            breakTime = shiftDuration > 8 ? 1 : 0.5;
        }
    }
    public string ShiftTimeText
    {
        get => shiftTimeText;
        set
        {
            shiftTimeText = value;
        }
    }
    public double BreakTime => breakTime;
    public string ShiftStatus => shiftStatus;
}

