/* MinMaxRangeDrawer.cs
* by Eddie Cameron – For the public domain
* ———————————————————–
* — EDITOR SCRIPT : Place in a subfolder named ‘Editor’ —
* ———————————————————–
* Renders a MinMaxRange field with a MinMaxRangeAttribute as a slider in the inspector
* Can slide either end of the slider to set ends of range
* Can slide whole slider to move whole range
* Can enter exact range values into the From: and To: inspector fields
*
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    bool locked = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 16;
    }

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Now draw the property as a Slider or an IntSlider based on whether it’s a float or integer.
        if (property.type != "MinMaxRange")
            Debug.LogWarning("Use only with MinMaxRange type");
        else
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);


            //var range = attribute as MinMaxRangeAttribute;
            var minValue = property.FindPropertyRelative("rangeStart");
            var maxValue = property.FindPropertyRelative("rangeEnd");
            var limitMinProp = property.FindPropertyRelative("limitMin");
            var limitMaxProp = property.FindPropertyRelative("limitMax");
            var newMin = minValue.floatValue;
            var newMax = maxValue.floatValue;
            var newLimitMin = limitMinProp.floatValue;
            var newLimitMax = limitMaxProp.floatValue;

            var xDivision = position.width * 0.30f;
            var yDivision = position.height * 0.5f;


            //Displays the Label
            EditorGUI.LabelField(new Rect(position.x, position.y, xDivision, yDivision)
                , label);

            locked = EditorGUI.Toggle(new Rect(position.width - 4, position.y, 20, yDivision), GUIContent.none, locked);

            //Displays the LimitMin as a label
            GUI.enabled = locked;
            newLimitMin = EditorGUI.FloatField(new Rect(position.x, position.y + yDivision, 50f, yDivision)
                , newLimitMin);
            GUI.enabled = true;
            //Displays the MinMaxSlider
            EditorGUI.MinMaxSlider(new Rect(position.x + 54f, position.y + yDivision, position.width - 112f, yDivision)
                , ref newMin, ref newMax, newLimitMin, newLimitMax);
            //Displays the LimitMax as a label
            GUI.enabled = locked;
            newLimitMax = EditorGUI.FloatField(new Rect(position.x + position.width - 52f, position.y + yDivision, 50f, yDivision)
                , newLimitMax);
            GUI.enabled = true;

            //Displays the current min
            EditorGUI.LabelField(new Rect(position.x + xDivision, position.y, xDivision, yDivision)
                , "From: ");
            newMin = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision + 34, position.y, xDivision - 34, yDivision)
                , newMin)
                , newLimitMin, newMax);
            //Displayers the current max
            EditorGUI.LabelField(new Rect(position.x + xDivision * 2f, position.y, xDivision, yDivision)
                , "To: ");
            newMax = Mathf.Clamp(EditorGUI.FloatField(new Rect(position.x + xDivision * 2f + 24, position.y, xDivision - 41, yDivision)
                , newMax)
                , newMin, newLimitMax);

            minValue.floatValue = newMin;
            maxValue.floatValue = newMax;
            limitMinProp.floatValue = newLimitMin;
            limitMaxProp.floatValue = newLimitMax;

            EditorGUI.EndProperty();
        }
    }
}