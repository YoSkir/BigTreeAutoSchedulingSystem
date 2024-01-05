using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class AutoShiftSystem : MonoBehaviour
{
    //設定並創建起始雙date
    //確認日期
    //設定並創建monthly
    //呼叫uictrler

    /*
     * 任何小時至少要有一個主管
     * 如果沒主管，要兩個人補上
     * 開班到十點，晚八到關班固定兩人
     * 六日晚八到關班至少三人
     * 上班時間9 11 13.5 15.5
     * 
     * 
     * total workHour直接拉整個月的來算
     * total offday在每天最後判斷數值
     * 
     * 未來加入:避免隔夜連班
     * 
     * 優先度總是排全部，在挑人時才替除:1.放假的人 2.正上班者延長班表 3.未上班者排入 (以下班者為連續上班時數等於0但今日上班時數>0
     * 
     * 班別6.5 8.5 11 13(含休息
     * 休息時間 10後1小時
       9   10  11  12  13  14  15  16  17  18  19  20  21 
    //{2,2,2,2,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,2}
    */

    //asscontroller需補上priority score的數值可視化界面
    ASSData assData;
    ASSController assController;
    int testCount;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&testCount<assData.MonthlyShiftData.Count)
        {
            testAutoScheduling();
        }
    }
    private void Awake()
    {
        UIController uIController = GetComponentInChildren<UIController>();
    }
    private void Start()
    {
        assData = CentralProcessor.ASSData;
        assController = CentralProcessor.Instance.ASSController;
        CentralProcessor.Instance.SystemSetup.testSetup();
        Date startDate = assData.StartDate;
        Date endDate = assData.EndDate;
        if (!startDate.DateError && !endDate.DateError)
        {
            CentralProcessor.Instance.TheUIController.BuildDatePanel(assData);
            CentralProcessor.Instance.TheUIController.BuildStaffPanel(assData);
            CentralProcessor.Instance.TheUIController.BuildShiftsPanel(assData);
            //CentralProcessor.Instance.TheUIController.BuildShiftPanel(assData);
        }
        else
        {
            Debug.Log("日期設定錯誤");
        }
        testCount = 0;
        /*StartAutoScheduling();
        CentralProcessor.Instance.TheUIController.BuildShiftPanel(assData);*/
    }
    public void testAutoScheduling()
    {
        List<ShiftData> monthlyShift = assData.MonthlyShiftData;
        OneDaySchedulingNoManagerOK(monthlyShift[testCount]);
        CentralProcessor.Instance.TheUIController.BuildShiftPanel(assData);
        testCount++;
    }
    public void StartAutoScheduling()
    {
        List<ShiftData> monthlyShift = CentralProcessor.ASSData.MonthlyShiftData;
        for(int i = 0; i < monthlyShift.Count; i++)
        {
            if (assData.NoManagerOK)
            {
                OneDaySchedulingNoManagerOK(monthlyShift[i]);
            }
            else
            {
                OneDayScheduling(monthlyShift[i]);
            }
            CentralProcessor.Instance.StaffController.CountTotalDayOff();
        }
    }
    private void testPrintAvailibleStaff(ShiftData shift)
    {
        CentralProcessor.Instance.StaffController.CountTotalWorkHours();
        string level="";
        foreach (StaffData staff in shift.AvailibleStaff)
        {
            level = staff.IsManager ? "主管" : "員工";
            Debug.Log(level+"名:" + staff.StaffName + "總時數:"+staff.TotalWorkHours);
        }
    }
    private void OneDaySchedulingNoManagerOK(ShiftData shift)
    {
        CentralProcessor.Instance.ShiftController.SetShiftAvailibleStaff(shift);
        for(int time = 0; time < shift.WorkHour.Length; time++)
        {
            int requireStaffsCounts = shift.RequireStaffs[time];
            StaffData staff;
            while (CurrentStaffsOnWork(shift.WorkHour[time]) < requireStaffsCounts)
            {
                staff = GetFirstPriorityStaff(shift, time);
                if (staff == null)
                {
                    Debug.Log("無可用員工");
                    //將已下班員工家長工時 等等
                    break;
                }
                else
                {
                    AddStaffToShift(staff, shift, time);
                }
            }
            CentralProcessor.Instance.StaffController.CountTotalWorkHours();
        }
    }
    private void OneDayScheduling(ShiftData shiftData)
    {
        CentralProcessor.Instance.ShiftController.SetShiftAvailibleStaff(shiftData);
        for (int time = 0; time < shiftData.WorkHour.Length; time++)
        {
            int requireStaffsCounts = shiftData.RequireStaffs[time];
            StaffData staff;
            //deal with manager first
            if (!CheckManagerAvailible(shiftData, time))
            {
                Debug.Log("無主管可用 新增需求員工 時間:" + (9 + time / 2) + "點");
                requireStaffsCounts++;
            }
            else if (!ManagerOnWork(shiftData.WorkHour[time]))
            {
                AddStaffToShift(GetFirstPriorityStaff(shiftData, time, true), shiftData, time);
            }
            //after manager
            while (CurrentStaffsOnWork(shiftData.WorkHour[time])<requireStaffsCounts)
            {
                staff=GetFirstPriorityStaff(shiftData,time,false);
                if (staff == null)
                {
                    Debug.Log("無可用員工");
                    break;
                }
                else
                {
                    AddStaffToShift(staff,shiftData,time);
                }
            }
            CentralProcessor.Instance.StaffController.CountTotalWorkHours();
        }
    }
    private void tempCode()
    {
        /*
        while ((!ManagerOnWork(shiftData.WorkHour[time]) && !noManagerAvailible) || staffsOnWork < requireStaffsCounts)//Check if the staffs amount on shift match the request.
        {
            if (!ManagerOnWork(shiftData.WorkHour[time]) && !noManagerAvailible)//當目前沒有主管或無可上班主管
            {
                if (staffsOnWork < requireStaffsCounts)//當上班人數小於需求人數
                {
                    staff = GetFirstPriorityStaff(shiftData, time, true);
                    if (staff != null)
                    {
                        AddStaffToShift(staff, shiftData, time);
                    }
                    else
                    {
                        noManagerAvailible = true;
                        AddStaffRequirement(shiftData, time);
                        requireStaffsCounts++;
                        staff = GetFirstPriorityStaff(shiftData, time, false);
                        if (staff == null)
                        {
                            Debug.Log("無可用員工");
                            //替代方案
                            break;
                        }
                        else
                        {
                            AddStaffToShift(staff, shiftData, time);
                        }
                    }
                }
                else//當上班人數等於需求人數
                {
                    staff = GetFirstPriorityStaff(shiftData, time, true);
                    if (staff == null)
                    {
                        AddStaffRequirement(shiftData, time);
                        requireStaffsCounts++;
                        noManagerAvailible = true;
                        staff = GetFirstPriorityStaff(shiftData, time, false);
                        if (staff == null)
                        {
                            Debug.Log(""); break;
                        }
                        AddStaffToShift(staff, shiftData, time);
                    }
                    else
                    {
                        SetExtraStaffOffShift(shiftData, time);
                        AddStaffToShift(staff, shiftData, time);
                    }
                }
            }
            else//有主管或無可上班主管
            {
                Debug.Log("目前需要員工");
                if (staffsOnWork < requireStaffsCounts)
                {
                    staff = GetFirstPriorityStaff(shiftData, time, false);
                    if (staff == null)
                    {
                        staff = GetFirstPriorityStaff(shiftData, time, true);
                    }
                    if (staff == null)
                    {
                        noManagerAvailible = true;
                        Debug.Log("無人可排"); break;
                    }
                    AddStaffToShift(staff, shiftData, time);
                }
            }
            requireStaffsCounts = shiftData.RequireStaffs[time];
            staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
        }
        */
    }


    private void CountStaffStatus(ShiftData shift)///
    {
        //ContinuousDayOff, ContinuousWorkDays,TotalDaysOff,TotalWorkHours, ContinuousOffHours, ContinuousWorkHours
        //count every staff total hour  ,if==0 add one day off
        //continuous off hour => 26,continuous day off++ workday=0, else continuous work day++ dayoff=0
        foreach (List<StaffData> workingHour in shift.WorkHour)
        {
            foreach(StaffData staff in workingHour)
            {
                staff.TotalWorkHours++;
                
            }
        }
    }
    private void AddStaffToShift(StaffData staff,ShiftData shift,int startTimeIndex,int shiftHour_halfHourCount)
    {
        for(int i = 0; i < shiftHour_halfHourCount; i++)
        {
            shift.WorkHour[startTimeIndex+i].Add(staff);
        }
    }
    private void AddStaffToShift(StaffData staff, ShiftData shift, int startTimeIndex)
    {
        int shiftTime;
        switch (GetContinuousWorkHours(staff,shift,startTimeIndex))
        {
            case 0:shiftTime = 13;break; 
                //未來可加入 這個CASE追加判斷此員工是已下班，則SHIFT TIME=4(5?)，並多呼叫一次AddStaffToShift(staff,shift,求上個上班時間段+1,startTimeindex-lastTimeIndex)
            case 13:
            case 22:shiftTime = 4;break;
            case 17:shiftTime = 5;break;
            default:shiftTime = 0;break;
        }
        //
        Debug.Log("增加員工:" + staff.StaffName+" "+shiftTime+"時間");
        //
        for (int i = 0; i < shiftTime; i++)
        {
            shift.WorkHour[startTimeIndex+i].Add(staff);
        }
    }
    private int GetContinuousWorkHours(StaffData staff,ShiftData shift,int timeIndex) //currently only find last time index
    {
        int continuousWorkHours = 0;
        while (timeIndex > 0)
        {
            timeIndex--;
            if (shift.WorkHour[timeIndex].Contains(staff))
            {
                continuousWorkHours++;
            }
            else
            {
                break;
            }
        }
        return continuousWorkHours;
    }
    private void AddStaffRequirement123(ShiftData shift,int startTimeIndex)
    {
        Debug.Log($"測試無主管時增加需求人數功能，如無問題請刪除此測試，日期 {shift.Date.DateString} 時間索引 {startTimeIndex}");
        int endTimeIndex = 0;
        switch (startTimeIndex)
        {
            case 0:endTimeIndex=4; break;
            case 4:endTimeIndex=9; break;
            case 9:endTimeIndex=13; break;
            case 13:endTimeIndex = shift.RequireStaffs.Count(); break;
            default:break;
        }
        for(int i=startTimeIndex;i<shift.RequireStaffs.Count();i++)
        {
            Debug.Log($"增加前人數 {shift.RequireStaffs[i]} i: {i} ");
            shift.RequireStaffs[i]++;
            Debug.Log($"增加後人數 {shift.RequireStaffs[i]} i: {i} ");
        }
        Debug.Log("ya");
    }
    private StaffData GetSecondPriorityStaff(ShiftData shift,int timeIndex)
    {
        shift.SecondAvailibleStaff = assController.GetStaffPriorityRate(shift.SecondAvailibleStaff.ToArray()).ToList();
        foreach(StaffData staff in shift.SecondAvailibleStaff)
        {
            if (StaffAvailibleCheck(staff, shift, timeIndex)) return staff;
        }
        return null;
    }
    private StaffData GetFirstPriorityStaff(ShiftData shift, int timeIndex)
    {
        shift.AvailibleStaff = assController.GetStaffPriorityRate(shift.AvailibleStaff.ToArray()).ToList();
        foreach(StaffData staff in shift.AvailibleStaff)
        {
            if (StaffAvailibleCheck(staff, shift, timeIndex)) return staff;
        }
        Debug.Log("第一列表無可用員工");
        return GetSecondPriorityStaff(shift, timeIndex);
    }
    private StaffData GetFirstPriorityStaff(ShiftData shift ,int timeIndex,bool pickManager) 
    {
        shift.AvailibleStaff = assController.GetStaffPriorityRate(shift.AvailibleStaff.ToArray()).ToList();
        foreach(StaffData staff in shift.AvailibleStaff)
        {
            if (pickManager==staff.IsManager)
            {
                if (StaffAvailibleCheck(staff, shift, timeIndex)) return staff;
            }
        }
        Debug.Log("第一列表無可用員工");
        return GetSecondPriorityStaff(shift, timeIndex);
    }
    private bool StaffAvailibleCheck(StaffData staff,ShiftData shift,int timeIndex)
    {
        if (!shift.WorkHour[timeIndex].Contains(staff))
        {
            if (assController.GetTodayWorkHours(staff, shift) > 0 && GetContinuousWorkHours(staff, shift, timeIndex) == 0)
            //今日有時數但上一時間沒上班代表已下班，不作為優先排入者
            //(未來可能將上班名單改成list，就算遇到已下班者為最高優先也可以往前拉長他的班表，避免無人可排入
            {
                Debug.Log("此員工已下班!");
                return false;
            }
            else if (timeIndex > 13)//三點半後不新增未上班員工，只延長班表
            {
                if (GetContinuousWorkHours(staff, shift, timeIndex) > 0)
                {
                    return true;
                }
            }
            else
            {
                if (timeIndex != 0 || !staff.LastDayCloseShift)//開班沒遇到昨天關班
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void SetExtraStaffOffShift(ShiftData shift,int timeIndex)
    {
        ASSController aSSController=CentralProcessor.Instance.ASSController;
        shift.WorkHour[timeIndex] = aSSController.GetStaffPriorityRate(shift.WorkHour[timeIndex].ToArray()).ToList();
        int lastIndex = shift.WorkHour[timeIndex].Count - 1;
        StaffData staffToOff = shift.WorkHour[timeIndex][lastIndex];
        foreach (List<StaffData> time in shift.WorkHour)
        {
            time.Remove(staffToOff);
        }
    }
    private int CurrentStaffsOnWork(List<StaffData> workingStaffs)
    {
        return workingStaffs.Count();
    }
    private bool ManagerOnWork(List<StaffData> workingStaffs)
    {
        foreach(StaffData staffData in workingStaffs)
        {
            if (staffData.IsManager)
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckManagerAvailible(ShiftData shift,int timeindex)
    {
        foreach(StaffData staff in shift.AvailibleStaff)
        {
            if(staff.IsManager)
            {
                if (timeindex !=0 || !staff.LastDayCloseShift)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

