using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vertx.Utilities.Editor;

namespace Vertx.Editors.Editor
{
	/// <summary>
	/// Animator inspector that makes it easy to select bones under a humanoid avatar.
	/// </summary>
	[CanEditMultipleObjects, CustomEditor(typeof(Animator))]
	public class AnimatorInspector : UnityEditor.Editor
	{
		private UnityEditor.Editor defaultEditor;

		private SerializedProperty avatar;

		//Whether the avatar UI should be shown
		private bool showAvatar;

		private static readonly GUIContent bodyLabel = new GUIContent("Body");

		private void OnEnable()
		{
			if (defaultEditor == null)
				defaultEditor = CreateEditor(targets, Type.GetType("UnityEditor.AnimatorInspector, UnityEditor"));

			avatar = serializedObject.FindProperty("m_Avatar");
			Animator animator = (Animator) target;


			if (avatar.objectReferenceValue != null)
				bones = GetBoneArray(animator);
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static BoneWrapper[] GetBoneArray(Animator animator)
		{
			string[] boneName = HumanTrait.BoneName;
			IEnumerable<HumanBodyBones> humanBodyBones = Enum.GetValues(typeof(HumanBodyBones)).Cast<HumanBodyBones>();
			BoneWrapper[] bones = new BoneWrapper[boneName.Length];
			int i = 0;
			foreach (HumanBodyBones humanBodyBone in humanBodyBones)
			{
				if (humanBodyBone == HumanBodyBones.LastBone)
					break;
				bones[i] = new BoneWrapper(boneName[i], animator == null ? null : animator.GetBoneTransform(humanBodyBone));
				i++;
			}

			return bones;
		}

		//Repaint if the body picker is open.
		public override bool RequiresConstantRepaint() => showAvatar;

		private void OnDisable()
		{
			if (defaultEditor == null) return;
			DestroyImmediate(defaultEditor);
		}

		public override void OnInspectorGUI()
		{
			defaultEditor.OnInspectorGUI();

			if (DrawAvatarFoldout(avatar, bones, ref showAvatar))
			{
				if (bones == null || bones.Length == 0)
					bones = GetBoneArray((Animator) target);
			}
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static bool DrawAvatarFoldout(SerializedProperty avatar, BoneWrapper[] bones, ref bool showBodyPicker, bool forceDrawing = false)
		{
			if (!forceDrawing)
			{
				if (avatar.objectReferenceValue == null)
					return false;
				if (!((Avatar) avatar.objectReferenceValue).isHuman)
					return false;
			}

			EditorGUIUtils.DrawSplitter();
			if (EditorGUIUtils.DrawHeaderWithFoldout(bodyLabel, showBodyPicker))
				showBodyPicker = !showBodyPicker;
			if (showBodyPicker)
			{
				using (new EditorGUILayout.VerticalScope("TE NodeBackground"))
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						GUILayout.FlexibleSpace();
						Rect rect = GUILayoutUtility.GetRect(Silhouettes[bodyView], GUIStyle.none, GUILayout.MaxWidth(Silhouettes[bodyView].image.width));
						DrawBodyParts(rect, bodyView);
						bool hasHover = false;
						if (bones != null)
						{
							for (int i = 0; i < bones.Length; i++)
								hasHover |= DrawBone(bodyView, i, rect, bones[i], hasHover);
						}

						GUILayout.FlexibleSpace();
					}

					Rect lastRect = GUILayoutUtility.GetLastRect();
					lastRect.x += 5f;
					lastRect.width = 80f;
					lastRect.yMin = lastRect.yMax - 69f;
					lastRect.height = 16f;
					for (int j = 0; j < toggleStrings.Length; j++)
					{
						if (GUI.Toggle(lastRect, bodyView == j, toggleStrings[j], EditorStyles.miniButton))
							bodyView = j;
						lastRect.y += 16f;
					}
				}
			}

			EditorGUIUtils.DrawSplitter();
			return true;
		}

		#region BodyPart
		
		private static bool DrawBone(int shownBodyView, int i, Rect rect, BoneWrapper bone, bool hasHover)
		{
			if (BonePositions[shownBodyView, i] == Vector2.zero)
				return false;
			Vector2 b = BonePositions[shownBodyView, i];
			b.y *= -1f;
			b.Scale(new Vector2(rect.width * 0.5f, rect.height * 0.5f));
			b = rect.center + b;
			const int num = 19;
			Rect rect2 = new Rect(b.x - num * 0.5f, b.y - num * 0.5f, num, num);
			//Returns true if this is being hovered
			return bone.BoneDotGUI(rect2, rect2, new Rect(25, rect.y + 6, 150, 20), i, hasHover);
		}

		private BoneWrapper[] bones;

		public class BoneWrapper
		{
			private readonly string boneName;
			private readonly Transform bone;
			private BoneState state;

			public BoneWrapper(string boneName, Transform bone)
			{
				this.boneName = ObjectNames.NicifyVariableName(boneName);
				this.bone = bone;
				state = bone == null ? BoneState.None : BoneState.Valid;
			}

			private static readonly Color kBoneValid = new Color(0f, 0.75f, 0f, 1f);
			private static readonly Color kBoneInvalid = new Color(1f, 0.3f, 0.25f, 1f);
			private static readonly Color kBoneInactive = Color.gray;
			private static readonly Color kBoneSelected = new Color(0.4f, 0.7f, 1f, 1f);
			
			/// <summary>
			/// Draws the GUI for a single bone dot.
			/// </summary>
			/// <returns>Returns true if this is being hovered</returns>
			public bool BoneDotGUI(Rect rect, Rect selectRect, Rect tooltipRect, int boneIndex, bool hasHover = false)
			{
				bool hover = false;
				Event current = Event.current;
				Color color = GUI.color;

				if (!hasHover)
				{
					if (selectRect.Contains(current.mousePosition))
					{
						if (state == BoneState.Valid)
						{
							if (current.type == EventType.MouseDown)
							{
								if (bone == null)
									state = BoneState.None;
								else
								{
									Selection.activeTransform = bone;
									if (bone != null)
										EditorGUIUtility.PingObject(bone);
								}

								current.Use();
							}
							else
							{
								GUI.color = kBoneSelected;
								GUI.DrawTexture(rect, DotSelection.image);
							}
						}

						GUI.color = color;
						GUI.Box(tooltipRect, boneName, EditorStyles.whiteMiniLabel);
						hover = true;
					}
				}

				switch (state)
				{
					case BoneState.Valid:
						GUI.color = kBoneValid;
						break;
					case BoneState.None:
						GUI.color = kBoneInactive;
						break;
					default:
						GUI.color = kBoneInvalid;
						break;
				}

				Texture image = HumanTrait.RequiredBone(boneIndex) ? DotFrame.image : DotFrameDotted.image;

				GUI.DrawTexture(rect, image);
				if (bone != null)
					GUI.DrawTexture(rect, DotFill.image);
				GUI.color = color;
				return hover;
			}
		}

		private static void DrawBodyParts(Rect rect, int shownBodyView)
		{
			GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			if (Silhouettes[shownBodyView] != null)
				GUI.DrawTexture(rect, Silhouettes[shownBodyView].image);
			for (int i = 1; i < 9; i++)
				DrawBodyPart(shownBodyView, i, rect);
		}

		private static void DrawBodyPart(int shownBodyView, int i, Rect rect)
		{
			if (BodyPart[shownBodyView, i] == null || BodyPart[shownBodyView, i].image == null)
				return;
			GUI.color = new Color(0.4f, 0.4f, 0.4f);
			GUI.DrawTexture(rect, BodyPart[shownBodyView, i].image);
			GUI.color = Color.white;
		}

		#endregion

		private static int bodyView;

		private static readonly string[] toggleStrings =
		{
			"Body",
			"Head",
			"Left Hand",
			"Right Hand"
		};

		#region AvatarControl

		private static Type avatarControlType;
		private static Type AvatarControlType => avatarControlType ?? (avatarControlType = Type.GetType("UnityEditor.AvatarControl, UnityEditor"));

		private static FieldInfo bonePositionsFI;

		private static FieldInfo BonePositionsFI =>
			bonePositionsFI ?? (bonePositionsFI = AvatarControlType.GetField("s_BonePositions", BindingFlags.Static | BindingFlags.NonPublic));

		private static Vector2[,] bonePositions;
		private static Vector2[,] BonePositions => bonePositions ?? (bonePositions = (Vector2[,]) BonePositionsFI.GetValue(null));

		private static PropertyInfo styles;
		private static PropertyInfo Styles => styles ?? (styles = AvatarControlType.GetProperty("styles", BindingFlags.Static | BindingFlags.NonPublic));

		private static Type _AvatarControlStylesType;
		private static Type AvatarControlStylesType => _AvatarControlStylesType ?? (_AvatarControlStylesType = Type.GetType("UnityEditor.AvatarControl+Styles, UnityEditor"));

		private static FieldInfo _SilhouettesFI;
		private static FieldInfo SilhouettesFI => _SilhouettesFI ?? (_SilhouettesFI = AvatarControlStylesType.GetField("Silhouettes", BindingFlags.Instance | BindingFlags.Public));

		private static FieldInfo bodyPartFI;
		private static FieldInfo BodyPartFI => bodyPartFI ?? (bodyPartFI = AvatarControlStylesType.GetField("BodyPart", BindingFlags.Instance | BindingFlags.Public));

		private static GUIContent[] silhouettes;
		private static GUIContent[] Silhouettes => silhouettes ?? (silhouettes = (GUIContent[]) SilhouettesFI.GetValue(Styles.GetValue(null)));

		private static GUIContent[,] bodyPart;
		private static GUIContent[,] BodyPart => bodyPart ?? (bodyPart = (GUIContent[,]) BodyPartFI.GetValue(Styles.GetValue(null)));

		private static GUIContent dotFill;
		private static GUIContent DotFill => dotFill ?? (dotFill = EditorGUIUtility.IconContent("AvatarInspector/DotFill"));

		private static GUIContent dotFrame;
		private static GUIContent DotFrame => dotFrame ?? (dotFrame = EditorGUIUtility.IconContent("AvatarInspector/DotFrame"));

		private static GUIContent dotFrameDotted;
		private static GUIContent DotFrameDotted => dotFrameDotted ?? (dotFrameDotted = EditorGUIUtility.IconContent("AvatarInspector/DotFrameDotted"));

		private static GUIContent dotSelection;
		private static GUIContent DotSelection => dotSelection ?? (dotSelection = EditorGUIUtility.IconContent("AvatarInspector/DotSelection"));

		#endregion
	}
}