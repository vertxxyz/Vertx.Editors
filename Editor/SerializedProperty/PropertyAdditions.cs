using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vertx.Utilities.Editor;
using Object = UnityEngine.Object;

namespace Vertx.Editors
{
	internal static class PropertyAdditions
	{
		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
			EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
		}

		private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
		{
			Object targetObject = property.serializedObject!.targetObject;
			if (property.isArray)
			{
				if (property.propertyType != SerializedPropertyType.String)
					ArrayOperations();
			}

			if (property.serializedObject != null)
			{
				switch (property.serializedObject.targetObject)
				{
					case Transform _:
						TransformResets();
						break;
					case SkinnedMeshRenderer _:
						BlendShapes();
						break;
				}
			}
			
			switch (property.propertyType)
			{
				case SerializedPropertyType.LayerMask:
					LayerMask();
					break;
				case SerializedPropertyType.ObjectReference:
					ObjectReference();
					break;
			}

			void ArrayOperations()
			{
				// Array Reverse.
				if (property.arraySize <= 1)
					menu.AddDisabledItem(new GUIContent("Reverse"));
				else
				{
					var propertyCopy = property.Copy();
					menu.AddItem(new GUIContent("Reverse"), false, arg =>
					{
						var prop = (SerializedProperty)arg;
						prop.ReverseArray();
						prop.serializedObject.ApplyModifiedProperties();
					}, propertyCopy);
				}

				// Array Clear
				if (property.arraySize == 0)
					menu.AddDisabledItem(new GUIContent("Clear"));
				else
				{
					var propertyCopy = property.Copy();
					menu.AddItem(new GUIContent("Clear"), false, arg =>
					{
						var prop = (SerializedProperty)arg;
						prop.arraySize = 0;
						prop.serializedObject.ApplyModifiedProperties();
					}, propertyCopy);
				}

				if (!EditorUtility.IsPersistent(targetObject) && targetObject is Component component)
				{
					if (property.arrayElementType.StartsWith("PPtr<") && property.arraySize == 0)
					{
						EditorUtils.GetFieldInfoFromProperty(property, out Type type);
						Type elementType = type.GetElementType()!;
						if (elementType.IsSubclassOf(typeof(Component)))
						{
							menu.AddSeparator("");
							menu.AddItem(new GUIContent("Assign All"), false, () =>
							{
								Object[] results = Object.FindObjectsByType(elementType, FindObjectsInactive.Include, FindObjectsSortMode.None);
								Scene scene = component.gameObject.scene;
								foreach (Object result in results)
								{
									var c = (Component)result;
									if(c.gameObject.scene != scene)
										continue;
									SerializedProperty e = property.GetArrayElementAtIndex(property.arraySize++);
									e.objectReferenceValue = result;
								}

								property.serializedObject.ApplyModifiedProperties();
							});
						}
					}
				}
			}

			void TransformResets()
			{
				switch (property.propertyPath)
				{
					case "m_LocalPosition":
						if (property.vector3Value == Vector3.zero)
							menu.AddDisabledItem(new GUIContent("Reset"));
						else
						{
							menu.AddItem(new GUIContent("Reset"), false, () =>
							{
								property.vector3Value = Vector3.zero;
								property.serializedObject.ApplyModifiedProperties();
							});
						}

						break;
					case "m_LocalRotation":
						if (property.quaternionValue == Quaternion.identity)
							menu.AddDisabledItem(new GUIContent("Reset"));
						else
						{
							menu.AddItem(new GUIContent("Reset"), false, () =>
							{
								property.quaternionValue = Quaternion.identity;
								property.serializedObject.ApplyModifiedProperties();
							});
						}

						break;
					case "m_LocalScale":
						if (property.vector3Value == Vector3.one)
							menu.AddDisabledItem(new GUIContent("Reset"));
						else
						{
							menu.AddItem(new GUIContent("Reset"), false, () =>
							{
								property.vector3Value = Vector3.one;
								property.serializedObject.ApplyModifiedProperties();
							});
						}

						break;
				}
			}

			void BlendShapes()
			{
				if (!property.propertyPath.StartsWith("m_BlendShapeWeights.Array."))
					return;
				
				menu.AddItem(new GUIContent("Copy Index"), false, () => EditorGUIUtility.systemCopyBuffer = GetIndex().ToString());

				menu.AddItem(new GUIContent("Copy Name"), false, () =>
				{
					int index = GetIndex();
					EditorGUIUtility.systemCopyBuffer = ((SkinnedMeshRenderer)property.serializedObject.targetObject).sharedMesh.GetBlendShapeName(index);
				});

				int GetIndex()
				{
					// "m_BlendShapeWeights.Array.data[...]"
					int startIndex = "m_BlendShapeWeights.Array.data[".Length;
					int endIndex = property.propertyPath.Length - 1;
					return int.Parse(property.propertyPath.Substring(startIndex, endIndex - startIndex));
				}
			}

			void LayerMask()
			{
				menu.AddSeparator("");

				menu.AddItem(new GUIContent("Edit Layers"), false, () =>
				{
					Type.GetType("UnityEditor.SettingsWindow,UnityEditor")?
						.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static)?
						.Invoke(null, new object[] { SettingsScope.Project, "Project/Tags and Layers" });
				});

				menu.AddItem(new GUIContent("Edit Layer Collision Matrix"), false, () =>
				{
					Type.GetType("UnityEditor.SettingsWindow,UnityEditor")?
						.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static)?
						.Invoke(null, new object[] { SettingsScope.Project, "Project/Physics" });
				});

#if PHYSICS_2D_MODULE
				menu.AddItem(new GUIContent("Edit Layer Collision Matrix 2D"), false, () =>
				{
					// Show the layer collision matrix tab
					EditorPrefs.SetBool("UnityEditor.U2D.Physics/GeneralSettingsSelected", false);
					Type.GetType("UnityEditor.SettingsWindow,UnityEditor")?
						.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static)?
						.Invoke(null, new object[] { SettingsScope.Project, "Project/Physics 2D" });
				});
#endif
			}

			void ObjectReference()
			{
				if (!EditorUtility.IsPersistent(targetObject) && targetObject is Component component)
				{
					if (property.objectReferenceValue == null && property.type.StartsWith("PPtr<"))
					{
						NullComponentPropertyAdditions(menu, property, component);
					}
				}
			}
		}

		private static void NullComponentPropertyAdditions(GenericMenu menu, SerializedProperty property, Component component)
		{
			Scene scene = component.gameObject.scene;
			EditorUtils.GetFieldInfoFromProperty(property, out Type type);
			if (!type.IsSubclassOf(typeof(Component)))
				return;
			
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Assign/Find First"), false, () =>
			{
				GameObject[] gameObjects = scene.GetRootGameObjects();
				foreach (GameObject gameObject in gameObjects)
				{
					Component result = gameObject.GetComponentInChildren(type, true);
					if (result == null) continue;
					property.objectReferenceValue = result;
					property.serializedObject.ApplyModifiedProperties();
					return;
				}
			});

			menu.AddItem(new GUIContent("Assign/Create Empty"), false, () =>
			{
				var gameObject = new GameObject(type.Name, type);
				Undo.RegisterCreatedObjectUndo(gameObject, "Created Empty");
				property.objectReferenceValue = gameObject.GetComponent(type);
				property.serializedObject.ApplyModifiedProperties();
			});
		}
	}
}