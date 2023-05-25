using System;
using System.Collections;
using UnityEngine;

namespace PureAnimator
{
    public class PureAnimation
    {
        public TransformChanges LastChanges { get; private set; }
        public bool IsPlay { get; private set; }
        public bool IsPlayCicle { get; private set; }

        private Coroutine _lastAnimation;
        private Func<float, TransformChanges> _body;
        private Action _exitCommand;
        
        private float _expiredSeconds = 0f;
        private float _progress = 0f;
        private float _duration;

        public void Play (float duration, Func<float, TransformChanges> body, Action exitCommand)
        {
            _duration = duration;
            _body = body;
            _exitCommand = exitCommand;
            
            IsPlay = true;
        }
        
        public void PlayCicle(float duration, Func<float, TransformChanges> body, Action exitCommand)
        {
            _duration = duration;
            _body = body;
            _exitCommand = exitCommand;
            
            IsPlayCicle = true;
        }

        public void Stop(bool isNeedCommand)
        {
            if (isNeedCommand)
                _exitCommand?.Invoke();
            Stop();
        }

        public void Stop()
        {
            IsPlay = false;
            _expiredSeconds = 0f;
            _progress = 0f;
            _duration = 0f;
        }

        public bool StepAnimation(float step)
        {
            if (!IsPlay && !IsPlayCicle)
                return false;
            if (_progress < 1f || IsPlayCicle)
            {
                _expiredSeconds += step;
                if (IsPlayCicle && _expiredSeconds > _duration)
                    _expiredSeconds -= _duration;
                
                _progress = _expiredSeconds / _duration;

                LastChanges = _body.Invoke(_progress);
                return true;
            }

            Stop();
            _exitCommand?.Invoke();
            return false;
        }
    }
}
