using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AutoShiftSystem : MonoBehaviour
{
    [SerializeField] ASSData assData;
    //設定並創建起始雙date
    //確認日期
    //設定並創建monthly
    //呼叫uictrler

    private void Awake()
    {
        UIController uIController = GetComponentInChildren<UIController>();
    }
    private void Start()
    {
        SetScriptableObject();
        SetStaff();
        
        StoreData storeData = assData.StoreData;
        Date startDate = assData.StartDate;
        Date endDate = assData.EndDate;
        if (!startDate.DateError && !endDate.DateError)
        {
            CentralProcessor.Instance.TheUIController.BuildDatePanel(storeData);
            CentralProcessor.Instance.TheUIController.BuildStaffPanel(storeData);
            CentralProcessor.Instance.TheUIController.BuildShiftPanel(storeData);
        }
        else
        {
            Debug.Log("日期設定錯誤");
        }
    }

    //暫時輸入區
    private void SetStaff()
    {
        List<StaffData> staffDatas = new List<StaffData>();
        List<string> staffName = new List<string>(){"吳玟頤","阿蛙","被被","伍佑群","臭臭" };
        for(int i = 0; i < 5; i++)
        {
            StaffData staffData = ScriptableObject.CreateInstance<StaffData>();
            staffData = CentralProcessor.Instance.StaffController.SetStaffInfo(staffName[i], i.ToString(), "員工");
            staffDatas.Add(staffData);
        }
        assData.StoreData.StoreStaffData = staffDatas;
    }
    private void SetScriptableObject()
    {

        assData.StartDate=CentralProcessor.Instance.DateController.SetDate(2023, 10, 29);
        assData.EndDate= CentralProcessor.Instance.DateController.SetDate(2023, 11, 25);
        assData.StoreData = CentralProcessor.Instance.ShiftController.SetMonthlyShiftDate(assData.StartDate, assData.EndDate);
    }
}

