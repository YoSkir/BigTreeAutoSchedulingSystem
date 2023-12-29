using UnityEngine;
using System.Collections;

public class ShiftTimeData : ScriptableObject 
{
    //待刪除
    public enum ShiftStatusEnum { 特休,指休,排休,早班,晚班,全班};
    public enum ShiftStartTime { 九點,十點,一點半,三點半};
    public enum ShiftType { 六小,八小,十小,十二小};
    int startTime, endTime,shiftDuration, breakTime;
    string shiftTimeText,shiftStatus;
    StaffData thisStaff;

    public StaffData ThisStaff { get => thisStaff; set => thisStaff = value; }
    public int StartTime
    {
        get => startTime;
        set
        {
            startTime = value;
        }
    }
    public int EndTime
    {
        get => endTime;
        set
        {
            endTime = value;
        }
    }
    public int ShiftDuration
    {
        get => shiftDuration;
        set
        {
            shiftDuration = value;
            breakTime = shiftDuration > 16 ? 2 : 1;
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
    public int BreakTime => breakTime;
    public string ShiftStatus => shiftStatus;
}

