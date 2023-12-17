using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class vkEnabler : MonoBehaviour
{
	[SerializeField] private TNVirtualKeyboard Keyboard;
	
	public void ShowVirtualKeyboard(){
		Keyboard.ShowVirtualKeyboard();
		Keyboard.targetText = gameObject.GetComponent<TMP_InputField>();
	}
}
