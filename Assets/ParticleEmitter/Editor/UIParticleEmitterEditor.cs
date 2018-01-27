using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UIParticleEmitter))]
public class UIParticleEmitterEditor : Editor {

	public override void OnInspectorGUI()
	{
		UIParticleEmitter script = (UIParticleEmitter)target;


		script.Loop = EditorGUILayout.Toggle ("Loop", script.Loop);
		if(!script.Loop)
			script.EmitterDuration = EditorGUILayout.FloatField ("Duration", script.EmitterDuration);




		script.sprite = (Sprite)EditorGUILayout.ObjectField ("Sprite", script.sprite,typeof(Sprite),false, GUILayout.Height(15F));
		script.EmissionRate=EditorGUILayout.IntField ("Emission Rate", script.EmissionRate);


		script.sizeType =(UIParticleEmitter.SizeType)EditorGUILayout.EnumPopup ("Size Type", script.sizeType);
		if (script.sizeType == UIParticleEmitter.SizeType.Single) {
			script.Size [0] = EditorGUILayout.FloatField ("Size", script.Size [0]);
		}
		if (script.sizeType == UIParticleEmitter.SizeType.Double) {
			script.Size [0] = EditorGUILayout.FloatField ("Size 1", script.Size [0]);
			script.Size [1] = EditorGUILayout.FloatField ("Size 2", script.Size [1]);
		}
		/*if (script.sizeType == UIParticleEmitter.SizeType.Curve) {
			script.Size [0] = EditorGUILayout.FloatField ("Size 1", script.Size [0]);
			script.SizeCurveX =EditorGUILayout.CurveField ("Curve X", script.SizeCurveX);
			script.SizeCurveY =EditorGUILayout.CurveField ("Curve Y", script.SizeCurveY);
		}*/


		script.PreWarm = EditorGUILayout.Toggle ("PreWarm", script.PreWarm);


		script.LifeSpan=EditorGUILayout.FloatField ("Life Span", script.LifeSpan);



		script.PoolSize=EditorGUILayout.IntField ("Maximum Number of Particles", script.PoolSize);
		while (script.particlePool.Count + script.activeParticles.Count > script.PoolSize) 
		{
			if (script.particlePool.Count > 0) {
				script.particlePool.RemoveAt (0);
				continue;
			}
			script.activeParticles.RemoveAt (0);
		}


		script.colorType =(UIParticleEmitter.ColorType)EditorGUILayout.EnumPopup ("Color Type", script.colorType);
		if (script.colorType == UIParticleEmitter.ColorType.Single) {
			script.colors[0] = EditorGUILayout.ColorField ("Color", script.colors[0]);
		}
		if (script.colorType == UIParticleEmitter.ColorType.Double) {
			script.colors[0] = EditorGUILayout.ColorField ("Starting Color", script.colors[0]);
			script.colors[1] = EditorGUILayout.ColorField ("End Color", script.colors[1]);
		}


		if (script.colorType == UIParticleEmitter.ColorType.Gradient) {
			EditorGUI.BeginChangeCheck ();
			SerializedObject serializedGradient = new SerializedObject (target);
			SerializedProperty colorGradient = serializedGradient.FindProperty ("gradient");
			EditorGUILayout.PropertyField (colorGradient, true, null);
			if (EditorGUI.EndChangeCheck ())
				serializedGradient.ApplyModifiedProperties ();
		}
		if (script.colorType == UIParticleEmitter.ColorType.Double_Gradient) {
			EditorGUI.BeginChangeCheck ();
			SerializedObject serializedGradient = new SerializedObject (target);
			SerializedProperty colorGradient = serializedGradient.FindProperty ("gradient");
			EditorGUILayout.PropertyField (colorGradient, true, null);
			if (EditorGUI.EndChangeCheck ())
				serializedGradient.ApplyModifiedProperties ();


			EditorGUI.BeginChangeCheck ();
			SerializedObject serializedGradient2 = new SerializedObject (target);
			SerializedProperty colorGradient2 = serializedGradient2.FindProperty ("gradient2");
			EditorGUILayout.PropertyField (colorGradient2, true, null);
			if (EditorGUI.EndChangeCheck ())
				serializedGradient2.ApplyModifiedProperties ();
		}

		script.direction =(UIParticleEmitter.Direction)EditorGUILayout.EnumPopup ("Emission", script.direction);



		if (script.direction == UIParticleEmitter.Direction.Radius) {
			//script.Radius = EditorGUILayout.FloatField ("Radius", script.Radius);
			script.innerRadius =EditorGUILayout.Slider ("Inner Radius", script.innerRadius,0f,1f);

		} else if (script.direction == UIParticleEmitter.Direction.Cone) {
			script.Angle = EditorGUILayout.FloatField ("Angle", script.Angle);
			script.Threshold = EditorGUILayout.Slider ("Threshold", script.Threshold,0f,1f);
		}


		script.raycastTarget = EditorGUILayout.Toggle ("Raycast Target", script.raycastTarget);
		//DrawDefaultInspector ();
	}
	void OnSceneGUI(){

		UIParticleEmitter script = (UIParticleEmitter)target;
		if (script.direction == UIParticleEmitter.Direction.Radius) {
			Handles.color = new Color (0.3f, 1f, 1f, 1f);
			Vector3 startCorner = new Vector3 (script.rectTransform.rect.width / 2f, 0) + script.transform.position;

			// The "previous" corner point, initialised to the starting corner.
			Vector3 previousCorner = startCorner;
			int numSides = 36;
			// For each corner after the starting corner...
			for (int i = 1; i < numSides + 1; i++) {
				// Calculate the angle of the corner in radians.
				float cornerAngle = 2f * Mathf.PI / (float)numSides * i;

				// Get the X and Y coordinates of the corner point.
				Vector3 currentCorner = new Vector3 (Mathf.Cos (cornerAngle) * script.rectTransform.rect.width / 2f,
					                        Mathf.Sin (cornerAngle) * script.rectTransform.rect.height / 2f) + script.transform.position;

				// Draw a side of the polygon by connecting the current corner to the previous one.
				Handles.DrawLine (currentCorner, previousCorner);

				// Having used the current corner, it now becomes the previous corner.
				previousCorner = currentCorner;
			}

			Handles.color = new Color (1f,1f, 0.3f, 1f);
			previousCorner = new Vector3 ((script.rectTransform.rect.width / 2f) * script.innerRadius, 0) + script.transform.position;
			for (int i = 1; i < numSides + 1; i++) {
				// Calculate the angle of the corner in radians.
				float cornerAngle = 2f * Mathf.PI / (float)numSides * i;

				// Get the X and Y coordinates of the corner point.
				Vector3 currentCorner = new Vector3 (Mathf.Cos (cornerAngle) * (script.rectTransform.rect.width / 2f) * script.innerRadius,
					                        Mathf.Sin (cornerAngle) * (script.rectTransform.rect.height / 2f) * script.innerRadius) + script.transform.position;

				// Draw a side of the polygon by connecting the current corner to the previous one.
				Handles.DrawLine (currentCorner, previousCorner);

				// Having used the current corner, it now becomes the previous corner.
				previousCorner = currentCorner;
			}
		} else if (script.direction == UIParticleEmitter.Direction.Cone) {
			Vector3 angle1= Quaternion.AngleAxis (-script.Angle, Vector3.forward) * Vector3.right;
			Vector3 angle2= Quaternion.AngleAxis (script.Angle, Vector3.forward) * Vector3.right;

			angle1 = new Vector3 (angle1.x * script.rectTransform.rect.width/2f, angle1.y * script.rectTransform.rect.height/2f);
			angle2 = new Vector3 (angle2.x * script.rectTransform.rect.width/2f, angle2.y * script.rectTransform.rect.height/2f);

			Vector3 line1Start = script.transform.position +  (Vector3.left*script.rectTransform.rect.width*0.5f)*(1f-script.Threshold) +angle1*script.Threshold ;
			Vector3 line2Start = script.transform.position +  (Vector3.left*script.rectTransform.rect.width*0.5f)*(1f-script.Threshold) +angle2*script.Threshold ;
			Vector3 line1End = script.transform.position + angle1 ;
			Vector3 line2End = script.transform.position + angle2 ;


			Handles.color = new Color (0.3f, 1f, 1f, 1f);
			Handles.DrawLine (line1Start,line2Start);
			Handles.DrawLine (line1Start, line1End);
			Handles.DrawLine (line2Start, line2End);
		}

	}
}
