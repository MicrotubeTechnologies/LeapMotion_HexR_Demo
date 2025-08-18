using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttons_actions : MonoBehaviour {

	public Animation anim_GameObject;

	   
	public void BButt_play_normal()
	{
	anim_GameObject.Play("slow");
	}
	
		public void BButt_play_fast()
	{
	anim_GameObject.Play("fast");
	}
		

    

		
		
}
