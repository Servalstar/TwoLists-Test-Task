using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Services
{
    public class UIRaycaster
    {
        private readonly EventSystem _eventSystem;
        private PointerEventData _pointerEventData;

        public UIRaycaster(EventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _pointerEventData = new PointerEventData(eventSystem);
        }

        public void GetHits(Vector3 position, List<RaycastResult> results)
        {
            _pointerEventData.position = position;
            _eventSystem.RaycastAll(_pointerEventData, results);
        }
    }
}