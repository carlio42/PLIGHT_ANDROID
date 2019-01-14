using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickInteractionButton_2 : MonoBehaviour 
{
	//public RectTransform MiddlePanel;

	public UIPanelManager panelManager;

	[Header("RectTransforms That Are Enable After Button CLick")]
	public RectTransform[] ErectTransforms;	

	[Header("RectTransforms That Are Disable After Button CLick")]
	public RectTransform[] DrectTransforms;

	public bool isButtonClick = true;

	public void OnButtonClick()
	{
		if (isButtonClick)		//TURN OFF THE MENU
		{
			foreach (RectTransform rect in ErectTransforms) 
				rect.gameObject.SetActive (isButtonClick);

			foreach (RectTransform rect in DrectTransforms) 
				rect.gameObject.SetActive (!isButtonClick);
		}

	}
}
