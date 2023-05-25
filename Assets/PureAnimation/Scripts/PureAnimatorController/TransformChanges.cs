using UnityEngine;

namespace PureAnimator
{
    public struct TransformChanges
    {
        public Vector3 Value { get; private set;}

        public TransformChanges(Vector3 value)
        {
            Value = value;
        }
    }
}
