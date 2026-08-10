using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

namespace AgentTraining
{
    [CustomEditor(typeof(AgentTrainer))]
    public class AgentTrainerEditor : Editor
    {
        Task trainingTask = null;

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
                if (trainingTask == null || trainingTask.IsCompleted)
                {
                    trainingTask = Task.Run(trainer.TrainAgent).ContinueWith(t =>
                    {
                        if (t.IsFaulted) Debug.LogError($"Training failed: {t.Exception?.GetBaseException()}");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    Debug.Log("Agent training already in progress.\n");
                }
            }

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Stop Training"))
            {
                if (trainingTask != null && !trainingTask.IsCompleted)
                {
                    Debug.Log("Terminating training.\n");
                    trainer.StopTraining();
                }
                else
                {
                    Debug.Log("No training session to terminate.\n");
                }
            }
        }
    }
}