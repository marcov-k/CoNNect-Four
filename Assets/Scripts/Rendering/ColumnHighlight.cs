using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ColumnHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] Color validColor;
        [SerializeField] Color invalidColor;
        Color highlightColor;
        Color defaultColor = Color.clear;
        SpriteRenderer myRenderer;
        public bool Selecting
        {
            get => _selecting;
            set
            {
                _selecting = value;
                myRenderer.color = value && Hovering ? highlightColor : defaultColor;
            }
        }
        bool _selecting = false;
        bool Hovering
        {
            get => _hovering;
            set
            {
                _hovering = value;
                myRenderer.color = value && Selecting ? highlightColor : defaultColor;
            }
        }
        bool _hovering = false;
        public bool ValidColumn
        {
            get => _validColumn;
            set
            {
                _validColumn = value;
                highlightColor = value ? validColor : invalidColor;
            }
        }
        bool _validColumn = true;
        InputManager inputManager;
        public int Column { get; private set; } = 0;

        void Awake()
        {
            myRenderer = GetComponent<SpriteRenderer>();
            inputManager = FindFirstObjectByType<InputManager>();
        }

        void Start()
        {
            highlightColor = validColor;
            myRenderer.color = defaultColor;
        }

        public void SetColumn(int col) => Column = col;

        public void OnPointerEnter(PointerEventData _) => Hovering = true;

        public void OnPointerExit(PointerEventData _) => Hovering = false;

        public void OnPointerClick(PointerEventData _)
        {
            if (Selecting && ValidColumn)
            {
                inputManager.ActionSelected(Column);
            }
        }
    }
}
