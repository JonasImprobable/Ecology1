using Improbable.Animal;
using Improbable.Unity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalInteractionMovement : MonoBehaviour
    {
        [Require] public AnimalStateMachineStateReader AnimalStateMachineStateReader;
        [Require] public AnimalPerceptionStateReader AnimalPerceptionStateReader;
        public bool IsInteracting = false;
        public float MovementSpeed = 6f;
        
        void OnEnable()
        {
            AnimalStateMachineStateReader.CurrentStateUpdated += OnCurrentStateUpdated;
        }

        void OnDisable()
        {
            AnimalStateMachineStateReader.CurrentStateUpdated -= OnCurrentStateUpdated;
        }

        void OnCurrentStateUpdated(AnimalState c)
        {
            IsInteracting = (c == AnimalState.Interacting);
        }

        void FixedUpdate()
        {
            if (IsInteracting && AnimalPerceptionStateReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineStateReader.TargetEntity))
            {
                Vector3 targetPosition = AnimalPerceptionStateReader.PerceivedEnvironmentEntities[AnimalStateMachineStateReader.TargetEntity].position.ToUnityVector();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MovementSpeed * Time.deltaTime);
            }
            if (IsInteracting && AnimalPerceptionStateReader.PerceivedAnimals.ContainsKey(AnimalStateMachineStateReader.TargetEntity))
            {
                Vector3 targetPosition = AnimalPerceptionStateReader.PerceivedAnimals[AnimalStateMachineStateReader.TargetEntity].position.ToUnityVector();
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MovementSpeed * Time.deltaTime);
            }
        }
    }
}
