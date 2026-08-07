using AgentTraining;
using NNNCSharp.Components.Utilities;
using NNNCSharp.Components.Utilities.SaveSystem;
using UnityEngine;

namespace Game
{
    public class Initializer : MonoBehaviour
    {
        void Awake()
        {
            NNNLog.Output = Debug.Log;
            Saver.DirectoryPath = FindFirstObjectByType<AgentTrainer>().GetAgentSaveDirectory();
        }
    }
}
