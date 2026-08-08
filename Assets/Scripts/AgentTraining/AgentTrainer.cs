using NNNCSharp.Components.Models;
using NNNCSharp.Components.Models.Layers;
using NNNCSharp.Components.Trainers;
using NNNCSharp.Components.Utilities.SaveSystem;
using UnityEngine;
using System;
using System.IO;

namespace AgentTraining
{
    public class AgentTrainer : MonoBehaviour
    {
        [Header("Training Settings")]

        [Space()]

        [Header("Episode Settings")]
        [SerializeField] int episodes = 1000;
        [SerializeField] int minExperiences = 2000;
        [SerializeField] float explorationDecay = 0.999f;

        [Space()]

        [Header("Replay Buffer Settings")]
        [SerializeField] int replayBufferSize = 20000;
        [SerializeField] int batchSize = 128;

        [Space()]

        [Header("Self-Play Settings")]
        [SerializeField] int agentBufferSize = 4;
        [SerializeField] int opponentCopyRate = 600;
        [SerializeField] int minRandomOpponentEpisodes = 600;

        [Space()]

        [Header("Parameter Update Settings")]
        [SerializeField] float discount = 0.99f;
        [SerializeField] float tau = 0.01f;
        [SerializeField] float maxGradNorm = 1.0f;

        [Space()]

        [Header("Evaluation Settings")]
        [SerializeField] int testEpisodes = 2000;

        [Header("Saving/Loading Settings")]
        [SerializeField] string agentSaveDirectory = "Agents";
        [SerializeField] string agentSaveName = "NewAgent";

        Model agent;

        public void InitializeAgent()
        {
            
        }

        public void LoadAgent()
        {
            Saver.DirectoryPath = Path.Combine(Application.streamingAssetsPath, agentSaveDirectory);
            agent = Saver.LoadModel(agentSaveName);
        }

        public void TrainAgent()
        {

        }

        public string GetAgentSaveDirectory()
        {
            return agentSaveDirectory;
        }
    }
}
