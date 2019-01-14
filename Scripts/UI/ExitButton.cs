using UnityEngine;
using UnityEngine.UI;

namespace Plight.UI
{
	public class ExitButton : MonoBehaviour
	{
		[SerializeField]
		protected CanvasGroup ExitPanel;
		protected bool isClicked = false;

		void Update()
		{
			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				if (ExitPanel != null)
					EnableExitPanel ();
				else
					Application.Quit ();
			}
		}

		protected void EnableExitPanel()
		{
			if (!isClicked) 
			{
				ExitPanel.gameObject.SetActive (true);
				isClicked = !isClicked;
				return;
			}

			if(isClicked)
			{
				ExitPanel.gameObject.SetActive (false);
				isClicked = !isClicked;
				return;
			}
				
		}
	}
}