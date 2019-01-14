using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyNetwork
{
	public class TempTouch : MonoBehaviour 
	{
		public Touch iniTouch = new Touch ();
		public Camera cam;
		public FreeLookCam m_FreeLookCam;

		public float rotX;
		public float rotY;

		public float deltaX;
		public float deltaY;

		public Vector3 OrigRot;
		//float speed = 0.002f;
		//float dif = -1f;

		void Start()
		{
			rotX = m_FreeLookCam.X_axis;
			rotY = m_FreeLookCam.Y_axis;
		}

		void Update()
		{
			foreach (Touch touch in Input.touches) 
			{
				if (touch.phase == TouchPhase.Began)
				{
					iniTouch = touch;
				}

				else if (touch.phase == TouchPhase.Moved) 
				{
					deltaX = iniTouch.position.x - touch.position.x;
					deltaY = iniTouch.position.y - touch.position.y;

					//rotX -= deltaY * speed;
					//rotY += deltaX * speed;
					//rotX = Mathf.Clamp (rotX, -80f, 80f);
					//cam.transform.eulerAngles = new Vector3 (rotX, rotY, 0f);
					//m_FreeLookCam.HandleRotationMovement(deltaX, deltaY);
				}

				else if(touch.phase == TouchPhase.Ended)
				{
					iniTouch = new Touch ();
					m_FreeLookCam.X_axis = 0;
					m_FreeLookCam.Y_axis = 0;

					deltaX = 0;
					deltaY = 0;
				}
			}
		}
	}
}