using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Editor
{
    class PlayGame : EditorWindow
    {
        [MenuItem("Dynomask/Play Game")]
        public static void RunMainScene()
        {
            EditorApplication.OpenScene("Assets/Scenes/Game.unity");
            EditorApplication.isPlaying = true;
        }
    }
}