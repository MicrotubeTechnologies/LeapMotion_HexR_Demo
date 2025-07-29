using UnityEngine;

public class BoxCapOpener : MonoBehaviour
{
    public Animator animator;
    public GameObject tv, GameBoy, computer, Phone;
    void Update()
    {

    }

    public void OpenCap()
    {
        //animator.SetTrigger("Open");
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
