using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Vertx.Editors.Editor
{
	[CustomEditor(typeof(MonoScript), true), CanEditMultipleObjects]
	public class ScriptInspector : UnityEditor.Editor
	{
		private UnityEditor.Editor baseMonoScriptInspector;
		
		private InspectorShared.ScriptType scriptType;
		private GUIContent selectContent, searchContent, searchContentSmall;
		private Type type;
		private static readonly Type scriptableObjectType = typeof(ScriptableObject);
		private List<GUIContent> searchForMoreContent;
		private List<Type> moreContentTypes;

		public void OnEnable()
		{
			baseMonoScriptInspector = CreateEditor(target, Type.GetType("UnityEditor.MonoScriptInspector,UnityEditor"));
			
			type = target == null // I have found this to sometimes be the case.
				? null 
				: ((MonoScript)target).GetClass();
			
			if (type == null)
			{
				scriptType = InspectorShared.ScriptType.Other;
				return;
			}
			
			scriptType = InspectorShared.PopulateMonoScriptGUIContent(
				type, 
				out selectContent, 
				out searchContent,
				out searchContentSmall,
				out searchForMoreContent,
				out moreContentTypes);
		}

		public void OnDisable()
		{
			if (baseMonoScriptInspector != null)
				DestroyImmediate(baseMonoScriptInspector);
		}

		protected override void OnHeaderGUI()
		{
			base.OnHeaderGUI();
			
			if (scriptType == InspectorShared.ScriptType.Other)
				return;
			
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
				DragAndDrop.StartDrag("Drag Script");
			}
			
			position.y = position.yMax - 26;
			position.height = 15;
			position.xMin += 46;
			position.xMax -= 4;

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
			InspectorShared.DrawSearchButton(position, selectPosition, searchContentToUse, type, searchForMoreContent, moreContentTypes, scriptType);
		}

		public override void OnInspectorGUI()
		{
			if(baseMonoScriptInspector != null)
				baseMonoScriptInspector.OnInspectorGUI();
		}

		public override VisualElement CreateInspectorGUI() => baseMonoScriptInspector != null ? baseMonoScriptInspector.CreateInspectorGUI() : null;
	}
}