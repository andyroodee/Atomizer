using UnityEngine;
using System.Collections.Generic;

public static class AtomizerGameObjectExtensions
{
	public static List<T> GetAllComponents<T>(this GameObject target)
		where T : Component
	{
		List<T> components = new List<T>();
		
		T[] rootComponents = target.GetComponents<T>();
		if (rootComponents != null && rootComponents.Length > 0)
		{
			components.AddRange(rootComponents);
		}
		
		T[] childComponents = target.GetComponentsInChildren<T>();
		if (childComponents != null && childComponents.Length > 0)
		{
			components.AddRange(childComponents);
		}
		
		return components;
	}
}
