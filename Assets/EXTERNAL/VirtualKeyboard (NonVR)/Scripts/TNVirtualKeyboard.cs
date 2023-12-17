using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;
	
	
    // Start is called before the first frame update
    void Start()
    {
		HideVirtualKeyboard();
		
    }

    public void ChangeInputField(TMP_InputField field)
    {
	    targetText = field;
    }
    
    
	public void KeyPress(string k){
		words += k;
		targetText.text = words;	
	}
	
	public void Del(){
		words = words.Remove(words.Length - 1, 1);
		targetText.text = words;	
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);
	}
}
