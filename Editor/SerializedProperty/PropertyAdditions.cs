using UnityEditor;
using UnityEngine;
using Vertx.Utilities.Editor;

namespace Vertx.Editors
{
	internal static class PropertyAdditions
	{
		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
			EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
		}

		private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
		{
			if (property.isArray)
			{
				// Array Reverse.
				if (property.arraySize <= 1)
					menu.AddDisabledItem(new GUIContent("Reverse"));
				else
				{
					var propertyCopy = property.Copy();
					menu.AddItem(new GUIContent("Reverse"), false, arg =>
					{
						var prop = (SerializedProperty) arg;
						prop.ReverseArray();
						prop.serializedObject.ApplyModifiedProperties();
					}, propertyCopy);
				}
				
				if(property.arraySize == 0)
					menu.AddDisabledItem(new GUIContent("Clear"));
				else
				{
					var propertyCopy = property.Copy();
					menu.AddItem(new GUIContent("Clear"), false, arg =>
					{
						var prop = (SerializedProperty) arg;
						prop.arraySize = 0;
						prop.serializedObject.ApplyModifiedProperties();
					}, propertyCopy);
				}
			}

			if (property.serializedObject != null)
			{
				// Blend shapes.
				if (property.serializedObject.targetObject is SkinnedMeshRenderer && property.propertyPath.StartsWith("m_BlendShapeWeights.Array."))
				{
					menu.AddItem(new GUIContent("Copy Index"), false, () => EditorGUIUtility.systemCopyBuffer = GetIndex().ToString());

					menu.AddItem(new GUIContent("Copy Name"), false, () =>
					{
						int index = GetIndex();
						EditorGUIUtility.systemCopyBuffer = ((SkinnedMeshRenderer)property.serializedObject.targetObject).sharedMesh.GetBlendShapeName(index);
					});

					int GetIndex()
					{
						// "m_BlendShapeWeights.Array.data[...]"
						int startIndex = "m_BlendShapeWeights.Array.data[".Length;
						int endIndex = property.propertyPath.Length - 1;
						return int.Parse(property.propertyPath.Substring(startIndex, endIndex - startIndex));
					}
				}
			}
		}
	}
}