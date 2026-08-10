using Environment;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

namespace Game
{
    public class GameRenderer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] GameObject background;
        [SerializeField] GameObject token;
        [SerializeField] Color redColor;
        [SerializeField] Color yellowColor;
        [SerializeField] Color emptyColor;
        [SerializeField] GameObject highlight;
        Vector2 boardDims = new(6, 7); // x = rows, y = columns
        readonly SpriteRenderer[,] tokens = new SpriteRenderer[6, 7];

        void Awake()
        {
            HideMessage();
            SetUpBoard();
            SetUpHighlights();
        }

        void SetUpBoard()
        {
            var (bgWidth, bgHeight) = GetBackgroundSize();

            float tokenSize = token.GetComponent<Renderer>().bounds.size.x;

            background.transform.position = new(0, 0);

            float xStep = (bgWidth / boardDims.y) / 2.0f;
            float yStep = (bgHeight / boardDims.x) / 2.0f;
            float startX = -bgWidth / 2.0f;
            float xPos, yPos = bgHeight / 2.0f;
            for (int row = 0; row < boardDims.x; row++)
            {
                xPos = startX;
                yPos -= yStep;
                for (int col = 0; col < boardDims.y; col++)
                {
                    xPos += xStep;
                    tokens[row, col] = Instantiate(token, new Vector2(xPos, yPos), Quaternion.identity).GetComponent<SpriteRenderer>();
                    tokens[row, col].color = emptyColor;
                    xPos += xStep;
                }
                yPos -= yStep;
            }
        }

        void SetUpHighlights()
        {
            var (bgWidth, _) = GetBackgroundSize();

            float xStep = (bgWidth / boardDims.y) / 2.0f;
            float xPos = -bgWidth / 2.0f;
            for (int col = 0; col < boardDims.y; col++)
            {
                xPos += xStep;
                Instantiate(highlight, new Vector2(xPos, 0.0f), Quaternion.identity).GetComponent<ColumnHighlight>().SetColumn(col);
                xPos += xStep;
            }
        }

        (float width, float height) GetBackgroundSize()
        {
            var bgRenderer = background.GetComponent<Renderer>();
            return (bgRenderer.bounds.size.x, bgRenderer.bounds.size.y);
        }

        public void RenderState(Connect4 gameBoard)
        {
            using var board = gameBoard.GetBoard();
            for (int row = 0; row < boardDims.x; row++)
            {
                for (int col = 0; col < boardDims.y; col++)
                {
                    tokens[row, col].color = board[row, col] switch
                    {
                        1.0f => redColor,
                        -1.0f => yellowColor,
                        _ => emptyColor
                    };
                }
            }
        }

        public void RenderAction(int action, Connect4 gameBoard)
        {
            using var board = gameBoard.GetBoard();
            for (int row = 0; row < boardDims.x; row++)
            {
                if (board[row, action] != 0.0f)
                {
                    tokens[row, action].color = board[row, action] switch
                    {
                        1.0f => redColor,
                        -1.0f => yellowColor,
                        _ => emptyColor
                    };
                    break;
                }
            }
        }

        public void RenderMessage(string message)
        {
            messageText.text = message;
        }

        public async Task RenderMessage(string message, int duration)
        {
            messageText.text = message;
            if (duration > 0)
            {
                await Task.Delay(duration);
                HideMessage();
            }
        }

        public void HideMessage()
        {
            messageText.text = string.Empty;
        }
    }
}
