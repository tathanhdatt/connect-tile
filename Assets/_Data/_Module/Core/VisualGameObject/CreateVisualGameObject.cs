using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Core.VisualGameObject
{
    public static class CreateVisualGameObject
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/Box Collider 2D", false, -10000)]
        private static void CreateBoxCollider2D(MenuCommand menuCommand)
        {
            var go = new GameObject("Box Collider 2D")
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
            go.AddComponent<BoxCollider2D>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, go.GetInstanceID().ToString());
            Selection.activeObject = go;
        }
#endif
    }
}