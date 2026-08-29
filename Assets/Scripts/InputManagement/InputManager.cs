using UnityEngine;
using System;
using System.Threading.Tasks;
using Environment;

namespace Game
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] GameObject playButton;
        [SerializeField] GameObject playAgainButton;
        GameRenderer gameRenderer;
        bool Selecting
        {
            set
            {
                foreach (var col in columnHighlights)
                {
                    col.Selecting = value;
                }
            }
        }
        ColumnHighlight[] columnHighlights;
        TaskCompletionSource<int> _tcs = new();

        void Awake()
        {
            gameRenderer = FindFirstObjectByType<GameRenderer>();
            HidePlayPrompts();
        }

        void Start()
        {
            columnHighlights = FindObjectsByType<ColumnHighlight>(FindObjectsSortMode.None);
            ShowPlayPrompt();
        }

        public async Task<int> GetPlayerAction()
        {
            Selecting = true;
            gameRenderer.RenderMessage("Select column to place token in");
            int action = await _tcs.Task;
            Selecting = false;
            _tcs = new();
            return action;
        }

        public void ActionSelected(int action)
        {
            _tcs.SetResult(action);
        }

        public void UpdateValidColumns(Connect4 gameBoard)
        {
            foreach (var highlight in columnHighlights)
            {
                highlight.ValidColumn = gameBoard.ValidAction(highlight.Column);
            }
        }

        public void ShowPlayPrompt()
        {
            gameRenderer.RenderMessage("CoNNect-Four");
            playButton.SetActive(true);
        }

        public void ShowPlayAgainPrompt()
        {
            gameRenderer.RenderMessage("Play again?");
            playAgainButton.SetActive(true);
        }

        public void HidePlayPrompts()
        {
            playButton.SetActive(false);
            playAgainButton.SetActive(false);
        }
    }
}
