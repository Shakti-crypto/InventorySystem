using UnityEngine;

namespace InventorySystem
{
    public class InputManager : MonoBehaviour
    {
        #region Singleton
        private static InputManager instance;
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(InputManager)) as InputManager;

                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject("InventorySystem_InputManager");
                        instance = gameObject.AddComponent<InputManager>();
                    }
                }

                return instance;

            }
        }
        #endregion

        #region Events
        public delegate void ReadMousePosition(Vector2 mousePosition);
        public event ReadMousePosition readMousePosition;

        public delegate void OnInventoryPressed();
        public event OnInventoryPressed onInventoryPressed;

        public delegate void OnInventoryClosed();
        public event OnInventoryClosed onInventoryClosed;

        public delegate void OnInteractionPressed();
        public event OnInteractionPressed onInteractionPressed;

        public delegate void OnQuickUseInventoryPressed(int slotIndex);
        public event OnQuickUseInventoryPressed onQuickUseInventoryPressed;

        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this);
                }
            }
        }
        private void Update()
        {
            _ReadMousePosition();

            if (Input.GetKeyDown(KeyCode.Tab)) _InventoryPressed();
            if (Input.GetKeyUp(KeyCode.Tab)) _InventoryClosed();
            if (Input.GetKeyDown(KeyCode.E)) _InteractionPressed();

            HandleQuickUseButtons();
        }
        private void _ReadMousePosition()
        {
            readMousePosition?.Invoke(Input.mousePosition);
        }
        private void _InventoryPressed()
        {
            onInventoryPressed?.Invoke();
        }
        private void _InventoryClosed()
        {
            onInventoryClosed?.Invoke();
        }
        private void _InteractionPressed()
        {
            onInteractionPressed?.Invoke();
        }
        private void HandleQuickUseButtons()
        {
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    onQuickUseInventoryPressed?.Invoke(i);
                    break;
                }
            }
        }
    }

}