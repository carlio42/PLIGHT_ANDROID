using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Plight.Audio;

namespace Plight.UI
{
	public class GameSettings : MonoBehaviour 
	{
		[SerializeField]
		protected Slider m_Master, m_Music, m_SFX;

		[SerializeField]
		public Text m_SongName;
		[SerializeField]
		protected Button m_PlayButton, m_StopButton, m_RandomButton;
		[SerializeField]
		protected AudioMixer m_AudioMixer;

		[SerializeField]
		protected string m_MusicVolume = "MusicVolume", m_SfxVolume = "SFXVolume", m_MasterVolume = "MasterVolume";

		void OnEnable()
		{
			SoundManager.s_Instance.m_ClipName = m_SongName;
			m_PlayButton.onClick.AddListener (OnClickPlayBtn);
			m_StopButton.onClick.AddListener (OnClickStopBtn);
			m_RandomButton.onClick.AddListener (OnClickRandBtn);

		}

		public void OnVolumeChanged()
		{
			m_AudioMixer.SetFloat(m_MusicVolume, 40f * Mathf.Log10(m_Music.value));
			m_AudioMixer.SetFloat(m_SfxVolume, 40f * Mathf.Log10(m_SFX.value));
			m_AudioMixer.SetFloat(m_MasterVolume, 40f * Mathf.Log10(m_Master.value));
		}

		protected void OnClickPlayBtn()
		{
			SoundManager.s_Instance.PlayCurrentMusic ();
		}

		protected void OnClickStopBtn()
		{
			SoundManager.s_Instance.StopMusic ();
		}

		protected void OnClickRandBtn()
		{
			SoundManager.s_Instance.RandMusic ();
		}

	}
}