# Vertx.Editors
Custom Editors and Controls for Unity

⚠️ Editors has a dependency on [Utilities](https://github.com/vertxxyz/Vertx.Utilities) so ensure that is referenced into your project to use this package successfully. ⚠️

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