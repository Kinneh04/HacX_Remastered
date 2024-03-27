using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseLayerScroll : MonoBehaviour
{
    public static MouseLayerScroll Instance { get; private set; }
    public int UILayer = 5;
    private void Update()
    {
       // print(IsPointerOverUIElement() ? "Over UI" : "Not over UI");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensures there is only one instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Keep instance alive across scenes
    }

    public bool IsPointerOverUIElement()
    {

        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    // Gets all event system raycast results of current mouse or touch position.
    private static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    // Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        foreach (RaycastResult curRaysastResult in eventSystemRaysastResults)
        {
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }
}
