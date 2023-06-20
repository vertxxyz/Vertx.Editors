using System.IO;
using UnityEditor;
using UnityEngine;

namespace Vertx.Editors
{
	internal static class ExportTextureMenu
	{
		[MenuItem("CONTEXT/Texture2D/Export as PNG")]
		public static void ExportTexture(MenuCommand ctx)
		{
			if (!(ctx.context is Texture2D texture2D))
			{
				Debug.LogWarning($"Texture is not {nameof(Texture2D)}.");
				return;
			}

			Texture2D toDispose = null;
			if (!texture2D.isReadable)
			{
				RenderTexture tmp = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0, texture2D.graphicsFormat, 1);
				Graphics.Blit(texture2D, tmp);
				RenderTexture previous = RenderTexture.active;
				RenderTexture.active = tmp;
				Texture2D newTex = new Texture2D(texture2D.width, texture2D.height) { name = texture2D.name };
				newTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
				newTex.Apply();
				RenderTexture.active = previous;
				RenderTexture.ReleaseTemporary(tmp);
				texture2D = newTex;
				toDispose = newTex;
			}

			string path = EditorUtility.SaveFilePanel("Export as PNG", "", texture2D.name, "png");
			if (string.IsNullOrEmpty(path))
				return;

			File.WriteAllBytes(path, texture2D.EncodeToPNG());
			if (toDispose != null)
				Object.DestroyImmediate(toDispose);
		}
	}
}