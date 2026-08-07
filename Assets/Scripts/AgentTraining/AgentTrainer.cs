using UnityEngine;
using NNNCSharp.Components.Models;
using NNNCSharp.Components.Models.Layers;
using NNNCSharp.Components.Trainers;
using NNNCSharp.Components.Utilities.SaveSystem;
using System;
using System.IO;

namespace AgentTraining
{
    public class AgentTrainer : MonoBehaviour
    {
        [Header("Training Settings")]
        [SerializeField] int episodes = 1000;

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
        }

        public void TrainAgent()
        {

        }
    }
}
