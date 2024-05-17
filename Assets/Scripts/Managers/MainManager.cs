using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance { get; private set; }

        public SlotManager SlotManager { get; private set; }

        private bool _isExiting;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            SlotManager = FindObjectOfType<SlotManager>();
        }

        
        public void ExitGame(InputAction.CallbackContext context)
        {
            ExitGame();
        }

        public void ExitGame()
        {
            if(_isExiting) return;
            _isExiting = true;
            SlotManager.RegisterNotifications(false);
            EventListener.ClearAllEvents();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}