using System
    .Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
   public GameObject SceneOne, SceneTwo, SceneThree;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SceneToggle()
    {
        if (SceneOne.activeInHierarchy)
        {
            SceneOne.SetActive(false);
            SceneTwo.SetActive(true);
        }
        else if (SceneTwo.activeInHierarchy)
        {
            SceneTwo.SetActive(false);
            SceneThree.SetActive(true);
        }
        else if (SceneThree.activeInHierarchy)
        {
            SceneThree.SetActive(false);
            SceneOne.SetActive(true);
        }
    }
}
