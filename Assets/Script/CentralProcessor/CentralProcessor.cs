using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralProcessor : MonoBehaviour
{
    DateController dateController;
    ASSController assController;
    StaffController staffController;
    ShiftController shiftController;
    SystemSetup systemSetup;
    [SerializeField]UIController theUIController;

    public static CentralProcessor Instance;
    public static ASSData ASSData;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        dateController = GetComponentInChildren<DateController>();
        assController = GetComponentInChildren<ASSController>();
        staffController = GetComponentInChildren<StaffController>();
        shiftController = GetComponentInChildren<ShiftController>();
        systemSetup = GetComponentInChildren<SystemSetup>();
        ASSData = ScriptableObject.CreateInstance<ASSData>();
    }

    public DateController DateController => dateController;
    public ASSController ASSController => assController;
    public UIController TheUIController => theUIController;
    public StaffController StaffController => staffController;
    public ShiftController ShiftController => shiftController;
    public SystemSetup SystemSetup => systemSetup;
    public void TimeShow(int time)
    {
        string timeText = "";
        timeText+=time<24?"上午":"下午";
        timeText += time / 2 + "點";
        timeText += time % 2 == 0 ? "" : "半";
        Debug.Log(timeText);
    }   
}
