# Vertx.Editors
Custom Editors and Controls for Unity

## Inspectors
### ScriptableObject Inspector
Adds Select and Search buttons to the header.  
Custom inspectors for ScriptableObject types should inherit from this type.

### Animator Inspector
Adds the Avatar bone selection interface to the inspector for humanoid Animators.

### Script Inspector
Adds a Search button to the header for ScriptableObject and MonoBehaviour scripts.

## Utilities
### StyleUtils
Helper functions for UIToolkit/UIElements.
- `GetStyleSheet`
- `GetUXML`
- `GetStyleSheetAndUXML`

## Windows
### Prefab Search
Used by the [Script Inspector](#script-inspector) to search for prefabs containing a certain component.

## Installation

<details>
<summary>Add from OpenUPM <em>| via scoped registry, recommended</em></summary>

This package is available on OpenUPM: https://openupm.com/packages/com.vertx.editors

To add it the package to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.vertx
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.vertx.editors`
- click <kbd>Add</kbd>
</details>

<details>
<summary>Add from GitHub | <em>not recommended, no updates through UPM</em></summary>

You can also add it directly from GitHub on Unity 2019.4+. Note that you won't be able to receive updates through Package Manager this way, you'll have to update manually.

- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `https://github.com/vertxxyz/Vertx.Editors.git`
- click <kbd>Add</kbd>  
  **or**
- Edit your `manifest.json` file to contain `"com.vertx.editors": "https://github.com/vertxxyz/Vertx.Editors.git"`,

⚠️ Editors has a dependency on [Utilities](https://github.com/vertxxyz/Vertx.Utilities) so ensure that is referenced into your project to use this package successfully. ⚠️  

To update the package with new changes, remove the lock from the `packages-lock.json` file.
</details>