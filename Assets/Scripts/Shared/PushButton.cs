using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Octamino
{
    public class PushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // The event triggered after user pushes the button.
        public UnityEvent OnPush = new UnityEvent();
        
        // The delay in seconds required to register the 'Push and hold' event. 
        public float PushAndHoldDelay = Constant.Input.KeyRepeatDelay;
        
        // The event triggered after the user pushes and holds the button.
        public UnityEvent OnPushAndHold = new UnityEvent();
        
        // If set to true, the 'Push and hold' event will repeatedly fire with specified interval.
        public bool RepeatPushAndHold = true;
        
        // The interval in seconds in which 'Push and hold' event repeatedly fires.
        public float PushAndHoldRepeatInterval = Constant.Input.KeyRepeatInterval;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPush.Invoke();
            StartCoroutine(PushAndHold());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
        }

        private IEnumerator PushAndHold()
        {
            yield return new WaitForSeconds(PushAndHoldDelay);
            OnPushAndHold.Invoke();

            while (RepeatPushAndHold)
            {
                yield return new WaitForSeconds(PushAndHoldRepeatInterval);
                OnPushAndHold.Invoke();
            }
        }
    }
}