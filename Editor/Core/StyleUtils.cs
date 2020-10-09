using UnityEditor;
using UnityEngine.UIElements;

namespace Vertx.Editors.Editor
{
	public static class StyleUtils
	{
		public static StyleSheet GetStyleSheet(string name)
		{
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(StyleSheet)} {name}");
			if (guids.Length == 0)
				return null;
			var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
			return sheet;
		}
		
		public static VisualTreeAsset GetUXML(string name)
		{
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(VisualTreeAsset)} {name}");
			if (guids.Length == 0)
				return null;
			var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
			return uxml;
		}
		
		public static (StyleSheet, VisualTreeAsset) GetStyleSheetAndUXML(string name)
		{
			string[] guids = AssetDatabase.FindAssets(name);
			if (guids.Length == 0)
				return (null, null);
			(StyleSheet, VisualTreeAsset) values = default;
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (path.EndsWith($"/{name}.uss"))
					values.Item1 = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
				else if (path.EndsWith($"/{name}.uxml"))
					values.Item2 = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
			}
			return values;
		}
	}
}