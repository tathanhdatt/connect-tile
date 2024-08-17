#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class EditorScene
{
#if UNITY_EDITOR
    [MenuItem("Scene/MainScene")]
    public static void OpenMainScene()
    {
        EditorSceneManager.OpenScene("Assets/_Scenes/GameScene.unity");
    }

    [MenuItem("Scene/Level")]
    public static void OpenLevelScene()
    {
        EditorSceneManager.OpenScene("Assets/_Scenes/Level.unity");
    }
#endif
}