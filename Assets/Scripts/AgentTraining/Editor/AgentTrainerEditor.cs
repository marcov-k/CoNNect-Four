using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

namespace AgentTraining
{
    [CustomEditor(typeof(AgentTrainer))]
    public class AgentTrainerEditor : Editor
    {
        bool Training => trainingTask != null && !trainingTask.IsCompleted;
        Task trainingTask = null;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AgentTrainer trainer = (AgentTrainer)target;

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Initialize New Agent"))
            {
                if (!Training)
                {
                    trainer.InitializeAgent();
                }
                else
                {
                    Debug.Log("Cannot initialize new agent - agent training in progress.\n");
                }
            }

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Load Existing Agent"))
            {
                if (!Training)
                {
                    trainer.LoadAgent();
                }
                else
                {
                    Debug.Log("Cannot load existing agent - agent training in progress.\n");
                }
            }

            EditorGUILayout.Space(3);

            if (GUILayout.Button("Train Loaded Agent"))
            {
                if (!Training)
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
                if (Training)
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