using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Vertx.Editors
{
	internal static class MenuItems
	{
		[MenuItem("CONTEXT/BoxCollider/Resize To Renderers In Children", priority = 2100)]
		private static void ResizeBoxColliderToRenderer(MenuCommand command)
		{
			var boxCollider = (BoxCollider)command.context;
			Renderer[] renderers = boxCollider.GetComponentsInChildren<Renderer>();
			List<Vector3> worldPoints = new List<Vector3>();
			foreach (Renderer renderer in renderers)
			{
#if UNITY_2021_1_OR_NEWER
				Bounds localBounds = renderer.localBounds;
#else
				Bounds localBounds;
				if (renderer is SkinnedMeshRenderer smr)
				{
					localBounds = smr.sharedMesh.bounds;
				}
				else
				{
					var meshFilter = renderer.GetComponent<MeshFilter>();
					if (meshFilter == null)
						return;
					localBounds = meshFilter.sharedMesh.bounds;
				}
#endif
				Transform t = renderer.transform;
				Vector3 min = localBounds.min;
				Vector3 max = localBounds.max;
				worldPoints.Add(t.TransformPoint(min));
				worldPoints.Add(t.TransformPoint(new Vector3(min.x, min.y, max.z)));
				worldPoints.Add(t.TransformPoint(new Vector3(min.x, max.y, min.z)));
				worldPoints.Add(t.TransformPoint(new Vector3(max.x, min.y, min.z)));
				worldPoints.Add(t.TransformPoint(new Vector3(max.x, max.y, min.z)));
				worldPoints.Add(t.TransformPoint(new Vector3(max.x, min.y, max.z)));
				worldPoints.Add(t.TransformPoint(new Vector3(min.x, max.y, max.z)));
				worldPoints.Add(t.TransformPoint(max));
			}

			if (worldPoints.Count == 0)
			{
				// No renderers to encapsulate
				return;
			}

			Transform transform = boxCollider.transform;
			Bounds bounds = new Bounds(transform.InverseTransformPoint(worldPoints[0]), Vector3.zero);
			for (int i = 1; i < worldPoints.Count; i++)
				bounds.Encapsulate(transform.InverseTransformPoint(worldPoints[i]));

			Undo.RecordObject(boxCollider, "Resized box collider to Renderer bounds");

			boxCollider.center = Round(bounds.center);
			boxCollider.size = Round(bounds.size);
		}
		
		private static Vector3 Round(Vector3 value, int decimals = 5)
			=> new Vector3((float)Math.Round(value.x, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round(value.y, decimals, MidpointRounding.AwayFromZero),
				(float)Math.Round(value.z, decimals, MidpointRounding.AwayFromZero)
			);
	}
}