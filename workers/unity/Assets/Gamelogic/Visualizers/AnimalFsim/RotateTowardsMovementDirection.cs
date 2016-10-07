using Assets.Gamelogic.Visualizers.AnimalClient;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class RotateTowardsMovementDirection : MonoBehaviour
    {
        public Rigidbody AnimalRigidbody;
        public AnimalInfoReader AnimalInfoReader;
        public float VelocityThreshold = 0.4f;
        public float RotationSpeed = 120f;

        void OnEnable()
        {
            AnimalRigidbody = GetComponent<Rigidbody>();
            AnimalInfoReader = GetComponent<AnimalInfoReader>();
        }

        void FixedUpdate()
        {
            if (AnimalInfoReader.IsAlive == AnimalVitalityStatus.Alive && AnimalRigidbody.velocity.magnitude >= VelocityThreshold)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(AnimalRigidbody.velocity), RotationSpeed * Time.deltaTime);
            }
        }
    }
}