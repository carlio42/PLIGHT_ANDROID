using UnityEngine;
using UnityEngine.SceneManagement;
using Plight.Utilities;
using UnityEngine.UI;

namespace Plight.Audio
{
	[RequireComponent(typeof(AudioSource))]
	//This class manages the playing of all music in the scenes.
	public class SoundManager : PersistentSingleton<SoundManager>
	{
		static public SoundManager s_Instance;
		//The menu music track
		public Text m_ClipName;
		[SerializeField]
		protected AudioClip m_MenuMusic;
		private int m_RandomMusic;
		[SerializeField]
		protected AudioClip[] m_AndroidMenuMusics;

		//Field to represent the dedicated audiosource for this manager.
		private AudioSource m_MusicSource;

		public AudioSource musicSource
		{
			get { return m_MusicSource; }
		}

		//Property to allow the start of music to be delayed.
		[SerializeField]
		protected float m_StartDelay = 0.5f;

		//Property to set the rate that music fades in. If 0, music starts at full volume.
		[SerializeField]
		protected float m_FadeRate = 2f;

		//Audiosource volume on instantiation, in case we want to tweak the volume directly in editor.
		private float m_OriginalVolume;

		//Proportion of fading.
		private float m_FadeLevel = 1f;

		//Get references to the local audiosource on start, subscribe to the scene manager change event, and start the menu music (since Start will occur in the main menu).
		protected void Start()
		{
			s_Instance = this;

			m_MusicSource = GetComponent<AudioSource>();
			SceneManager.activeSceneChanged += OnSceneChanged;

			m_OriginalVolume = m_MusicSource.volume;

			PlayMusic(m_MenuMusic);
		}

		//Volume fade-in logic happens here, assuming the relevant parameters are set.
		protected void Update()
		{
			if (m_FadeLevel < 1f && m_FadeRate > 0f)
			{
				m_FadeLevel = Mathf.Lerp(m_FadeLevel, 1f, Time.deltaTime * m_FadeRate);

				if (m_FadeLevel >= 0.99f)
				{
					m_FadeLevel = 1f;
				}

				m_MusicSource.volume = m_OriginalVolume * m_FadeLevel;
			}
		}

		//This method is subscribed to the activeSceneChanged event, and will fire whenever the scene changes.
		private void OnSceneChanged(Scene scene1, Scene newScene)
		{
			if (m_MusicSource != null)
			{
				// Make sure to reset pitch
				m_MusicSource.pitch = 1;
			}

			//If we're transitioning to the menu scene, play the menu music (if it is not already playing). Otherwise pull and autoplay the current level's music.
			if (newScene.name == "AndroidUI") 
			{
				m_RandomMusic = Random.Range (0, m_AndroidMenuMusics.Length);
				PlayMusic (m_AndroidMenuMusics[m_RandomMusic], true);
			}
			else 
			{
				
			}
		}

		public void StopMusic()
		{
			m_MusicSource.Stop();
		}

		public void PlayCurrentMusic()
		{
			m_MusicSource.Play();
		}

		public void RandMusic()
		{
			m_RandomMusic = Random.Range (0, m_AndroidMenuMusics.Length);
			PlayMusic (m_AndroidMenuMusics[m_RandomMusic], true);
		}

		private void PlayMusic(AudioClip music, bool fadeIn = false, bool loop = true)
		{
			m_MusicSource.Stop();

			m_MusicSource.loop = loop;
			m_MusicSource.clip = music;
			m_MusicSource.PlayDelayed(m_StartDelay);

			if(m_ClipName != null)
				m_ClipName.text = "<i>"+music.name+"</i>";

			if (fadeIn)
			{
				m_FadeLevel = -m_StartDelay;
			}
		}

		//Unsubscribe from the SceneManager on destruction.
		protected override void OnDestroy()
		{
			SceneManager.activeSceneChanged -= OnSceneChanged;
			base.OnDestroy();
		}
	}
}
