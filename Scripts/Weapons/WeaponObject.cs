using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class WeaponObject : ScriptableObject 
{
	public int WeaponIndex;
	public string weaponName = "Weapon Name Here";
	public bool isUnlocked;
	public float FireRate = 0.0f;
	public float EffectDisplayTime = 0.0f;
	public float ReloadTime;
	public int Magazine;
	public int MaxBulletes;
	public float damage = 0.0f;
	public float range = 0;
	public Color color = Color.yellow;	

	public GameObject FireMark;
	public GameObject CrossHair;
	public AudioClip shotSound;

	public Sprite WeaponImage;

}
