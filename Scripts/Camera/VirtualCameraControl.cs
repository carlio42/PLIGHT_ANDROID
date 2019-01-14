using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

//Turn on this script untill i find solution for touch input directly from finger. link:-freelookcam, TouchControl, virtualCameraControl
namespace MyNetwork
{
	public class VirtualCameraControl : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler	
	{
		public FreeLookCam m_FreeLookCam;
		private RectTransform bgRect;
		public RectTransform JoyRect;
		public Vector3 inputVector;

		private void Start()
		{
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

				Horizontal ();
				Vertical ();
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
			m_FreeLookCam.X_axis = 0;
			m_FreeLookCam.Y_axis = 0;
			JoyRect.anchoredPosition = Vector3.zero;
		}
		public void Horizontal()
		{
			if (inputVector.x != 0)
				m_FreeLookCam.X_axis = inputVector.x * 1.0f;
			
				
		}

		public void Vertical()
		{
			if (inputVector.z != 0)
				m_FreeLookCam.Y_axis = inputVector.z * 0.5f;
			
				
		}
	}
}