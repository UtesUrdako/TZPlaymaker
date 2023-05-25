using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace PureAnimator
{
    public class PureAnimationPool
    {
        private List<PureAnimation> _pureAnimations;
        private MonoBehaviour _context;

        public PureAnimationPool(MonoBehaviour context)
        {
            _context = context;
            _pureAnimations = new List<PureAnimation>();
            _pureAnimations.Add(new PureAnimation());
        }

        public PureAnimation GetPureAnimation()
        {
            if (_pureAnimations.Count > 0)
                foreach (var anim in _pureAnimations)
                    if (!anim.IsPlay)
                    {
                        var returned = anim;
                        _pureAnimations.Remove(anim);
                        return returned;
                    }

            var pureAnimation = new PureAnimation();
            return pureAnimation;
        }

        public void ReleasePureAnimation(PureAnimation pureAnimation)
        {
            _pureAnimations.Add(pureAnimation);
        }
    }
}