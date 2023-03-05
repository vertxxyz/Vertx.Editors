using System;
using System.Reflection;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Vertx.Editors
{
	internal static class Shortcuts
	{
		[Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Control | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void ClearConsole() => Type.GetType("UnityEditor.LogEntries,UnityEditor")!.GetMethod("Clear", BindingFlags.Public | BindingFlags.Static)!.Invoke(null, null);
	}
}