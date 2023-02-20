using System;
using System.Reflection;
using UnityEditor;

namespace Vertx.Editors
{
	internal static class GizmoEditMenu
	{
		[MenuItem("Edit/Gizmos/Disable All Icons")]
		private static void DisableAllGizmoIcons() => GizmoIconsSetEnabled(true, false);
		
		[MenuItem("Edit/Gizmos/Enable All Icons")]
		private static void EnableAllGizmoIcons() => GizmoIconsSetEnabled(true, true);

		[MenuItem("Edit/Gizmos/Disable Scripts Icons")]
		private static void DisableScriptGizmoIcons() => GizmoIconsSetEnabled(false, false);

		private static void GizmoIconsSetEnabled(bool disableBuiltInIcons, bool value)
		{
			var annotationUtilityType = Type.GetType("UnityEditor.AnnotationUtility,UnityEditor");
			var annotationType = Type.GetType("UnityEditor.Annotation,UnityEditor");

			// Get annotations
			MethodInfo getAnnotations = annotationUtilityType!.GetMethod("GetAnnotations", BindingFlags.NonPublic | BindingFlags.Static)!;
			MethodInfo setIconEnabled = annotationUtilityType!.GetMethod("SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Static)!;
			var annotations = (Array)getAnnotations.Invoke(null, null);

			//
			FieldInfo classIDField = annotationType!.GetField("classID", BindingFlags.Public | BindingFlags.Instance)!;
			FieldInfo scriptClassField = annotationType!.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance)!;

			object[] parameters = new object[3];
			parameters[2] = value ? 1 : 0;

			foreach (object annotation in annotations)
			{
				parameters[0] = classIDField.GetValue(annotation);
				parameters[1] = scriptClassField.GetValue(annotation);
				if (!disableBuiltInIcons && string.IsNullOrEmpty((string)parameters[1]))
					continue;
				setIconEnabled.Invoke(null, parameters);
			}
		}
	}
}