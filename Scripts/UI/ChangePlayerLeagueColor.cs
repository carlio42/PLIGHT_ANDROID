using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangePlayerLeagueColor : MonoBehaviour
{
	Image LeagueImage;

	static Color ReadyColor = new Color(240.0f / 255.0f, 134.0f / 255.0f, 0, 1.0f);
	float R = 240.0f, G = 134.0f, B = 0.0f, A = 1.0f;

	void Start()
	{
		LeagueImage = GetComponent<Image> ();
	}

	public void ChangeColor(int XP)
	{
		StartCoroutine (ChangeColorS(XP));
	}
	IEnumerator ChangeColorS(int XP)
	{
		yield return new WaitForSeconds (1.0f);


		if (XP > 0 && XP <= 20) 
		{
			R = 240.0f;
			G = 134.0f;
			B = 0.0f;
	
			G += (20 * 7.0f);
			//ReadyColor = new Color (R / 255.0f, G / 255.0f, B, A);
		}

		else if (XP >= 21 && XP <= 40) 
		{
			G = 240.0f;
			B = 0.0f;
			R = 240.0f;

			R -= (XP * 6.0f);
			//ReadyColor = new Color (R / 255.0f, G / 255.0f, B, A);
		}

		else if(XP >=41 && XP <= 60)
		{
			G = 240.0f;
			R = 0.0f;
			B = 0.0f;

			B +=(XP * 4.0f);
			//ReadyColor = new Color (R / 255.0f, G / 255.0f, B / 255.0f, A);
		}

		else if(XP >=61 && XP <= 80)
		{
			R = 0.0f;
			B = 240.0f;
			G = 240.0f;

			G -= (XP * 3.0f);
			//ReadyColor = new Color (R / 255.0f, G / 255.0f, B / 255.0f, A);
		}

		else if(XP >= 81 && XP <= 90)
		{
			R = 0.0f;
			G = 0.0f;
			B = 240.0f;

			R += (XP * 2.6f);
		}

		else if(XP >= 91 && XP <= 100)
		{
			R = 240.0f;
			G = 0.0f;
			B = 240.0f;

			B -= (XP * 2.4f);
		}

		ReadyColor = new Color (R / 255.0f, G / 255.0f, B / 255.0f, A);
		LeagueImage.color = ReadyColor;
	}
}