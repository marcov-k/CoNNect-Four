using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ColumnHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] Color highlightColor;
        Color defaultColor = Color.clear;
        SpriteRenderer myRenderer;
        public bool Selecting
        {
            get => _selecting;
            set
            {
                _selecting = value;
                if (!value)
                {
                    myRenderer.color = defaultColor;
                }
            }
        }
        bool _selecting = false;
        InputManager inputManager;
        int column = 0;

        void Awake()
        {
            myRenderer = GetComponent<SpriteRenderer>();
            inputManager = FindFirstObjectByType<InputManager>();
        }

        void Start()
        {
            myRenderer.color = defaultColor;
        }

        public void SetColumn(int col)
        {
            column = col;
        }

        public void OnPointerEnter(PointerEventData _)
        {
            if (Selecting)
            {
                myRenderer.color = highlightColor;
            }
        }

        public void OnPointerExit(PointerEventData _)
        {
            myRenderer.color = defaultColor;
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (Selecting)
            {
                inputManager.ActionSelected(column);
            }
        }
    }
}
