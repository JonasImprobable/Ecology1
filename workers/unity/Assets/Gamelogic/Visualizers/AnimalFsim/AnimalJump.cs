using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Util;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

// todo: this is deactivated right now.
namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalJump : MonoBehaviour
    {
        public AnimalInfoReader AnimalInfoReader;
        public Rigidbody AnimalRigidbody;
        public float VelocityThreshold = 0.4f;
        public float JumpMagnitude = 3.0f;
        public float GroundedThreshold = 0.05f;
        public float JumpPropensity = 0.1f;

        void OnEnable()
        {
            AnimalRigidbody = GetComponent<Rigidbody>();
            AnimalInfoReader = GetComponent<AnimalInfoReader>();
        }

        void FixedUpdate()
        {
            if (AnimalInfoReader.IsAlive == AnimalVitalityStatus.Alive && GameSettings.AnimalJumpEnabled && AnimalRigidbody.velocity.magnitude >= VelocityThreshold && transform.position.y <= GroundedThreshold && (Random.Range(0.0f, 1.0f) + JumpPropensity >= 1.0f))
            {
                AnimalRigidbody.AddForce(new Vector3(0.0f, JumpMagnitude, 0.0f), ForceMode.Impulse);
            }
        }
        
    }
}