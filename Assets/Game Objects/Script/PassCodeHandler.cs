using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PassCodeHandler : MonoBehaviour
{
    public TextMeshPro KeyCode;
    public BoxCapOpener boxCapOpener;
    private string Password = "";

    // Called by UI buttons for digits
    public void OnePress() { AddDigit("1"); }
    public void TwoPress() { AddDigit("2"); }
    public void ThreePress() { AddDigit("3"); }
    public void FourPress() { AddDigit("4"); }
    public void FivePress() { AddDigit("5"); }
    public void SixPress() { AddDigit("6"); }
    public void SevenPress() { AddDigit("7"); }
    public void EightPress() { AddDigit("8"); }
    public void NinePress() { AddDigit("9"); }
    public void ZeroPress() { AddDigit("0"); }

    // Helper function to add digit to password
    private void AddDigit(string digit)
    {
        if (Password.Length < 4)
        {
            Password += digit;
            UpdateDisplay();
        }
    }

    // Remove last character from password
    public void BackSpace()
    {
        if (Password.Length > 0)
        {
            Password = Password.Substring(0, Password.Length - 1);
            UpdateDisplay();
        }
    }

    // Called when Enter button is pressed
    public void Enter()
    {
        Debug.Log("Entered Password: " + Password);

        if (Password.Length == 4)
        {


            if (int.TryParse(Password, out int year))
            {
                boxCapOpener.TurnAllObjectOFF();
                if (year <= 1969)
                {
                    Debug.Log("Special year: 1969");
                    // Do something
                }
                else if (year >= 1970 && year <= 1980)
                {
                    boxCapOpener.RecordP();
                    // Do something
                }
                else if (year >= 1980 && year <= 1989)
                {
                    boxCapOpener.TV();
                    // Do something
                }
                else if (year >= 1991 && year <= 2000)
                {
                    boxCapOpener.gameboy();
                    // Do something
                }
                else if (year >= 2000 && year <= 2009)
                {
                    boxCapOpener.Computer();
                    // Do something
                }
                else if (year >= 2010 && year <= 2025)
                {
                    boxCapOpener.SmartPhone();
                    // Do something
                }
                else if (year >= 2026)
                {
                    Debug.Log("After 2020");
                    // Do something
                }

            }
            else
            {
                Debug.Log("Invalid password: not a number.");
            }
        }
        else
        {
            Debug.Log("Password must be exactly 4 digits.");
        }
    }

    // Updates the on-screen text
    private void UpdateDisplay()
    {
        KeyCode.text = Password;
    }
}
