using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
            monthlyShift[i] = OneDayScheduling(monthlyShift[i]);
        }
    }
    private ShiftData OneDayScheduling(ShiftData shiftData)
    {
        for(int time = 0; time < shiftData.WorkHour.Length; time++) //Scheduling 0.5 hour by 0.5 hour
        {
            int requireStaffsCounts = shiftData.WorkHour[time].Length;
            int staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
            while (!ManagerOnWork(shiftData.WorkHour[time]) ||staffsOnWork < requireStaffsCounts)//Check if the staffs amount on shift match the request.
            {
                //比較優先度時 假如優先者已在上班(continuousWorkHours>0) 則延長班別
                if (!ManagerOnWork(shiftData.WorkHour[time]))
                {
                    if (staffsOnWork < requireStaffsCounts)
                    {
                        //add manager
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
        }
        return shiftData;
    }
    //每半小時檢查 :這時間沒有主管
    //true: 如果需求人員已滿= 讓最高優先下班者下班(需另做下班判斷) : addFirstPrio(主管)
    //false:如果需求人員已滿= null : addFirstPrio() 
    private StaffData AddFirstPriorityStaff(StaffData[] storeStaffs ,bool pickManager)
    {
        List<StaffData> staffPriorityRate = new List<StaffData>();
        if (pickManager)
        {
            for(int colum=0; colum < storeStaffs.Length; colum++)
            {
                if (storeStaffs[colum].IsManager)
                {

                }
            }
        }
        else
        {

        }
        return storeStaffs[0]; ///
    }
    //也許先從6小時開始排比較好?
    //記得排進來要更新判斷數值
    //如果雙主管,確保第二個主管會到關班
    //如果沒有主管可以上班 增加一個員工
    private List<StaffData> GetPriorityRate(StaffData[] storeStaff)
    {
        return new List<StaffData>();///
    }
    private StaffData[] SetExtraStaffOffShift(StaffData[] currentTimeStaffs)
    {
        return currentTimeStaffs;///
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

