﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private VisualElement rootVisualElement;
    void Awake()
    {
        rootVisualElement = GetComponentInParent<UIDocument>().rootVisualElement;
    }
    public void BuildDatePanel(ASSData assData)
    {
        VisualElement datesContainer = rootVisualElement.Q("DatesContainer");
        datesContainer.Clear();
        foreach (ShiftData shiftData in assData.MonthlyShiftData)
        {
            bool even=false;
            if (shiftData.Line % 2 == 0)
            {
                even = true;
            }
            datesContainer.Add(new UIPanel(shiftData,even));
        }
    }
    public void BuildStaffPanel(ASSData assData)
    {
        VisualElement staffContainer = rootVisualElement.Q("StaffsContainer");
        staffContainer.Clear();
        foreach(StaffData staffData in assData.StoreStaffData)
        {
            staffContainer.Add(new UIPanel(staffData));
        }
    }
    public void BuildShiftsPanel(ASSData assData)
    {
        VisualElement shiftsContainer = rootVisualElement.Q("MainShiftsContainer");
        shiftsContainer.Clear();
        for (int i = 0; i < assData.ShiftDuration; i++)
        {
            shiftsContainer.Add(new UIPanel(i));
        }
    }
    public void BuildShiftPanel(ASSData assData)
    {
        VisualElement shiftContainer;
        for(int i = 0; i < assData.ShiftDuration; i++)
        {
            shiftContainer = rootVisualElement.Query("ShiftsContainer").AtIndex(i);
            shiftContainer.Clear();
            for (int j = 0; j < assData.StoreStaffData.Length; j++)
            {
                //shiftContainer.Add(new UIPanel(assData.MonthlyShiftData[i].ShiftTimes[j]));//待刪
                shiftContainer.Add(new UIPanel(GetShiftText(assData.StoreStaffData[j], assData.MonthlyShiftData[i])));
            }
        }

    }

    private string GetShiftText(StaffData staff,ShiftData shift)
    {
        ASSController assController = CentralProcessor.Instance.ASSController;
        string shiftText= string.Empty;
        if (assController.CheckStaffDayOff(staff, shift.Date))
        {
            shiftText = "指休";
        }
        else if (assController.CheckShiftError(staff,shift))
        {
            shiftText = "異常";
        }
        else if (assController.GetTodayWorkHours(staff,shift)>0)
        {
            shiftText = assController.GetShiftText(staff, shift);
        }
        else if (assController.GetTodayWorkHours(staff,shift)==0)
        {
            shiftText = "排休";
        }
        else
        {
            shiftText = "空";
        }
        return shiftText;
    }

}
