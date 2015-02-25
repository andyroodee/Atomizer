using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Atomizer))]
public class AtomizerEditor : Editor 
{
	private SerializedObject atomizer;
	private SerializedProperty renderLayer;

	void OnEnable()
	{
		atomizer = new SerializedObject(target);
		renderLayer = atomizer.FindProperty("renderLayer");
	}

	public override void OnInspectorGUI() 
	{
		if (target == null)
		{
			return;
		}
		
		atomizer.Update();

		renderLayer.intValue = EditorGUILayout.LayerField("Atomizer Layer:", renderLayer.intValue);

		atomizer.ApplyModifiedProperties();
	}
}
