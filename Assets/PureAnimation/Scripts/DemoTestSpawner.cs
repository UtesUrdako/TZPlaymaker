using System.Collections;
using UnityEngine;

namespace PureAnimator
{
    public class DemoTestSpawner : MonoBehaviour
    {
        public AnimationCurve _curve;
        public static AnimationCurve Curve;
        public int countObject = 5000;
        IEnumerator Start()
        {
            Curve = _curve;
            for (int i = 0; i < countObject; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = Vector3.forward * 1.5f * i;
                var animator = go.AddComponent<DemoAnimation>();
                animator.StartAnimation();
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
