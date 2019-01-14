using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour 
{

	public GameObject LoadingScreen;
	public Slider slider;
	public Text ProgressText;

	public void LoadLevel(string sceneName)
	{
		StartCoroutine (LoadAsynchronously(sceneName));
	}

	IEnumerator LoadAsynchronously(string sceneName)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneName);
		LoadingScreen.SetActive (true);

		while(!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			slider.value = progress;
			ProgressText.text = progress * 100f + "%";

			yield return null;
		}
		LoadingScreen.SetActive (false);
	}
}
