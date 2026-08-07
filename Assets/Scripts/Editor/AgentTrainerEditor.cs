using UnityEditor;
using UnityEngine;

namespace AgentTraining
{
    [CustomEditor(typeof(AgentTrainer))]
    public class AgentTrainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AgentTrainer trainer = (AgentTrainer)target;

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Initialize New Agent"))
            {
                trainer.InitializeAgent();
            }

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Load Existing Agent"))
            {
                trainer.LoadAgent();
            }

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Train Loaded Agent"))
            {
                trainer.TrainAgent();
            }
        }
    }
}