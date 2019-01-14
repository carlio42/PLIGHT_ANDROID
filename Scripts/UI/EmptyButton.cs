using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class EmptyButton : MonoBehaviour 
{
	public RectTransform WarningPanel;
	public Button ExitButton;
	public AudioSource m_Asource;

	public AudioSource MusicsSource;
	public AudioClip[] BackgroundSongs;
	public Text m_Songname;

	void Start()
	{
		m_Asource = GetComponent<AudioSource> ();

		MusicsSource.clip = BackgroundSongs [0];

		MusicsSource.Play ();
		SongName ();
	}

	void SongName()
	{
		m_Songname.text = MusicsSource.clip.name;
	}
	public void NoFunction()
	{
		WarningPanel.gameObject.SetActive (true);
		//m_Asource.clip = SadWarningClip;
		m_Asource.Play ();
	}

	public void StopSound()
	{
		m_Asource.Stop ();
	}

	public void PlaySong()
	{
		int RandomClip = Random.Range (0, BackgroundSongs.Length);
		MusicsSource.clip = BackgroundSongs [RandomClip];

		MusicsSource.Play ();
		SongName ();
	}

	public void StopSong()
	{
		MusicsSource.Stop ();
		SongName ();
	}
}
