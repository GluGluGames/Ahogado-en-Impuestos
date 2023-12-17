using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vkKey : MonoBehaviour
{
	[SerializeField] private TNVirtualKeyboard Keyboard;
	public string k = "xyz";
	
	public void KeyClick(){
		Keyboard.KeyPress(k);
	}
}
