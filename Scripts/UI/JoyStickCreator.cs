using UnityEngine;
using UnityEngine.EventSystems;
//Remove this script untill i find a solution for direct touch input by finger. link:-freelookcam, TouchControl, virtualCameraControl
namespace MyNetwork
{
	public class JoyStickCreator : MonoBehaviour//, IPointerDownHandler
	{
		public VirtualCameraControl m_CameraJoyStick;
		public FreeLookCam m_FreeLookCam;
		GameObject[] rt;
		public RectTransform m_RightDPadJoystick;
		//public RectTransform m_InstantiatePanel;

		void Update()
		{
			foreach (Touch touch in Input.touches) 
			{
				if (touch.phase == TouchPhase.Began)
					Debug.Log ("Began");
				else if (touch.phase == TouchPhase.Canceled)
					Debug.Log ("Canceled");
				else if (touch.phase == TouchPhase.Ended)
					Debug.Log ("Ended");
				else if (touch.phase == TouchPhase.Moved)
					Debug.Log ("Moved");
				else if (touch.phase == TouchPhase.Stationary)
					Debug.Log ("Stationary");
				

			}
		}

		/*public virtual void OnPointerDown(PointerEventData ped)
		{
			Vector3 touchPosition;
			touchPosition = Input.mousePosition;

			m_RightDPadJoystick.position = touchPosition;

			m_CameraJoyStick.OnPointerDown (ped);
			m_CameraJoyStick.inputVector = Vector3.zero;
			m_FreeLookCam.X_axis = 0;
			m_FreeLookCam.Y_axis = 0;
			m_CameraJoyStick.JoyRect.anchoredPosition = Vector3.zero;
		}
		*/
		public void Destroy()
		{
			//rt = GameObject.FindGameObjectsWithTag ("RightD-Pad-Joystick");
			//foreach (GameObject g in rt)
			//	Destroy (g);
		}
	}
}