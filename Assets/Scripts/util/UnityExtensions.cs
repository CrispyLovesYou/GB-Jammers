using System;
using UnityEngine;

public static class UnityExtensions
{
	/// <summary>
	/// Allows a GameObject to get a component, leaving an error message if none is found.
	/// </summary>
	public static T GetSafeComponent<T>(this GameObject _obj) where T : Component
	{
		T component = _obj.GetComponent<T>();
		
		if (component == null)
			Debug.LogError("GameObject '" + _obj.name + "' does not have '" + typeof(T) + "' component.", _obj);
		
		return component;
	}
}