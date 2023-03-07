using UnityEngine;
using UnityEditor;

public class CreateParent
{
    // Adding a new context menu item
    [MenuItem("GameObject/Create Parent", true)]
    static bool ValidateLogSelectedTransformName()
    {
        // disable menu item if no transform is selected.
        return Selection.activeTransform != null;
    }

    // Put menu item at top near other "Create" options
    [MenuItem("GameObject/Create Parent", false, 0)] //10
    private static void EmptyParent(MenuCommand menuCommand)
    {
        // Use selected item as our context (otherwise does nothing because of above)
        //GameObject selected = menuCommand.context as GameObject;
        GameObject selected = Selection.activeObject as GameObject;

        // Create a empty game object with same name
        GameObject go = new GameObject(selected.name);

        // adjust hierarchy accordingly
        GameObjectUtility.SetParentAndAlign(go, selected.transform.parent.gameObject);
        go.transform.position = selected.transform.position;
        go.transform.rotation = selected.transform.rotation;
        go.transform.localScale = selected.transform.localScale;
        GameObjectUtility.SetParentAndAlign(selected, go);

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Parented " + go.name);

        // Yea!
        Debug.Log("Created a Parent for " + selected.name + ".");
    }
}