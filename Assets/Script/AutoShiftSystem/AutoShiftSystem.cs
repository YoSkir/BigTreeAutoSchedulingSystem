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
     * 班別6.5 8.5 11 13(含休息
     * 休息時間 10後1小時
       9   10  11  12  13  14  15  16  17  18  19  20  21 
    //{2,2,2,2,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,2}
    */

    //asscontroller需補上priority score的數值可視化界面
    ASSData assData;
    private void Awake()
    {
        UIController uIController = GetComponentInChildren<UIController>();
    }
    private void Start()
    {
        assData = CentralProcessor.ASSData;
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
            //total day off
        }
    }
    //
    private void OneDayScheduling(ShiftData shiftData) //undone3
    {       
        //先只用TOTAL作為判斷依據
        for(int time = 0; time < shiftData.WorkHour.Length; time++) //change to 9 11 13.5 15.5
            //8點檢查時，如未上班者優先度大於上班者，則回頭讓上班者於15.5下班，排入未上班者，反之延長最高優先者班表
        {
            int requireStaffsCounts = shiftData.WorkHour[time].Length;
            int staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
            StaffData staff;
            bool noManagerAvailible = false;
            while ((!ManagerOnWork(shiftData.WorkHour[time])&&!noManagerAvailible) ||staffsOnWork < requireStaffsCounts)//Check if the staffs amount on shift match the request.
            {
                if (!ManagerOnWork(shiftData.WorkHour[time]))
                {
                    if (staffsOnWork < requireStaffsCounts)
                    {
                        staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), true); 
                        if (staff != null)
                        {
                            AddStaffToShift(staff, shiftData,time);                           
                        }
                        else
                        {
                            AddStaffRequirement(shiftData, time);
                            staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), false);
                            if (staff == null)
                            {
                                Debug.Log("無可用員工");
                                noManagerAvailible = true;
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
                        staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), true);
                        if (staff == null)
                        {
                            AddStaffRequirement(shiftData, time);
                            noManagerAvailible= true;
                            staff=GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(),false);
                            if (staff == null)
                            {
                                Debug.Log("");break;
                            }
                            AddStaffToShift(staff, shiftData, time);
                        }
                        else
                        {
                            SetExtraStaffOffShift(shiftData, time);//
                            AddStaffToShift(staff,shiftData,time);
                        }
                    }
                }
                else
                {
                    if (staffsOnWork < requireStaffsCounts)
                    {
                        staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), false);
                        if (staff == null)
                        {
                            staff = GetFirstPriorityStaff(shiftData.AvailibleStaff.ToArray(), true);
                            noManagerAvailible = true;
                        }
                        if (staff == null)
                        {
                            Debug.Log("無人可排"); break;
                        }
                        AddStaffToShift(staff,shiftData,time);
                    }
                }
                requireStaffsCounts = shiftData.WorkHour[time].Length;
                staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
            }
            CentralProcessor.Instance.StaffController.CountTotalWorkHours();
            CountStaffStatus(shiftData); ///
            //check off staff, add to availible, 
        }
        //last day off shift set
    }
    //每半小時檢查 :這時間沒有主管
    //true: 如果需求人員已滿= 讓最高優先下班者下班(需另做下班判斷) : addFirstPrio(主管)
    //    如無主管可上班:後段班表增加一名需求員工
    //等等問:指定早或晚班時只會6.5HR 極端情況下有可能放假
    //false:如果需求人員已滿= null : addFirstPrio() 
    //                                  如最高優先已在上班:延長班表
    //                                  如雙主管:盡可能避開
    private void CountStaffStatus(ShiftData shift)///
    {
        //ContinuousDayOff, ContinuousWorkDays,TotalDaysOff,TotalWorkHours, ContinuousOffHours, ContinuousWorkHours
        //count every staff total hour  ,if==0 add one day off
        //continuous off hour => 26,continuous day off++ workday=0, else continuous work day++ dayoff=0
        foreach (StaffData[] workingHour in shift.WorkHour)
        {
            foreach(StaffData staff in workingHour)
            {
                staff.TotalWorkHours++;
                
            }
        }
    }
    private void CountStaffContinuousHour(StaffData[] workHour) ///
    {
        foreach(StaffData storeStaff in assData.StoreStaffData)
        {
            if (workHour.Contains(storeStaff))
            {

            }
            else
            {

            }
        }
    }
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
    private StaffData GetFirstPriorityStaff(StaffData[] availibleStaffs ,bool pickManager) 
    {
        ASSController aSSController = CentralProcessor.Instance.ASSController;
        availibleStaffs = aSSController.GetStaffPriorityRate(availibleStaffs);
        foreach(StaffData staff in availibleStaffs)
        {
            if (pickManager)
            {
                if (staff.IsManager) return staff;
            }
            else
            {
                if(!staff.IsManager) return staff;
            }
        }
        return null;
    }
    private void SetExtraStaffOffShift(ShiftData shift,int timeIndex)///
    {
        ASSController aSSController=CentralProcessor.Instance.ASSController;
        shift.WorkHour[timeIndex] = aSSController.GetStaffPriorityRate(shift.WorkHour[timeIndex]);

    }
    //用6 8 10 12小時來判斷?用總時數判斷?用連續上班天判斷?
    //記得更改判斷數值
    //先減班為主
    //如都一樣就以最低優先員工為主
    //已上班員工中正在上班時數等於13(6.5hr)直接整個拔掉 換為需求的人，因為6.5不能再減少，其他人則以減少班時為主
    private int GetCurrentWorkHourToday(StaffData staff,ShiftData shift)
    {
        int workHour = 0;
        foreach (StaffData[] workingHour in shift.WorkHour)
        {
            if (workingHour.Contains(staff))
            {
                workHour++;
            }
        }
        return workHour;
    }
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

