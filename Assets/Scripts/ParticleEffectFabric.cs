using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using PureAnimator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RdmGame
{
    public class ParticleEffectFabric : MonoBehaviour
    {
        [SerializeField] private AnimationCurve jumpCurve;

        [Space(20)]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;

        [Space(20)]
        [SerializeField] private GameObject particlePrefab;

        [Space(20)]
        [SerializeField] private Config config;


        private void Start()
        {
            ForwardMove();
        }

        public void ForwardMove() =>
            StartCoroutine(SpawnParticle(config.countParticle, config.durationWave, startPoint, endPoint, BackwardMove));

        public void BackwardMove() =>
            StartCoroutine(SpawnParticle(config.countParticle, config.durationWave, endPoint, startPoint, ForwardMove));

        private IEnumerator SpawnParticle(int count, float duration, Transform from, Transform to, Action endCommand)
        {
            List<GameObject> coins = new List<GameObject>();
            var delay = duration / count;
            for (int i = 0; i < count; i++)
            {
                coins.Add(MoveCoin(from, to));
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitForSeconds(config.delayWave);
            endCommand?.Invoke();
        }

        private GameObject MoveCoin(Transform from, Transform to)
        {
            var particle = CreateNewParticle(from);
            var positionOffset = Random.insideUnitCircle * 0.25f;
            particle.position = new Vector3(particle.position.x + positionOffset.x, particle.position.y + positionOffset.y, particle.position.z);
            
            var startPosition = particle.position;
            var endPosition = to.position;
            var duration = 5.0f;
            var direction = Random.insideUnitCircle.normalized;
            var height = Random.Range(0.0f, config.heightWave);
            var offset = Random.Range(0f, 100000f);

            var jump = Services<PureAnimatorController>.Get().GetPureAnimator();
            jump.Play(duration, (progress) =>
            {
                if (progress > 1) progress = 1;
                float noiseOffset = Get2DPerlin(new Vector2(progress, 0), offset, config.noiseScale) * config.noiseStrange;
                Vector3 position =
                    Vector3.Scale(new Vector3(noiseOffset * height * jumpCurve.Evaluate(progress), noiseOffset * height * jumpCurve.Evaluate(progress), 0),
                        direction);

                return new TransformChanges(position);
            }, () => { });
            
            var move = Services<PureAnimatorController>.Get().GetPureAnimator();
            move.Play(duration, progress =>
            {
                var newPos = Vector3.Lerp(startPosition, endPosition + new Vector3(positionOffset.x, positionOffset.y, 0), progress) + 
                             jump.LastChanges.Value;
                particle.position = newPos;
                return default;
            }, () =>
            {
                Destroy(particle.gameObject);
            });
            return particle.gameObject;
        }
        
        private Transform CreateNewParticle(Transform parent)
        {
            GameObject partObject = Instantiate(particlePrefab);

            partObject.transform.position = parent.position;

            return partObject.transform;
        }

        private float Get2DPerlin(Vector2 position, float offset, float scale)
        {
            position.x += (offset + 0.1f);
            position.y += (offset + 0.1f);

            return Mathf.PerlinNoise(position.x * scale, position.y * scale);
        }
    }
}
