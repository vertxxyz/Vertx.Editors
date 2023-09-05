using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vertx.Utilities.Editor;

namespace Vertx.Editors.Editor
{
	internal static class InspectorShared
	{
		public static bool PopulateGUIContent(
			Type type,
			Type baseType,
			out GUIContent selectContent,
			out GUIContent searchContent,
			out GUIContent searchContentSmall,
			out List<GUIContent> searchForMoreContent,
			out List<Type> moreContentTypes
		)
		{
			searchContent = new GUIContent($"Search for {type.Name}");
			selectContent = new GUIContent("Select");
			searchContentSmall = new GUIContent("Search");
			if (type.BaseType != baseType)
			{
				searchForMoreContent = new List<GUIContent>();
				moreContentTypes = new List<Type>();
				do
				{
					type = type.BaseType;
					searchForMoreContent.Add(new GUIContent($"Search for {type.Name}"));
					moreContentTypes.Add(type);
					if (type.BaseType == null)
						return false;
				} while (type.BaseType != baseType);
			}
			else
			{
				searchForMoreContent = null;
				moreContentTypes = null;
			}

			return true;
		}

		public enum ScriptType
		{
			Other,
			ScriptableObject,
			MonoBehaviour
		}
		
		public static ScriptType PopulateMonoScriptGUIContent(
			Type type,
			out GUIContent selectContent,
			out GUIContent searchContent,
			out GUIContent searchContentSmall,
			out List<GUIContent> searchForMoreContent,
			out List<Type> moreContentTypes
		)
		{
			Type soType = typeof(ScriptableObject);
			Type mbType = typeof(MonoBehaviour);
			searchContent = new GUIContent($"Search for {type.Name}");
			selectContent = new GUIContent("Select");
			searchContentSmall = new GUIContent("Search");
			if (type.BaseType != null && type.BaseType != soType && type.BaseType != mbType)
			{
				searchForMoreContent = new List<GUIContent>();
				moreContentTypes = new List<Type>();
				do
				{
					type = type.BaseType;
					searchForMoreContent.Add(new GUIContent($"Search for {type.Name}"));
					moreContentTypes.Add(type);
					if (type.BaseType == null)
						return ScriptType.Other;
				} while (type.BaseType != soType && type.BaseType != mbType);
			}
			else
			{
				searchForMoreContent = null;
				moreContentTypes = null;
			}

			return type.BaseType == soType ? ScriptType.ScriptableObject : ScriptType.MonoBehaviour;
		}

		public static void DrawSearchButton(
			Rect position,
			Rect selectPosition,
			GUIContent searchContent,
			Type type,
			List<GUIContent> searchForMoreContent,
			List<Type> moreContentTypeNames,
			ScriptType scriptType = ScriptType.ScriptableObject
		)
		{
			//Draw the Search button
			Rect searchPosition = position;
			searchPosition.xMin = selectPosition.xMax;
			if (searchForMoreContent == null)
			{
				if (GUI.Button(searchPosition, searchContent, EditorStyles.miniButtonRight))
				{
					switch (scriptType)
					{
						case ScriptType.Other:
							break;
						case ScriptType.ScriptableObject:
							EditorUtils.SetProjectBrowserSearch($"t:{type.Name}");
							break;
						case ScriptType.MonoBehaviour:
							PrefabSearchWindow.Open(type);
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(scriptType), scriptType, null);
					}
				}
			}
			else
			{
				Rect searchPositionWhole = searchPosition;
				searchPosition.width -= 15;
				if (GUI.Button(searchPosition, searchContent, EditorStyles.miniButtonMid))
					EditorUtils.SetProjectBrowserSearch($"t:{type.Name}");
				searchPosition.x = searchPosition.xMax - 1;
				searchPosition.width = 15;
				if (EditorGUI.DropdownButton(searchPosition, GUIContent.none, FocusType.Keyboard, EditorStyles.miniButtonRight))
				{
					GenericMenu menu = new GenericMenu();
					for (var i = 0; i < searchForMoreContent.Count; i++)
					{
						GUIContent content = searchForMoreContent[i];
						Type localType = moreContentTypeNames[i];
						if (localType.IsGenericType)
						{
							//If someone knows how to support generics in type searches I would love to know!
							menu.AddDisabledItem(content);
						}
						else
						{
							menu.AddItem(content, false, () =>
							{
								switch (scriptType)
								{
									case ScriptType.Other:
										break;
									case ScriptType.ScriptableObject:
										EditorUtils.SetProjectBrowserSearch($"t:{localType.Name}");
										break;
									case ScriptType.MonoBehaviour:
										PrefabSearchWindow.Open(localType);
										break;
									default:
										throw new ArgumentOutOfRangeException(nameof(scriptType), scriptType, null);
								}
							});
						}
					}

					searchPositionWhole.yMax += 3;
					menu.DropDown(searchPositionWhole);
				}

				if (Event.current.type == EventType.Repaint)
				{
					searchPosition.x += 1;
					searchPosition.y += 3;
					GUIStyle.none.Draw(searchPosition, DropdownIcon, false, false, false, false);
				}
			}
		}

		private static GUIContent dropdownIcon;
		public static GUIContent DropdownIcon => dropdownIcon ?? (dropdownIcon = EditorGUIUtility.IconContent("Icon Dropdown"));
	}
}