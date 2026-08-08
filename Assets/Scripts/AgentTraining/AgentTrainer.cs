using Environment;
using NNNCSharp.Components.Episodes;
using NNNCSharp.Components.Models;
using NNNCSharp.Components.Models.Layers;
using NNNCSharp.Components.Trainers;
using NNNCSharp.Components.Utilities;
using NNNCSharp.Components.Utilities.SaveSystem;
using UnityEngine;
using System;
using System.IO;
using NNNCSharp.Components.Optimizers;
using NNNCSharp.Components.Costs;
using NNNCSharp.Components.Buffers;
using NNNCSharp.Components.Activations;
using System.Threading.Tasks;
using System.Threading;

namespace AgentTraining
{
    public class AgentTrainer : MonoBehaviour
    {
        [Header("Training Settings")]

        [Space()]

        [Header("Episode Settings")]
        [SerializeField] int episodes = 1000;
        [SerializeField] int minExperiences = 2000;
        [SerializeField] float startExploration = 1.0f;
        [SerializeField] float explorationDecay = 0.999f;
        [SerializeField] float minExploration = 0.001f;

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
        [SerializeField] int trainEvery = 1;
        [SerializeField] float learningRate = 0.001f;
        [SerializeField] float discount = 0.99f;
        [SerializeField] float tau = 0.005f;
        [SerializeField] float maxGradNorm = 1.0f;

        [Space()]

        [Header("Evaluation Settings")]
        [SerializeField] int testEvery = 200;
        [SerializeField] int testEpisodes = 2000;

        [Header("Saving/Loading Settings")]
        [SerializeField] string agentSaveDirectory = "Agents";
        [SerializeField] string agentSaveName = "NewAgent";

        Model agent;

        public void InitializeAgent()
        {
            Connect4 env = new();

            NNNLog.Output = Debug.Log;

            agent?.Dispose();
            agent = new(new Layer[]
            {
                new Dense(256, new LeakyReLU()),
                new Dense(256, new LeakyReLU()),
                new Dense(128, new LeakyReLU()),
                new Dense(env.ActionCount, new Linear())
            }, env.StateFormat);

            NNNLog.WriteLine("New agent initialized.");
        }

        public void LoadAgent()
        {
            NNNLog.Output = Debug.Log;

            Saver.DirectoryPath = GetAgentSaveDirectoryPath();
            try
            {
                var loadedAgent = Saver.LoadModel(agentSaveName);
                agent?.Dispose();
                agent = loadedAgent;
            }
            catch (Exception e)
            {
                NNNLog.WriteLine($"Could not load agent: {e.Message}");
            }

            NNNLog.WriteLine("Agent loaded.");
        }

        public async Task TrainAgent()
        {
            NNNLog.Output = Debug.Log;

            if (agent == null)
            {
                NNNLog.WriteLine("No agent loaded.");
                return;
            }

            Connect4 env = new();
            Optimizer optimizer = new Adam(learningRate);
            Cost cost = new Huber();

            DQNTrainer trainer = new(
                agent: agent,
                environment: env,
                optimizer: optimizer,
                cost: cost,
                trainEvery: trainEvery,
                discount: discount,
                exploration: startExploration,
                explorationDecay: explorationDecay,
                minExploration: minExploration,
                replayBufferSize: replayBufferSize,
                batchSize: batchSize,
                agentBufferSize: agentBufferSize,
                opponentCopyRate: opponentCopyRate,
                minRandomOpponentEpisodes: minRandomOpponentEpisodes,
                tau: tau,
                maxGradNorm: maxGradNorm,
                minExperiences: minExperiences);

            FIFOBuffer<Episode> _ = null;

            NNNLog.WriteLine("Beginning agent training.");
            trainer.Train(ref _, episodes, testEvery, testEpisodes);
            agent = trainer.Agent;
            NNNLog.WriteLine("Agent training finished.");

            Saver.DirectoryPath = GetAgentSaveDirectoryPath();
            Saver.SaveModel(agent, agentSaveName, "connect-four neural network player");
            NNNLog.WriteLine("Agent saved to file.");
        }

        public string GetAgentSaveDirectory()
        {
            return agentSaveDirectory;
        }

        public string GetAgentSaveDirectoryPath()
        {
            return Path.Combine(Application.streamingAssetsPath, agentSaveDirectory);
        }
    }
}
