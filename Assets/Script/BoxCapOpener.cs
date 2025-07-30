using UnityEngine;

public class BoxCapOpener : MonoBehaviour
{
    public Animator animator;
    public GameObject RecordPlayer, tv, GameBoy, computer, Phone;
    void Update()
    {

    }
    public void TurnAllObjectOFF()
    {
        RecordPlayer.SetActive(false);
        tv.SetActive(false);
        computer.SetActive(false);
        GameBoy.SetActive(false);
        Phone.SetActive(false);
    }
    public void OpenCap()
    {
        //animator.SetTrigger("Open");
    }
    public void RecordP()
    {
        RecordPlayer.SetActive(true);
        //2010-2019
        animator.SetTrigger("RecordPlayer");
    }
    public void TV()
    {
        tv.SetActive(true);
        //1980-1989
        animator.SetTrigger("TV");
    }
    public void gameboy()
    {
        GameBoy.SetActive(true);
        //1990-1999
        animator.SetTrigger("Gameboy");
    }
    public void Computer()
    {
        computer.SetActive(true);
        //2000-2009
        animator.SetTrigger("Computer");
    }
    public void SmartPhone()
    {
        Phone.SetActive(true);
        //2010-2019
        animator.SetTrigger("SmartPhone");
    }


}
