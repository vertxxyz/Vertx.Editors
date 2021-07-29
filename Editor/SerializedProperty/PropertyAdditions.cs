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
			if (!property.isArray)
				return;
			
			if (property.arraySize <= 1)
				menu.AddDisabledItem(new GUIContent("Reverse"));
			else
			{
				var propertyCopy = property.Copy();
				menu.AddItem(new GUIContent("Reverse"), false, () =>
				{
					propertyCopy.ReverseArray();
					propertyCopy.serializedObject.ApplyModifiedProperties();
				});
			}
		}
	}
}