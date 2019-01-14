using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[SerializeField]
public class News : MonoBehaviour 
{
	int UpdateCount;

	[SerializeField]
	GameObject m_NewsPrefab;

	[SerializeField]
	Transform NewsList;

	public InternetActivity Internet;

	void Start()
	{
		StartCoroutine (ShowGameUpdates());
	}

	IEnumerator ShowGameUpdates()
	{
		WWW ImageCount = new WWW ("http://databasehandler42.gearhostpreview.com/unitywww/imgcount.php");
		yield return ImageCount;
		WWW TextCount = new WWW ("http://databasehandler42.gearhostpreview.com/unitywww/txtcount.php");
		yield return TextCount;
		UpdateCount = Int32.Parse (TextCount.text);

		for (int i = 1; i <= UpdateCount; i++) 
		{
			WWW img = new WWW ("http://databasehandler42.gearhostpreview.com/unitywww/imgupdate"+i+".jpg");	//
			yield return img;
			
			Texture2D texture = new Texture2D (img.texture.width, img.texture.height, TextureFormat.DXT1, false);
			img.LoadImageIntoTexture (texture);

			Rect rec = new Rect (0, 0, texture.width, texture.height);
			Sprite spriteToUse = Sprite.Create (texture, rec, new Vector2 (0.5f, 0.5f), 100);

			WWW txt = new WWW ("http://databasehandler42.gearhostpreview.com/unitywww/txtupdate"+i+".txt");
			yield return txt;

			GameObject itemGO = (GameObject)Instantiate (m_NewsPrefab,NewsList);
			Newsitems items = itemGO.GetComponent<Newsitems> ();
			if (items != null)
				items.Setup (spriteToUse, txt.text);
		}
	}
}
