using UnityEngine;
using NNNCSharp.Components.Utilities.SaveSystem;
using System.IO;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
        Saver.DirectoryPath = Path.Combine(Application.streamingAssetsPath, "Models");
    }
}
