using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

//This is the old backup Script for Left joystick for character Control
// we have used a new Script for CHaracter Control

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler 	
{

	private RectTransform bgRect;
	private RectTransform JoyRect;
	public Vector3 inputVector;

	private void Start()
	{
		//PlayerUserControl.Joystick = this;// Enable this when we use this script

		bgRect = GetComponent<RectTransform> ();
		JoyRect = transform.GetChild (0).GetComponent<RectTransform> ();
	}

	public virtual void OnDrag(PointerEventData ped)
	{
		Vector2 pos;
	
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgRect, ped.position, ped.pressEventCamera, out pos)) 
		{
			pos.x = (pos.x / bgRect.sizeDelta.x);						
			pos.y = (pos.y / JoyRect.sizeDelta.y);
	
			inputVector = new Vector3 (pos.x * 5, 0, pos.y);				
			inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
					
			JoyRect.anchoredPosition = new Vector3 (inputVector.x * (bgRect.sizeDelta.x / 4), inputVector.z * (bgRect.sizeDelta.y / 4));		
		}
		else
			return;			
		}
		
	public virtual void OnPointerDown(PointerEventData ped)
	{
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped)
	{
		inputVector = Vector3.zero;
		JoyRect.anchoredPosition = Vector3.zero;
	}
	public float Horizontal()
	{
		if (inputVector.x != 0) 
			return inputVector.x;
		else
			return Input.GetAxis ("Horizontal");
	}

	public float Vertical()
	{
		if (inputVector.z != 0) 
			return inputVector.z;
		else
			return Input.GetAxis ("Vertical");
	}
}