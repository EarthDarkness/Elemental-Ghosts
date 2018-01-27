using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UIGradient))]
public class UIGradientEditor : Editor {

	public override void OnInspectorGUI()
	{
		UIGradient script = (UIGradient)target;

		script.colors[0] = EditorGUILayout.ColorField ("Bot1", script.colors[0]);
		script.colors[3] = EditorGUILayout.ColorField ("Bot2", script.colors[3]);
		script.colors[1] = EditorGUILayout.ColorField ("Top1", script.colors[1]);
		script.colors[2] = EditorGUILayout.ColorField ("Top2", script.colors[2]);
		
	}
	void OnSceneGUI(){

		
		

	}
}
