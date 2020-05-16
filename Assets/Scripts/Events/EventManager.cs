using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 *
 *  Developed largely with this Unity tutorial:
 *  https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa
 *
 **/
public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType (typeof (EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active Event Manager script on a gameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent newEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out newEvent))
        {
            newEvent.AddListener(listener);
        }
        else
        {
            newEvent = new UnityEvent();
            newEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, newEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        // A check in case we have destroyed the EventManager
        if (eventManager == null) return;

        UnityEvent dontFeelSoGoodEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out dontFeelSoGoodEvent))
        {
            dontFeelSoGoodEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent currEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out currEvent))
        {
            currEvent.Invoke();
        }
    }
}
