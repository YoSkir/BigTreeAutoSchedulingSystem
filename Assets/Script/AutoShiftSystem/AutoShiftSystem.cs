using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
     * 上班時間9 10 13.5 15.5
     * 班別6.5 8.5 11 13(含休息
     * 休息時間 10後1小時
       9   10  11  12  13  14  15  16  17  18  19  20  21 
    //{2,2,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,2}
    */

    //asscontroller需補上priority score的數值可視化界面
    private void Awake()
    {
        UIController uIController = GetComponentInChildren<UIController>();
    }
    private void Start()
    {
        ASSData assData = CentralProcessor.ASSData;
        CentralProcessor.Instance.SystemSetup.testSetup();
        Date startDate = assData.StartDate;
        Date endDate = assData.EndDate;
        if (!startDate.DateError && !endDate.DateError)
        {
            CentralProcessor.Instance.TheUIController.BuildDatePanel(assData);
            CentralProcessor.Instance.TheUIController.BuildStaffPanel(assData);
            CentralProcessor.Instance.TheUIController.BuildShiftsPanel(assData);
            CentralProcessor.Instance.TheUIController.BuildShiftPanel(assData);
        }
        else
        {
            Debug.Log("日期設定錯誤");
        }
    }

    public void StartAutoScheduling()
    {
        List<ShiftData> monthlyShift = CentralProcessor.ASSData.MonthlyShiftData;
        for(int i = 0; i < monthlyShift.Count; i++)
        {
            OneDayScheduling(monthlyShift[i]);
        }
    }
    private void OneDayScheduling(ShiftData shiftData) //undone3
    {       
        for(int time = 0; time < shiftData.WorkHour.Length; time++) //change to 9 10 13.5 15.5
            //8點檢查時，如未上班者優先度大於上班者，則回頭讓上班者於15.5下班，排入未上班者，反之延長最高優先者班表
        {
            int requireStaffsCounts = shiftData.WorkHour[time].Length;
            int staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
            StaffData staff;
            while (!ManagerOnWork(shiftData.WorkHour[time]) ||staffsOnWork < requireStaffsCounts)//Check if the staffs amount on shift match the request.
            {
                if (!ManagerOnWork(shiftData.WorkHour[time]))
                {
                    if (staffsOnWork < requireStaffsCounts)
                    {
                        //add manager
                        //if getFirstPriority==null,add one more require staff
                        staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), true); //
                        if (staff != null)
                        {
                            AddStaffToShift(staff, shiftData,time);                           
                        }
                        else
                        {
                            AddStaffRequirement(shiftData, time);
                            requireStaffsCounts++;
                            staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), false);
                            if (staff == null)
                            {
                                Debug.Log("無可用員工");
                                //替代方案
                                break;
                            }
                            else
                            {
                                AddStaffToShift(staff,shiftData,time);
                            }
                        }
                    }
                    else
                    {
                        //off a staff
                        //add manager
                    }
                }
                else
                {
                    if (staffsOnWork < requireStaffsCounts)
                    {
                        //add a staff
                    }
                }
            }
            //check off staff, refresh status, add to availible
        }
    }
    //每半小時檢查 :這時間沒有主管
    //true: 如果需求人員已滿= 讓最高優先下班者下班(需另做下班判斷) : addFirstPrio(主管)
    //    如無主管可上班:後段班表增加一名需求員工
    //等等問:指定早或晚班時 有可能放假嗎?
    //false:如果需求人員已滿= null : addFirstPrio() 
    //                                  如最高優先已在上班:延長班表
    //                                  如雙主管:排雙主管?避開?
    private void AddStaffToShift(StaffData staff,ShiftData shift,int startTimeIndex,int shiftHour_halfHourCount)
    {
        for(int i = 0; i < shiftHour_halfHourCount; i++)
        {
            for(int j = 0; j < shift.WorkHour[startTimeIndex + i].Length; j++)
            {
                if (shift.WorkHour[startTimeIndex + i][j] == null)
                {
                    shift.WorkHour[startTimeIndex + i][j] = staff;
                }
            }
        }
        if(shift.AvailibleStaff.Contains(staff))shift.AvailibleStaff.Remove(staff);
    }
    private void AddStaffToShift(StaffData staff, ShiftData shift, int startTimeIndex)
    {
        int shiftTime;
        switch (staff.ContinuousWorkHours)
        {
            case 0:shiftTime = 13;break;
            case 13:
            case 22:shiftTime = 4;break;
            case 17:shiftTime = 5;break;
            default:shiftTime = 0;break;
        }
        for (int i = 0; i < shiftTime; i++)
        {
            for (int j = 0; j < shift.WorkHour[startTimeIndex + i].Length; j++)
            {
                if (shift.WorkHour[startTimeIndex + i][j] == null)
                {
                    shift.WorkHour[startTimeIndex + i][j] = staff;
                }
            }
        }
        if (shift.AvailibleStaff.Contains(staff)) shift.AvailibleStaff.Remove(staff);
    }
    private void AddStaffRequirement(ShiftData shift,int startTimeIndex)
    {
        for(int i = 0; i < shift.WorkHour.Length - startTimeIndex; i++)
        {
            Queue<StaffData> temp = new Queue<StaffData>();
            foreach(StaffData staff in shift.WorkHour[startTimeIndex + i])
            {
                if(staff!=null) temp.Enqueue(staff);
            }
            shift.WorkHour[startTimeIndex + i] = new StaffData[shift.WorkHour[startTimeIndex + i].Length + 1];
            int staffIndex = 0;
            while(temp.Count > 0)
            {
                shift.WorkHour[startTimeIndex + i][0] = temp.Dequeue();
                staffIndex++;
            }
        }
    }
    private StaffData GetFirstPriorityStaff(StaffData[] todayStaffs ,bool pickManager) ///
    {
        ASSController aSSController = CentralProcessor.Instance.ASSController;
        todayStaffs = aSSController.GetStaffPriorityRate(todayStaffs);
        foreach(StaffData staff in todayStaffs)
        {

        }

        return todayStaffs[0];///S
    }
    //比較優先度時 假如優先者已在上班(continuousWorkHours>0) 則延長班別
    //也許先從6小時開始排比較好?
    //記得排進來要更新判斷數值
    //如果雙主管,確保第二個主管會到關班
    //如果沒有主管可以上班 增加一個員工
    private StaffData[] SetExtraStaffOffShift(StaffData[] currentTimeStaffs)
    {
        //remember to remove staff from shiftData.availibleStaffs
        return currentTimeStaffs;///undone4
    }
    //用6 8 10 12小時來判斷?用總時數判斷?用連續上班天判斷?
    //記得更改判斷數值
    private int CurrentStaffsOnWork(StaffData[] workingStaffs)
    {
        int staffCount=0;
        foreach(StaffData staffData in workingStaffs)
        {
            staffCount += staffData == null ? 0 : 1;
        }
        return staffCount;
    }
    private bool ManagerOnWork(StaffData[] workingStaffs)
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
}

