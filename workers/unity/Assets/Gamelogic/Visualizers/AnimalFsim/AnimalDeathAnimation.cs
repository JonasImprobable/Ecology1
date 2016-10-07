using Improbable.Animal;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalDeathAnimation : MonoBehaviour
    {
        [Require] public AnimalInfoStateReader AnimalInfoStateReader;
        public Rigidbody AnimalRigidbody;
        public float DeathAnimationPeriod;
        public float RotationSpeed = 120f;

        void OnEnable()
        {
            AnimalRigidbody = GetComponent<Rigidbody>();
            AnimalInfoStateReader.IsAliveUpdated += OnIsAliveUpdated;

            // because of worker pooling
            DeathAnimationPeriod = 0f;
            GetComponent<RotateTowardsMovementDirection>().enabled = true;
        }

        void OnDisable()
        {
            AnimalInfoStateReader.IsAliveUpdated -= OnIsAliveUpdated;
        }

        void OnIsAliveUpdated(AnimalVitalityStatus isAlive)
        {
            if (isAlive != AnimalVitalityStatus.Alive)
            {
                GetComponent<RotateTowardsMovementDirection>().enabled = false;
                DeathAnimationPeriod = 90f;
            }
        }

        void FixedUpdate()
        {
            if (DeathAnimationPeriod > 0f)
            {
                transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
                DeathAnimationPeriod -= RotationSpeed * Time.deltaTime;
            }
        }
    }
}
