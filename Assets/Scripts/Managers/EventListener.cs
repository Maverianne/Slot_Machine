using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class EventListener : MonoBehaviour
    {
        private Dictionary<string, UnityEvent> _eventDictionary;

        private static EventListener _eventManager;

        private static EventListener Instance
        {
            get
            {
                if (!ReferenceEquals(_eventManager, null)) return _eventManager;
                _eventManager = FindObjectOfType(typeof(EventListener)) as EventListener;

                if (!ReferenceEquals(_eventManager, null)) _eventManager.Initialize();
                return _eventManager;
            }
        }

        private void Initialize()
        {
            _eventDictionary ??= new Dictionary<string, UnityEvent>();
        }

        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Instance._eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction listener)
        {
            if (_eventManager == null) return;
            if (!Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) return;
            thisEvent.RemoveListener(listener);;
        }
    
        public static void ClearAllEvents () {
            Instance._eventDictionary.Clear();
        }
    
        public static void TriggerEvent (string eventName) {
            if (string.IsNullOrEmpty(eventName)) return;
			
            if (Instance._eventDictionary.TryGetValue (eventName, out var thisEvent)) {
                thisEvent.Invoke ();
            }
        }

    }
}
