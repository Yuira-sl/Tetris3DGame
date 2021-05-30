using System;
using System.Collections.Generic;
using UnityEngine;

namespace Octamino
{
    public class KeyboardInput : IPlayerInput
    {
        private readonly Dictionary<KeyCode, Action> _actionForKey = new Dictionary<KeyCode, Action>
        {
            { KeyCode.A, () => Game.Instance.MoveHorizontal(0) },
            { KeyCode.D, () => Game.Instance.MoveHorizontal(1) },
            { KeyCode.S, () => Game.Instance.MoveDown() },
            { KeyCode.E, () => Game.Instance.Rotate(false) },
            { KeyCode.Q, () => Game.Instance.Rotate(true) },
            { KeyCode.Space, () => Game.Instance.FallDown() }
        };

        private float _repeatedKeyTime;

        public void Update()
        {
            if (!Game.Instance.IsPlaying)
            {
                return;
            }
            
            foreach (var pair in _actionForKey)
            {
                if (UnityEngine.Input.GetKeyDown(pair.Key))
                {
                    pair.Value.Invoke();
                    _repeatedKeyTime = Time.time + Constant.Input.KeyRepeatDelay;
                }
                else if (UnityEngine.Input.GetKeyUp(pair.Key))
                {
                    _repeatedKeyTime = 0;
                }

                if (UnityEngine.Input.GetKey(pair.Key))
                {
                    if (Time.time >= _repeatedKeyTime)
                    {
                        pair.Value.Invoke();
                        _repeatedKeyTime = Time.time + Constant.Input.KeyRepeatInterval;
                    }
                }
            }
        }
    }
}