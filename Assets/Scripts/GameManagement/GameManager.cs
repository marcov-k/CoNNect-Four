using AgentTraining;
using Environment;
using NNNCSharp.Components.Models;
using NNNCSharp.Components.Utilities.SaveSystem;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] int messageDuration = 2000;
        InputManager inputManager;
        GameRenderer gameRenderer;
        Model agent;
        readonly Connect4 gameBoard = new();

        void Awake()
        {
            inputManager = FindFirstObjectByType<InputManager>();
            gameRenderer = FindFirstObjectByType<GameRenderer>();
        }

        void Start()
        {
            agent = Saver.LoadModel(FindFirstObjectByType<AgentTrainer>().GetAgentSaveName());
        }

        async Task RunGame()
        {
            bool playerWon = false;
            bool agentWon = false;
            gameBoard.Reset();
            gameRenderer.RenderState(gameBoard);
            string message = "You are playing " + (gameBoard.AgentTurn ? "Yellow and going second" : "Red and going first");
            await gameRenderer.RenderMessage(message, messageDuration);
            while (!gameBoard.BoardFilled())
            {
                bool agentActing = gameBoard.AgentTurn;

                int action = await GetAction();

                gameBoard.TakeAction(action);
                gameRenderer.RenderAction(action, gameBoard);

                if (gameBoard.Won(action))
                {
                    if (agentActing) agentWon = true;
                    else playerWon = true;
                    break;
                }
            }

            if (playerWon) message = "You Win!";
            else if (agentWon) message = "You Lose...";
            else message = "Tie";

            await gameRenderer.RenderMessage(message, messageDuration);
            inputManager.ShowPlayAgainPrompt();
        }

        public void Play()
        {
            inputManager.HidePlayPrompts();
            _ = RunGame();
        }

        public void Quit()
        {
            Application.Quit();
        }

        async Task<int> GetAction()
        {
            return gameBoard.AgentTurn ? gameBoard.GetAgentAction(agent) : await inputManager.GetPlayerAction();
        }
    }
}
