using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnumFlagAttribute : PropertyAttribute
{
	public string name;

	public EnumFlagAttribute() { }

	public EnumFlagAttribute(string name)
	{
		this.name = name;
	}
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnumFlagAttribute))]
public class EnumFlagDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
		Enum targetEnum = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);

		string propName = flagSettings.name;
		if (string.IsNullOrEmpty(propName))
			propName = ObjectNames.NicifyVariableName(property.name);

		EditorGUI.BeginProperty(position, label, property);
		Enum enumNew = EditorGUI.EnumMaskPopup(position, propName, targetEnum);
		property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
		EditorGUI.EndProperty();
	}
}
#endif