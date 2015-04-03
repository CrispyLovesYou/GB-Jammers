using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T instance = null;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)FindObjectOfType(typeof(T));
				
				if (instance == null)
					Debug.LogError("Cannot find Singleton instance of " + typeof(T) + ".");
			}
			
			return instance;
		}
	}
    public static bool IsNull { get { return (instance == null); } }
	
	protected virtual void Awake()
	{
		if (instance != null)
			Debug.LogError("A Singleton instance of " + typeof(T) + " was already created!");
		
		instance = (T)this;
	}
}