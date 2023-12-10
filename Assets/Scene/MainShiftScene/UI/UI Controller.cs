using System.Collections;
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
    public void BuildDatePanel(StoreData storeData)
    {
        VisualElement datesContainer = rootVisualElement.Q("DatesContainer");
        datesContainer.Clear();
        foreach (ShiftData shiftData in storeData.MonthlyShiftData)
        {
            bool even=false;
            if (shiftData.Line % 2 == 0)
            {
                even = true;
            }
            datesContainer.Add(new UIPanel(shiftData,even));
        }
    }
    public void BuildStaffPanel(StoreData storeData)
    {
        VisualElement staffContainer = rootVisualElement.Q("StaffsContainer");
        staffContainer.Clear();
        foreach(StaffData staffData in storeData.StoreStaffData)
        {
            staffContainer.Add(new UIPanel(staffData));
        }
    }
    public void BuildShiftPanel(StoreData storeData)
    {
        VisualElement shiftsContainer = rootVisualElement.Q("MainShiftsContainer");
        shiftsContainer.Clear();
        for (int i = 0; i < storeData.ShiftDuration; i++)
        {
            VisualElement shiftContainer = new UIPanel(i);
            shiftContainer.Add(new UIPanel(storeData.MonthlyShiftData[i].ShiftTimes[1]));
            /*foreach (ShiftTimeData shiftTimeData in storeData.MonthlyShiftData[i].ShiftTimes)
            {
                shiftContainer.Add(new UIPanel(shiftTimeData));
            }*/
            shiftsContainer.Add(shiftContainer);

        }
    }
}
