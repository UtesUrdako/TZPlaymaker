using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace PureAnimator
{
    public class PureAnimatorController : MonoBehaviourService<PureAnimatorController>
    {
        private HashSet<PureAnimation> _activeAnimators;
        private HashSet<PureAnimation> _queueNewAnimators;
        private List<PureAnimation> _releaseAnimations;
        
        private MonoBehaviour _context;
        private PureAnimationPool _animationPool;

        public PureAnimation GetPureAnimator()
        {
            var animator = _animationPool.GetPureAnimation();
            _queueNewAnimators.Add(animator);
            return animator;
        }

        private void ReleasePureAnimator(PureAnimation pureAnimation)
        {
            if (_activeAnimators.Contains(pureAnimation))
            {
                _activeAnimators.Remove(pureAnimation);
                _animationPool.ReleasePureAnimation(pureAnimation);
            }
        }

        private void FixedUpdate()
        {
            foreach (var anim in _activeAnimators)
                if (!anim.StepAnimation(Time.fixedDeltaTime))
                    _releaseAnimations.Add(anim);

            foreach (var newAnim in _queueNewAnimators)
                _activeAnimators.Add(newAnim);
            _queueNewAnimators.Clear();

            foreach (var anim in _releaseAnimations)
                ReleasePureAnimator(anim);
            _releaseAnimations.Clear();
        }

        protected override void OnCreateService()
        {
            _context = this;
            GameObject.DontDestroyOnLoad(gameObject);
            _activeAnimators = new HashSet<PureAnimation>();
            _queueNewAnimators = new HashSet<PureAnimation>();
            _releaseAnimations = new List<PureAnimation>();
            _animationPool = new PureAnimationPool(_context);
        }

        protected override void OnDestroyService()
        {
            _activeAnimators = null;
            _queueNewAnimators = null;
            _releaseAnimations = null;
            _animationPool = null;
        }
    }
}
