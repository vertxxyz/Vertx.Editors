# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.5.0]
- Raised Vertx.Utilities dependency to 4.0.0
- Added component context menus: 
  - BoxCollider/Resize To Renderers In Children.
- Added Serialized Property context menus:
  - Individual Transform (Position, Rotation, Scale) Resets.
  - LayerMask
    - Edit Layers
    - Edit Layer Collision Matrix (2D)
  - ObjectField (when empty): Assign
    - Find First Component, also affects empty Component arrays.
    - Create Empty.
- Fixed issue where Clear and Reverse menu items would unintentionally appear for string fields (Tag field was a surprise).
- Added Copy GUID and FileID from selected asset shortcut (Ctrl+Alt+Shift+I)
- Added Export to PNG context menu to Texture2D headers.

## [1.4.1]
- Added shortcut to clear the console window (Ctrl+Alt+Shift+C)
- Fixed rare InvalidCastException.

## [1.4.0] - 2023-02-20
- Added Edit/Gizmos menu to mass-disable gizmo icons.

## [1.3.1] - 2022-08-28
- Added UIToolkit support for 2022.2+ for ScriptableObjectInspector.
- Added future-proofed UIToolkit support for ScriptInspector.
- Added HelpBox fallback for <2020.1.

## [1.3.0] - 2021-10-22
- Added Copy Name and Copy Index to blend shape properties.
- Added Clear to the SerializedProperty array context menu.

## [1.2.2] - 2021-10-22
- Added support for 2021.1+.

## [1.2.1] - 2021-07-29
- Added Reverse to the SerializedProperty array context menu.

## [1.2.0] - 2020-11-06
- Moved StyleUtils and HelpBox to the Vertx.Utilities package. This also changes their namespace.

## [1.1.2] - 2020-10-19
- Fixed compatibility with 2019

## [1.1.1] - 2020-10-10
- Added active scene search to prefab search window.

## [1.1.0] - 2020-10-03
- Update to stay inline with Vertx.Utilities changes.
- StyleExtensions has been renamed to StyleUtils to match other Utility/Extension classes across these packages.
- Added Select and Search to MonoScript. Added a prefab search window that is used by searches with MonoBehaviour scripts.

## [1.0.0]
- Initial release.