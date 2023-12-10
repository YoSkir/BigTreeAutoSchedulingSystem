using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMethodScene : MonoBehaviour
{
    [SerializeField] InputField startInput, endInput;
    [SerializeField] Text resultText;


    public void OnButtonClick()
    {
        //Debug.Log(startInput.textComponent.text);
        Date date1 = TransformInputToTheDate(startInput.textComponent.text);
        Date date2 = TransformInputToTheDate(endInput.textComponent.text);
        resultText.text=CentralProcessor.Instance.DateController.GetDaysCount(date1, date2).ToString();
        List<Date> test = new List<Date>();
        test = CentralProcessor.Instance.DateController.GetRangeDate(date1, date2);
        foreach (Date d in test) Debug.Log(d.DateString);
    }
    private Date TransformInputToTheDate(string input)
    {
        if (CheckInput(input))
        {
            string[] dateHolder = input.Split(" ");
            Date temp = CentralProcessor.Instance.DateController.SetDate(int.Parse(dateHolder[0]),
    int.Parse(dateHolder[1]),
    int.Parse(dateHolder[2]));
            return temp;
        }
        else
        {
            startInput.textComponent.text = "請確認輸入！";
            return null;
        }
    }
    private bool CheckInput(string input)
    {
        if(input.Length is<5 or >10 ||
            !input.Contains(" "))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
