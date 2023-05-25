using System;
using Common;
using UnityEngine;

namespace PureAnimator
{
    public class DemoAnimation : MonoBehaviour
    {
        private Vector3 _startPosition;
        private float _directionMove = 5f;
        private float _duration = 2f;

        public void StartAnimation()
        {
            StartNew();
        }

        void StartNew()
        {
            _startPosition = transform.position;
            var pureAnimatorJump = Services<PureAnimatorController>.Get().GetPureAnimator();
            pureAnimatorJump.Play(_duration, JumpCommand, () => { });
            
            var pureAnimatorScale = Services<PureAnimatorController>.Get().GetPureAnimator();
            pureAnimatorScale.Play(_duration, ScaleCommand, () => { });
            
            var pureAnimatorMove = Services<PureAnimatorController>.Get().GetPureAnimator();
            pureAnimatorMove.Play(_duration,
                progress =>
                {
                    transform.position = Vector3.Lerp(_startPosition,
                        _startPosition + Vector3.right * _directionMove,
                        progress) + pureAnimatorJump.LastChanges.Value;
                    transform.localScale = pureAnimatorScale.LastChanges.Value;
                    return default;
                },
                ExitCommand
            );
        }

        private TransformChanges JumpCommand(float progress)
        {
            if (progress > 1) progress = 1;
            Vector3 position =
                Vector3.Scale(new Vector3(0, 2f * DemoTestSpawner.Curve.Evaluate(progress), 0),
                    Vector3.up);
            return new TransformChanges(position);
        }
        
        private TransformChanges ScaleCommand(float progress)
        {
            Vector3 scale = Vector3.one + Vector3.up * 0.5f * DemoTestSpawner.Curve.Evaluate(progress);
            return new TransformChanges(scale);
        }

        private void ExitCommand()
        {
            _directionMove *= -1;
            StartNew();
        }
    }
}
