using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Octamino
{
    public class PointerHandler : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent OnPointerDown = new UnityEvent();

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) => OnPointerDown.Invoke();
    }
}

