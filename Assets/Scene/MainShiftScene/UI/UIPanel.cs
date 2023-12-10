using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPanel : VisualElement
{
    readonly TemplateContainer templateContainer;

    public new class UxmlFactory : UxmlFactory<UIPanel> { }
    public UIPanel()
    {
    }
    public UIPanel(ShiftData dailyShiftData,bool even) : this()
    {
        templateContainer = Resources.Load<VisualTreeAsset>(path: "DateContainer").Instantiate();
        hierarchy.Add(templateContainer);
        userData = dailyShiftData;
        templateContainer.Q<Label>(name: "CalenderLabel").text = dailyShiftData.Calender;
        if(even)
        templateContainer.Q<Label>(name: "CalenderLabel")
                .style.backgroundColor = new Color(0.3679702f, 0.7169812f, 0.3348167f, 0.5176471f);
        templateContainer.Q<Label>(name: "DateLabel").text = dailyShiftData.Date.Day.ToString();
        templateContainer.Q<Label>(name: "WeekDayLabel").text = dailyShiftData.Date.WeekDay; 
    }
    public UIPanel(StaffData staffData) : this()
    {
        templateContainer = Resources.Load<VisualTreeAsset>(path: "StaffContainer").Instantiate();
        templateContainer.style.flexGrow = 1.0f;
        hierarchy.Add(templateContainer);
        userData = staffData;
        templateContainer.Q<Label>(name: "StaffNameLabel").text = staffData.StaffName;
        templateContainer.Q<Label>(name: "StaffLevelLabel").text = staffData.StaffLevel;
        templateContainer.Q<Label>(name: "StaffNumberLabel").text = staffData.StaffNumber;
    }
    public UIPanel(ShiftTimeData shiftTimeData) : this()
    {
        templateContainer = Resources.Load<VisualTreeAsset>(path: "ShiftContainer").Instantiate();
        hierarchy.Add(templateContainer);
        userData = shiftTimeData;
        templateContainer.Q<Label>(name: "ShiftLabel").text = "test";
    }
    public UIPanel(int line) : this()
    {
        templateContainer = Resources.Load<VisualTreeAsset>(path: "ShiftsContainer").Instantiate();
        if (line % 2 == 0)
        {
            templateContainer.style.backgroundColor = new Color(0.3679702f, 0.7169812f, 0.3348167f, 0.5176471f);
        }
        hierarchy.Add(templateContainer);
    }
}
