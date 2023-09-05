# Vertx.Editors
Custom Editors and Controls for Unity

## Inspectors
### ScriptableObject Inspector
Adds Select and Search buttons to the header.  
Custom inspectors for ScriptableObject types should inherit from this type.

### Script Inspector
Adds a Search button to the header for ScriptableObject and MonoBehaviour scripts.

> **Note**  
> When overriding these inspectors, as of 2022.2 UIToolkit is the default inspector.  
> If you use IMGUI, override `CreateInspectorGUI` and return `null`.

### Animator Inspector
Adds the Avatar bone selection interface to the inspector for humanoid Animators.

## Windows
### Prefab Search
Used by the [Script Inspector](#script-inspector) to search for prefabs containing a certain component.

## Serialized Property
Adds functionality to the property right-click menu.  
### Arrays
- Reverse
- Clear  
### Blend Shapes
- Copy Name
- Copy Index
### LayerMask
- Edit Layers
- Edit Layer Collision Matrix
- Edit Layer Collision Matrix 2D
### Object
- Assign First
- Create Empty

## Menu Items
### BoxCollider
- Resize To Renderers In Children
### Texture2D
- Export to PNG

## Gizmos
- Edit/Gizmos/
  - Disable All Icons
  - Enable All Icons
  - Disable Scripts Icons

## Shortcuts
- Clear Console (Ctrl+Alt+Shift+C)
- Copy GUID and FileID from selected asset (Ctrl+Alt+Shift+I)

## Installation

[![openupm](https://img.shields.io/npm/v/com.vertx.editors?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.vertx.editors/)

<table><tr><td>

#### Add the OpenUPM registry
1. Open `Edit/Project Settings/Package Manager`
1. Add a new Scoped Registry (or edit the existing OpenUPM entry):
   ```
   Name: OpenUPM
   URL:  https://package.openupm.com/
   Scope(s): com.vertx
   ```
1. **Save**

#### Add the package
1. Open the Package Manager via `Window/Package Manager`.
1. Select the <kbd>+</kbd> from the top left of the window.
1. Select **Add package by Name** or **Add package from Git URL**.
1. Enter `com.vertx.editors`.
1. Select **Add**.

</td></tr></table>

If you find this resource helpful:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Z8Z42ZYHB)