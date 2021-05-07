using System.Collections;
using UnityEngine;

namespace Octamino
{
    public static class CoroutineHelper
    {
        private class CoroutineHolder : MonoBehaviour {}
 
        private static CoroutineHolder _runner;
        private static CoroutineHolder Runner 
        {
            get {
                if (_runner == null) 
                {
                    _runner = new GameObject("Static Corotuine Runner").AddComponent<CoroutineHolder>();
                }
                return _runner;
            }
        }
 
        public static void StartCoroutine(IEnumerator coroutine) 
        {
            Runner.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(IEnumerator coroutine)
        {
            Runner.StopCoroutine(coroutine);
        }
    }
}