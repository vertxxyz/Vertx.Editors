using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vertx.Editors
{
	internal static class Shortcuts
	{
		[Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Action | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void ClearConsole() => Type.GetType("UnityEditor.LogEntries,UnityEditor")!.GetMethod("Clear", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, null);

		[Shortcut("Copy GUID and FileId from selected asset", KeyCode.I, ShortcutModifiers.Action | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		// [MenuItem("Assets/Copy GUID and FileId", priority = 19)] // priority cannot get this menu in the correct position
		public static void CopyGuid()
		{
			Object[] objects = Selection.objects;
			if (objects.Length == 0)
				return;
			
			// Copy the first selected GUID.
			if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(objects[0], out string guid, out long localId))
			{
				string log = $"{guid} : {localId}";
				EditorGUIUtility.systemCopyBuffer = log;
				if (objects.Length == 1)
				{
					Debug.Log(log);
					return;
				}
			}

			StringBuilder stringBuilder = new StringBuilder("IDs:");
			stringBuilder.AppendLine();
			foreach (Object o in objects)
			{
				if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out guid, out localId))
					continue;
				string log = $"{guid}: {localId}";
				stringBuilder.AppendLine(o.name);
				stringBuilder.AppendLine(log);
			}

			Debug.Log(stringBuilder);
		}
	}
}