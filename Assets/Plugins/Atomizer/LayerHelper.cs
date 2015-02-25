using UnityEngine;
using System.Collections;

public static class LayerHelper
{	
	public static void SetLayer(GameObject parent, int layer)
	{
		parent.layer = layer;
		Transform parentTransform = parent.transform;
		int childCount = parentTransform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = parentTransform.GetChild(i);
			SetLayer(child.gameObject, layer);
		}
	}	
}
