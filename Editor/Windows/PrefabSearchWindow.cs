using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Vertx.Utilities.Editor;

namespace Vertx.Editors.Editor
{
	public class PrefabSearchWindow : EditorWindow
	{
		public enum SearchType
		{
			SurfaceLevel,
			Deep
		}

		public SearchType QueryType { get; private set; }
		public Type SearchingType { get; private set; }
		[SerializeField] private string searchingTypeString;

		private bool deepFallbackAllowed = true;

		public static void Open(Type searchingType, SearchType searchType = SearchType.SurfaceLevel)
		{
			var window = GetWindow<PrefabSearchWindow>();
			window.titleContent = new GUIContent("Prefab Search");
			window.deepFallbackAllowed = true;
			window.StartSearch(searchingType, searchType);
			window.Show();
		}

		private ListView listView;
		private Label typeLabel;
		private HelpBox helpBox;
		private EnumField queryTypeField;
		[SerializeField] private List<Component> results = new List<Component>();
		[SerializeField] private GameObject[] prefabs;
		private int searchIndex;
		private const int countPerUpdate = 20;

		private void OnEnable()
		{
			var root = rootVisualElement;
			VisualElement padding = new VisualElement
			{
				style =
				{
					paddingBottom = 10,
					paddingLeft = 10,
					paddingRight = 10,
					paddingTop = 10
				}
			};

			root.Add(padding);
			padding.StretchToParentSize();
			root = padding;

			typeLabel = new Label
			{
				style = {unityFontStyleAndWeight = FontStyle.Bold}
			};
			root.Add(typeLabel);

			queryTypeField = new EnumField(QueryType)
			{
				label = "Query Type"
			};
			queryTypeField.RegisterValueChangedCallback(evt =>
			{
				deepFallbackAllowed = false;
				ReSearch((SearchType) evt.newValue);
			});
			root.Add(queryTypeField);

			listView = new ListView(results,
				(int) EditorGUIUtils.HeightWithSpacing,
				() =>
				{
					var objectField = new ObjectField
					{
						style = {maxHeight = EditorGUIUtility.singleLineHeight}
					};
					objectField.Q(className: ObjectField.selectorUssClassName).style.display = DisplayStyle.None;
					objectField.RegisterValueChangedCallback(evt => objectField.SetValueWithoutNotify(evt.previousValue));
					return objectField;
				},
				(element, i) =>
				{
					var button = (ObjectField) element;
					Component component = results[i];
					button.objectType = typeof(GameObject);
					button.SetValueWithoutNotify(component.transform.root.gameObject);
				}
			)
			{
				style =
				{
					flexGrow = 1,
					backgroundColor = new Color(0, 0, 0, 0.15f),
					marginTop = 5
				}
			};
			root.Add(helpBox = new HelpBox("No Results", HelpBoxMessageType.Info)
			{
				style =
				{
					marginBottom = 5,
					marginLeft = 5,
					marginRight = 5,
					marginTop = 5
				}
			});
			if (results.Count != 0)
				HideHelpBox();
			root.Add(listView);
		}

		private void ReSearch(SearchType queryType)
		{
			if (SearchingType == null)
				SearchingType = Type.GetType(searchingTypeString);
			if (SearchingType == null)
			{
				Debug.LogWarning("Component type used in search has been lost. Please start a new search.");
				return;
			}

			StartSearch(SearchingType, queryType);
		}

		public void StartSearch(Type searchingType, SearchType queryType)
		{
			if (searchingType == null)
			{
				Debug.LogWarning("Component type used in search has is null. Please start a new search.");
				return;
			}

			SearchingType = searchingType;
			typeLabel.text = searchingType.Name;
			searchingTypeString = SearchingType.AssemblyQualifiedName;
			QueryType = queryType;
			results.Clear();
			ShowHelpBox();
			listView.Refresh();
			queryTypeField.SetValueWithoutNotify(QueryType);

			prefabs = EditorUtils.LoadAssetsOfType<GameObject>();
			searchIndex = 0;
			EditorApplication.update += DoSearch;
		}

		private void DoSearch()
		{
			bool result = false;
			try
			{
				for (int g = 0; g < countPerUpdate; g++)
				{
					int i = searchIndex + g;
					if (i >= prefabs.Length)
					{
						EditorApplication.update -= DoSearch;
						if (deepFallbackAllowed && results.Count == 0 && QueryType != SearchType.Deep)
							ReSearch(SearchType.Deep);
						return;
					}

					switch (QueryType)
					{
						case SearchType.SurfaceLevel:
						{
							if (prefabs[i].TryGetComponent(SearchingType, out var component))
							{
								results.Add(component);
								result = true;
							}

							break;
						}
						case SearchType.Deep:
						{
							var component = prefabs[i].GetComponentInChildren(SearchingType);
							if (component != null)
							{
								results.Add(component);
								result = true;
							}

							break;
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				searchIndex += countPerUpdate;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				EditorApplication.update -= DoSearch;
			}
			finally
			{
				if (result)
				{
					HideHelpBox();
					listView.Refresh();
					Repaint();
				}
			}
		}

		void HideHelpBox()
		{
			helpBox.visible = false;
			helpBox.style.display = DisplayStyle.None;
		}

		void ShowHelpBox()
		{
			helpBox.visible = true;
			helpBox.style.display = DisplayStyle.Flex;
		}
	}
}