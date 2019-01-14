using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickInteractionButton : MonoBehaviour	//SCRIPT FOR :- ATTATCH TO BUTTON WHICH CONTROL OTHER GAMEOBJECT
{
	public RectTransform MiddlePanel;

	public UIPanelManager panelManager;

	[Header("RectTransforms That Are Enable After Button CLick")]
	public RectTransform[] ErectTransforms;	

	[Header("RectTransforms That Are Disable After Button CLick")]
	public RectTransform[] DrectTransforms;

	public bool isButtonClick = true;

	void OnEnable()
	{
		isButtonClick = true;
	}
	void OnDisable()
	{
		isButtonClick = false;
	}
	public void OnButtonClick()
	{
		if (!isButtonClick)		//TURN OFF THE MENU
		{
			foreach (RectTransform rect in ErectTransforms) 
				rect.gameObject.SetActive (isButtonClick);

			foreach (RectTransform rect in DrectTransforms) 
				rect.gameObject.SetActive (isButtonClick);


			//panelManager.currentPanel = panelManager.previousPanel;
			//panelManager.previousPanel = MiddlePanel;
			panelManager.ChangePanel(panelManager.exitPanel);

			isButtonClick = !isButtonClick;
			return;
		}

		if (isButtonClick) 		//TURN ON THE MENU
		{
			foreach (RectTransform rect in ErectTransforms) 
				rect.gameObject.SetActive (isButtonClick);

			foreach (RectTransform rect in DrectTransforms) 
				rect.gameObject.SetActive (!isButtonClick);

			//panelManager.previousPanel = panelManager.currentPanel;
			//panelManager.currentPanel = MiddlePanel;
			panelManager.ChangePanel(MiddlePanel);

			isButtonClick = !isButtonClick;
		}

	}

}
