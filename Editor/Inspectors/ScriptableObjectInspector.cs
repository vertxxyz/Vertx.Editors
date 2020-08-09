using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vertx.Utilities.Editor;

namespace Vertx.Editors.Editor
{
	[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
	public class ScriptableObjectInspector : UnityEditor.Editor
	{
		private GUIContent selectContent, searchContent, searchContentSmall;
		private static readonly Type scriptableObjectType = typeof(ScriptableObject);
		private List<GUIContent> searchForMoreContent;
		private List<string> moreContentTypeNames;
		private bool hasMoreSearches;

		private GUIContent dropdownIcon;
		private GUIContent DropdownIcon => dropdownIcon ?? (dropdownIcon = EditorGUIUtility.IconContent("Icon Dropdown"));

		protected virtual void OnEnable()
		{
			selectContent = new GUIContent("Select");
			Type type = target.GetType();
			searchContent = new GUIContent($"Search for {type.Name}");
			if (type.BaseType != scriptableObjectType)
			{
				searchForMoreContent = new List<GUIContent>();
				moreContentTypeNames = new List<string>();
				do
				{
					type = type.BaseType;
					searchForMoreContent.Add(new GUIContent($"Search for {type.Name}"));
					moreContentTypeNames.Add(type.FullName);
				} while (type.BaseType != scriptableObjectType && type.BaseType != null);
			}
			searchContentSmall = new GUIContent("Search");
		}

		protected override void OnHeaderGUI()
		{
			base.OnHeaderGUI();
			if (selectContent == null)
			{
				Debug.LogWarning($"base.OnEnable was not called for {GetType().Name}, a class inheriting from {nameof(ScriptableObjectInspector)}.");
				return;
			}

			Event e = Event.current;

			Rect position = GUILayoutUtility.GetLastRect();
			Rect titleRect = position;
			titleRect.xMin += 40;
			titleRect.xMax -= 55;
			titleRect.yMax -= 25;
			if (e.isMouse && e.type == EventType.MouseDown && titleRect.Contains(e.mousePosition))
			{
				DragAndDrop.objectReferences = targets;
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				DragAndDrop.StartDrag("Drag SO");
			}
			
			position.y = position.yMax - 21;
			position.height = 15;
			position.xMin += 46;
			position.xMax -= 55;

			Rect selectPosition = position;
			float searchWidth = EditorStyles.miniButton.CalcSize(searchContent).x;
			if (searchForMoreContent != null)
				searchWidth += 15;
			selectPosition.width = Mathf.Min(position.width / 2f, position.width - searchWidth);


			//Selectively use a small version of the search button when the large version forces the Select button to be too small.
			GUIContent searchContentToUse = searchContent;
			if (selectPosition.width < 60)
			{
				selectPosition.width = 60;
				searchContentToUse = searchContentSmall;
			}

			//Draw the Select button
			if (GUI.Button(selectPosition, selectContent, EditorStyles.miniButtonLeft))
			{
				Selection.activeObject = target;
				EditorGUIUtility.PingObject(target);
			}

			//Draw the Search button
			Rect searchPosition = position;
			searchPosition.xMin = selectPosition.xMax;
			if (searchForMoreContent == null)
			{
				if (GUI.Button(searchPosition, searchContentToUse, EditorStyles.miniButtonRight))
					EditorGUIUtils.SetProjectBrowserSearch($"t:{target.GetType().FullName}");
			}
			else
			{
				Rect searchPositionWhole = searchPosition;
				searchPosition.width -= 15;
				if (GUI.Button(searchPosition, searchContentToUse, EditorStyles.miniButtonMid))
					EditorGUIUtils.SetProjectBrowserSearch($"t:{target.GetType().FullName}");
				searchPosition.x = searchPosition.xMax - 1;
				searchPosition.width = 15;
				if (EditorGUI.DropdownButton(searchPosition, GUIContent.none, FocusType.Keyboard, EditorStyles.miniButtonRight))
				{
					GenericMenu menu = new GenericMenu();
					for (var i = 0; i < searchForMoreContent.Count; i++)
					{
						int iLocal = i;
						GUIContent content = searchForMoreContent[i];
						menu.AddItem(content, false, () => EditorGUIUtils.SetProjectBrowserSearch($"t:{moreContentTypeNames[iLocal]}"));
					}

					searchPositionWhole.yMax += 3;
					menu.DropDown(searchPositionWhole);
				}

				if (e.type == EventType.Repaint)
				{
					searchPosition.x += 1;
					searchPosition.y += 3;
					GUIStyle.none.Draw(searchPosition, DropdownIcon, false, false, false, false);
				}
			}
		}

		public override void OnInspectorGUI() => DrawDefaultInspector();
	}
}