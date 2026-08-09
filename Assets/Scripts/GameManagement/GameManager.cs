using AgentTraining;
using Environment;
using NNNCSharp.Components.Models;
using NNNCSharp.Components.Utilities.SaveSystem;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        InputManager inputManager;
        GameRenderer gameRenderer;
        Model agent;
        readonly Connect4 gameBoard = new();
        Task gameThread;

        void Awake()
        {
            inputManager = FindFirstObjectByType<InputManager>();
            gameRenderer = FindFirstObjectByType<GameRenderer>();
        }

        void Start()
        {
            agent = Saver.LoadModel(FindFirstObjectByType<AgentTrainer>().GetAgentSaveName());
            gameThread = RunGame();
        }

        async Task RunGame()
        {
            bool playerWon = false;
            bool agentWon = false;
            while (!gameBoard.BoardFilled())
            {
                gameRenderer.RenderState(gameBoard);
                bool agentActing = gameBoard.AgentTurn;

                int action = await GetAction();

                await Task.Run(() => gameRenderer.RenderAction(action, gameBoard));
                gameBoard.TakeAction(action);

                if (gameBoard.Won(action))
                {
                    if (agentActing) agentWon = true;
                    else playerWon = true;
                    break;
                }
            }

            string message;
            if (playerWon) message = "You Win!";
            else if (agentWon) message = "You Lose...";
            else message = "Tie";

            gameRenderer.RenderMessage(message);
            _ = PlayAgainPrompt();
        }

        async Task PlayAgainPrompt()
        {
            if (await Task.Run(inputManager.GetPlayAgain))
            {
                gameThread = RunGame();
            }
            else
            {
                Application.Quit();
            }
        }

        async Task<int> GetAction()
        {
            return gameBoard.AgentTurn ? gameBoard.GetAgentAction(agent) : await Task.Run(inputManager.GetPlayerAction);
        }
    }
}
