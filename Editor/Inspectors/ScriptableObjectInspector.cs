﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vertx.Editors.Editor
{
	[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
	public class ScriptableObjectInspector : UnityEditor.Editor
	{
		private GUIContent selectContent, searchContent, searchContentSmall;
		private Type type;
		private static readonly Type scriptableObjectType = typeof(ScriptableObject);
		private List<GUIContent> searchForMoreContent;
		private List<Type> moreContentTypes;

		protected virtual void OnEnable()
		{
			type = target.GetType();
			InspectorShared.PopulateGUIContent(
				type, 
				scriptableObjectType,
				out selectContent, 
				out searchContent,
				out searchContentSmall,
				out searchForMoreContent,
				out moreContentTypes);
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
			titleRect.yMax -= 23;
			if (e.isMouse && e.type == EventType.MouseDown && titleRect.Contains(e.mousePosition))
			{
				DragAndDrop.objectReferences = targets;
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				DragAndDrop.StartDrag("Drag SO");
			}
			
			position.y = position.yMax - 26;
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
			InspectorShared.DrawSearchButton(position, selectPosition, searchContentToUse, type, searchForMoreContent, moreContentTypes);
		}

		public override void OnInspectorGUI() => DrawDefaultInspector();
	}
}