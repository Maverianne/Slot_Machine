using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance { get; private set; }
        
        public SlotManager SlotManager { get; private set; }

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
    }
}