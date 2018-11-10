using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class EventSystemManager : MonoBehaviour
{
    EventSystem eventSystem;

    void Awake()
    {
        eventSystem = GetComponent<EventSystem>();
        if (EventSystem.current != eventSystem)
        {
            Destroy(gameObject);
        }
    }
}