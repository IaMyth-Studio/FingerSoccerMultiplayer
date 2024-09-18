using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputField : MonoBehaviour
{
    public TMP_InputField inputLogin;
    public TMP_InputField inputPass;
    public AudioClip soundValidate;

    //CHECK IF THE CHARACTERS IN THE INPUTFIELD LOGIN, IS GREATER THAN OR EQUAL TO 4
    void Update()
    {
        if(inputLogin.text.Length >= 4)//if characters length is superior or equal to 4
        {
            UIGlobal.instance.colorText.a = 1f;
            UIGlobal.instance.BtnNext.GetComponent<Button>().interactable = true;
            UIGlobal.instance.BtnNext.GetComponentInChildren<TextMeshProUGUI>().color = UIGlobal.instance.colorText;
        }
        else
        {
            UIGlobal.instance.colorText.a = 0.4f;
            UIGlobal.instance.BtnNext.GetComponent<Button>().interactable = false;
            UIGlobal.instance.BtnNext.GetComponentInChildren<TextMeshProUGUI>().color = UIGlobal.instance.colorText;
        }

        //CHECK IF THE CHARACTERS IN THE INPUTFIELD PASSWORD, IS GREATER THAN OR EQUAL TO 4
        if (inputPass.text.Length >= 4)//if characters length is superior or equal to 4
        {
            UIGlobal.instance.colorText.a = 1f;
            UIGlobal.instance.BtnValidate.GetComponent<Button>().interactable = true;
            UIGlobal.instance.BtnValidate.GetComponentInChildren<TextMeshProUGUI>().color = UIGlobal.instance.colorText;
        }
        else
        {
            UIGlobal.instance.colorText.a = 0.4f;
            UIGlobal.instance.BtnValidate.GetComponent<Button>().interactable = false;
            UIGlobal.instance.BtnValidate.GetComponentInChildren<TextMeshProUGUI>().color = UIGlobal.instance.colorText;
        }
    }

    public void ValidateNameInput()
    {
        UIGlobal.instance.MyName = inputLogin.text;
        UIGlobal.instance.ChangeTitleForPassword();
        inputLogin.text = "";
        UIGlobal.instance.PlayFXSound(soundValidate);
    }

    public void ValidatePassInput()
    {
        UIGlobal.instance.MyPass = inputPass.text;
        UIGlobal.instance.CloseInput();
        inputPass.text = "";
        UIGlobal.instance.PlayFXSound(soundValidate);
    }
}
