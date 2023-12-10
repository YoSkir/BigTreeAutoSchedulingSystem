using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralProcessor : MonoBehaviour
{
    DateController dateController;
    ShiftController shiftController;
    StaffController staffController;
    [SerializeField]UIController theUIController;

    public static CentralProcessor Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
        dateController = GetComponentInChildren<DateController>();
        shiftController = GetComponentInChildren<ShiftController>();
        staffController = GetComponentInChildren<StaffController>();
    }

    public DateController DateController => dateController;
    public ShiftController ShiftController => shiftController;
    public UIController TheUIController => theUIController;
    public StaffController StaffController => staffController;
}
