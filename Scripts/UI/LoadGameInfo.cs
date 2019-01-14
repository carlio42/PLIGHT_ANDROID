using UnityEngine;
using UnityEngine.UI;

namespace Plight.UI
{
	public class LoadGameInfo : MonoBehaviour 
	{
		[SerializeField]
		protected Text m_VersionNumber;

		protected void Start()
		{
			m_VersionNumber.text = "V " + Application.version;
		}
		
	}
}