using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeWeaponObject
{
	[MenuItem("MyAssets/Create/Weapon Object")]
	public static void CreateWeaponObject()
	{
		WeaponObject asset = ScriptableObject.CreateInstance<WeaponObject> ();
		AssetDatabase.CreateAsset (asset, "Assets/Scripts/Weapons/WeaponsAsset/NewWeaponObject.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;

	}

	[MenuItem("MyAssets/Create/Loadout Object")]
	public static void CreateLoadout()
	{
		LoadoutObj asset = ScriptableObject.CreateInstance<LoadoutObj> ();
		AssetDatabase.CreateAsset (asset, "Assets/Scripts/Weapons/Loadouts/NewLoadoutObj.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}