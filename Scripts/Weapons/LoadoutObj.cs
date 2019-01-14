using System.Collections;
using UnityEngine;

[System.Serializable]
public class LoadoutObj : ScriptableObject 
{
	public string m_LoadoutName;
	public WeaponObject m_PrimaryWeapon;
	public int m_PAmmo;
	public WeaponObject m_SecondryWeapon;
	public int m_SAmmo;
}
