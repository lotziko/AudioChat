using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Tools
{
    [ExecuteInEditMode]
    public class PlayFromScene : EditorWindow
    {
        bool m_WasLaunched = false;
        bool m_IsLaunchFromThisScript = false;

        [SerializeField] string m_WorkScene = "";
        [SerializeField] int m_WorkSceneIndex = 0;
        [SerializeField] string m_LaunchScene = "";
        [SerializeField] int m_LaunchSceneIndex = 0;

        [MenuItem("Tools/Play From Scene %F5")]
        public static void Run()
        { EditorWindow.GetWindow<PlayFromScene>(); }

        static string[] m_SceneNames;
        static EditorBuildSettingsScene[] m_Scenes;

        void OnEnable()
        {
            m_Scenes = EditorBuildSettings.scenes;

            List<string> sceneNames = new List<string>();
            foreach (string scene in m_Scenes.Select(x => AsSpacedCamelCase(Path.GetFileNameWithoutExtension(x.path))))
                sceneNames.Add(scene);

            m_SceneNames = sceneNames.ToArray();
        }

        void Update()
        {
            if (EditorApplication.isPlaying)
                m_WasLaunched = true;

            if (!EditorApplication.isPlaying && m_IsLaunchFromThisScript && m_WasLaunched)
            {
                m_IsLaunchFromThisScript = false;
                m_WasLaunched = false;

#pragma warning disable CS0618 // Type or member is obsolete
                if (EditorApplication.currentScene != m_WorkScene)
                    EditorApplication.OpenScene(m_WorkScene);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            Repaint();
        }

        void OnGUI()
        {
            if (EditorApplication.isPlaying)
                return;

            if (m_SceneNames == null)
                return;

            m_WorkSceneIndex = EditorGUILayout.Popup("Work scene", m_WorkSceneIndex, m_SceneNames);
            m_WorkScene = m_Scenes[m_WorkSceneIndex].path;

            if (GUILayout.Button("Go to scene"))
            {
#pragma warning disable CS0618 // Type or member is obsolete
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
                EditorApplication.OpenScene(m_WorkScene);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            GUILayout.Space(20.0f);

            m_LaunchSceneIndex = EditorGUILayout.Popup("Launch scene", m_LaunchSceneIndex, m_SceneNames);
            m_LaunchScene = m_Scenes[m_LaunchSceneIndex].path;

            if (GUILayout.Button("Play"))
            {
                m_IsLaunchFromThisScript = true;
                m_WasLaunched = false;
#pragma warning disable CS0618 // Type or member is obsolete
                EditorApplication.SaveCurrentSceneIfUserWantsTo();
                EditorApplication.OpenScene(m_LaunchScene);
#pragma warning restore CS0618 // Type or member is obsolete
                EditorApplication.isPlaying = true;
            }
        }

        public string AsSpacedCamelCase(string text)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length * 2);

            sb.Append(char.ToUpper(text[0]));

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                    sb.Append(' ');

                sb.Append(text[i]);
            }

            return sb.ToString();
        }
    }
}