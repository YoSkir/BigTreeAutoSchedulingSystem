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
     * 班別6 8 10 12
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
            int staffsOnWork = CurrentStaffsOnWork(shiftData.WorkHour[time]);
            while (!ManagerOnWork(shiftData.WorkHour[time]) ||staffsOnWork < shiftData.WorkHour[time].Length)//Check if the staffs amount on shift match the request.
            {
                if (!ManagerOnWork(shiftData.WorkHour[time]))
                {
                    shiftData.WorkHour[time][]
                }
            }
        }
        return shiftData;
    }
    //每半小時檢查 :這時間沒有主管
    //true: 如果需求人員已滿= 讓最高優先下班者下班(需另做下班判斷) : addFirstPrio(主管)
    //false:如果需求人員已滿= null : addFirstPrio(一般) 
    private StaffData AddFirstPriorityStaff(StaffData[] staffDatas,bool pickManager)
    {       
        return staffDatas[0];
    }
    private StaffData[] SetExtraStaffOffShift(StaffData[] currentTimeStaffs)
    {

    }
    //比較當前上班中員工
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

