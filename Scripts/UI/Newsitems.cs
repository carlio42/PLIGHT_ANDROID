using UnityEngine;
using UnityEngine.UI;

public class Newsitems : MonoBehaviour 
{
	[SerializeField]
	Image img;
	[SerializeField]
	Text txt;

	public void Setup(Sprite sprt, string text)
	{
		img.sprite = sprt;
		txt.text = text;
	}
}
