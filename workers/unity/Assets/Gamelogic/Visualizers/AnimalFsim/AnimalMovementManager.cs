using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Util;
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
    public class AnimalMovementManager : MonoBehaviour
    {
        [Require] public AnimalMovementStateWriter AnimalMovementStateWriter;
        public Rigidbody AnimalRigidbody;
        public AnimalGameInfoReader AnimalGameInfoReader;
        public AnimalInfoReader AnimalInfoReader;
        public AnimalPerceptionReader AnimalPerceptionReader;
        public AnimalStateMachineReader AnimalStateMachineReader;
        public AnimalMovementReader AnimalMovementReader;
        public float AnimalTickCounter;

        void OnEnable()
        {
            AnimalRigidbody = GetComponent<Rigidbody>();
            AnimalGameInfoReader = GetComponent<AnimalGameInfoReader>();
            AnimalInfoReader = GetComponent<AnimalInfoReader>();
            AnimalPerceptionReader = GetComponent<AnimalPerceptionReader>();
            AnimalStateMachineReader = GetComponent<AnimalStateMachineReader>();
            AnimalMovementReader = GetComponent<AnimalMovementReader>();
        }

        void FixedUpdate()
        {
            if (AnimalInfoReader.IsAlive == AnimalVitalityStatus.Alive)
            {
                AnimalTickCounter += Time.deltaTime;
                if (AnimalTickCounter >= GameSettings.AnimalTickRate)
                {
                    AnimalTickCounter -= GameSettings.AnimalTickRate;
                    UpdateSteeringSources();
                }
                MoveAnimal();
            }
        }

        void MoveAnimal()
        {
            if (transform.position.y < -1f)
            {
                transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
            }
            Vector3 currentVelocity = AnimalRigidbody.velocity;
            Vector3 targetVelocity = AnimalMovementReader.SteeringSourceAggregate.direction.ToUnityVector() * GameSettings.AnimalMovementSpeed * AnimalMovementReader.SteeringSourceAggregate.intensity;
            Vector3 velocityChange = MathUtils.FlattenVector(targetVelocity - currentVelocity);
            AnimalRigidbody.AddForce(velocityChange);
            //Debug.LogWarning(AnimalRigidbody.velocity.magnitude + ", " + velocityChange.magnitude);
        }

        void UpdateSteeringSources()
        {
            SteeringSource wanderSteeringSource = GetWanderSteeringSource();
            SteeringSource herdSteeringSource = GetHerdSteeringSource();
            SteeringSource followSteeringSource = GetFollowSteeringSource();
            SteeringSource seekSteeringSource = GetSeekSteeringSource();
            SteeringSource avoidSteeringSource = GetAvoidSteeringSource();
            AnimalMovementReader.SteeringSources[SteeringSourceType.Wander] = wanderSteeringSource;
            AnimalMovementReader.SteeringSources[SteeringSourceType.Herd] = herdSteeringSource;
            AnimalMovementReader.SteeringSources[SteeringSourceType.Follow] = followSteeringSource;
            AnimalMovementReader.SteeringSources[SteeringSourceType.Seek] = seekSteeringSource;
            AnimalMovementReader.SteeringSources[SteeringSourceType.Avoid] = avoidSteeringSource;

            SteeringSource steeringSourceAggregate = GetSteeringSourceAggregate();
            AnimalMovementReader.SteeringSourceAggregate = steeringSourceAggregate;

            if (!GameSettings.PerformanceSaverMode)
            {
                IDictionary<int, SteeringSource> newSteeringSources = new Dictionary<int, SteeringSource>()
                {
                    {(int)SteeringSourceType.Wander, wanderSteeringSource},
                    {(int)SteeringSourceType.Herd, herdSteeringSource},
                    {(int)SteeringSourceType.Follow, followSteeringSource},
                    {(int)SteeringSourceType.Seek, seekSteeringSource},
                    {(int)SteeringSourceType.Avoid, avoidSteeringSource}
                };
                AnimalMovementStateWriter.Update.SteeringSources(newSteeringSources).SteeringSourceAggregate(steeringSourceAggregate).FinishAndSend();
            }
        }

        SteeringSource GetWanderSteeringSource()
        {
            Vector3 targetDirection = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f).normalized;
            return new SteeringSource(targetDirection.ToNativeVector(), 1f);
        }

        SteeringSource GetHerdSteeringSource()
        {
            float alignmentWeight = 1f;
            float cohesionWeight = 1f;
            float separationWeight = 1f;

            IList<Vector3> fellowVelocities = new List<Vector3>();
            IList<Vector3> fellowPositions = new List<Vector3>();
            IList<Vector3> animalPositions = new List<Vector3>(); // feature creep: avoid animals from other species here too
            foreach (var item in AnimalPerceptionReader.PerceivedAnimals)
            {
                if (item.Value.species == AnimalInfoReader.Species && item.Value.isAlive == AnimalVitalityStatus.Alive)
                {
                    fellowVelocities.Add(item.Value.velocity.ToUnityVector());
                    fellowPositions.Add(item.Value.position.ToUnityVector());
                }
                if (item.Value.isAlive == AnimalVitalityStatus.Alive && item.Value.entityId != AnimalStateMachineReader.TargetEntity)
                {
                    animalPositions.Add(item.Value.position.ToUnityVector());
                }
            }
            Vector3 alignmentDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(MathUtils.GetAverageVector(fellowVelocities)));
            Vector3 cohesionDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(MathUtils.GetAverageVector(fellowPositions)));
            Vector3 separationDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(GetAvoidDirection(animalPositions)));
            Vector3 targetDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(alignmentDirection * alignmentWeight + cohesionDirection * cohesionWeight + separationDirection * separationWeight));
            return new SteeringSource(targetDirection.ToNativeVector(), 1f);
        }

        Vector3 GetAvoidDirection(IList<Vector3> obstaclePositions)
        {
            Vector3 result = Vector3.zero;
            for (int i = 0; i < obstaclePositions.Count; i++)
            {
                result += (transform.position - obstaclePositions[i]).normalized * (1 - (transform.position - obstaclePositions[i]).sqrMagnitude / (GameSettings.PerceptionRadius * GameSettings.PerceptionRadius)) / obstaclePositions.Count;
            }
            return result;
        }

        SteeringSource GetFollowSteeringSource()
        {
            Vector3 targetDirection = Vector3.zero;
            return new SteeringSource(targetDirection.ToNativeVector(), 1f);
        }

        SteeringSource GetSeekSteeringSource()
        {
            Vector3 targetDirection = Vector3.zero;
            if (AnimalStateMachineReader.CurrentState == AnimalState.SeekingEntity || AnimalStateMachineReader.CurrentState == AnimalState.Hunting)
            {
                if (AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity))
                {
                    targetDirection = (AnimalPerceptionReader.PerceivedEnvironmentEntities[AnimalStateMachineReader.TargetEntity].position.ToUnityVector() - transform.position).normalized;
                }
                if (AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity))
                {
                    targetDirection = (AnimalPerceptionReader.PerceivedAnimals[AnimalStateMachineReader.TargetEntity].position.ToUnityVector() - transform.position).normalized;
                }
            }
            if (AnimalStateMachineReader.CurrentState == AnimalState.SeekingPosition)
            {
                targetDirection = (AnimalStateMachineReader.TargetPosition - transform.position).normalized;
            }
            return new SteeringSource(targetDirection.ToNativeVector(), 1f);
        }

        SteeringSource GetAvoidSteeringSource()
        {
            float avoidEntitiesWeight = 1f;
            float avoidWorldBoundariesWeight = 4f;
            IList<Vector3> obstaclePositions = new List<Vector3>();
            foreach (var item in AnimalPerceptionReader.PerceivedEnvironmentEntities)
            {
                if (GameUtils.IsObstacleEnvironment(AnimalStateMachineReader.TargetNeed, item.Value) && (transform.position - item.Value.position.ToUnityVector()).magnitude <= AnimalProperties.GetAvoidDistance(AnimalStateMachineReader.CurrentState))
                {
                    obstaclePositions.Add(item.Value.position.ToUnityVector());
                }
            }
            foreach (var item in AnimalPerceptionReader.PerceivedAnimals)
            {
                if (GameUtils.IsObstacleAnimal(AnimalInfoReader.Species, item.Value) && (transform.position - item.Value.position.ToUnityVector()).magnitude <= AnimalProperties.GetAvoidDistance(AnimalStateMachineReader.CurrentState))
                {
                    obstaclePositions.Add(item.Value.position.ToUnityVector());
                }
            }
            Vector3 avoidEntitiesDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(GetAvoidDirection(obstaclePositions)));

            Vector3 avoidWorldBoundariesDirection = Vector3.zero;
            if (transform.position.x < (-AnimalGameInfoReader.MapSize / 2 + GameSettings.WorldBoundaryRadius) * AnimalGameInfoReader.GridSize)
            {
                avoidWorldBoundariesDirection += Vector3.right;
            }
            if (transform.position.x > (AnimalGameInfoReader.MapSize / 2 - GameSettings.WorldBoundaryRadius) * AnimalGameInfoReader.GridSize)
            {
                avoidWorldBoundariesDirection += Vector3.left;
            }
            if (transform.position.z < (-AnimalGameInfoReader.MapSize / 2 + GameSettings.WorldBoundaryRadius) * AnimalGameInfoReader.GridSize)
            {
                avoidWorldBoundariesDirection += Vector3.forward;
            }
            if (transform.position.x > (AnimalGameInfoReader.MapSize / 2 - GameSettings.WorldBoundaryRadius) * AnimalGameInfoReader.GridSize)
            {
                avoidWorldBoundariesDirection += Vector3.back;
            }

            Vector3 targetDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(avoidEntitiesDirection * avoidEntitiesWeight + avoidWorldBoundariesDirection * avoidWorldBoundariesWeight));
            return new SteeringSource(targetDirection.ToNativeVector(), 1f);
        }

        SteeringSource GetSteeringSourceAggregate()
        {
            Vector3 targetDirection = Vector3.zero;
            foreach (var item in AnimalMovementReader.SteeringSources)
            {
                targetDirection += item.Value.direction.ToUnityVector() * item.Value.intensity * AnimalProperties.SteeringSourceModifiersByState[AnimalStateMachineReader.CurrentState][item.Key] * AnimalGameInfoReader.SteeringSourceWeights[item.Key];
            }
            targetDirection = MathUtils.ClampNormalise(MathUtils.FlattenVector(targetDirection));
            float staminaFactor = AnimalInfoReader.Stamina / 100f;
            float intensityFactor = (AnimalProperties.MovementSpeedsByState[AnimalStateMachineReader.CurrentState] - 0.5f) / 0.5f;
            float targetIntensity = 0.5f + (0.5f * staminaFactor * intensityFactor);
            return new SteeringSource(targetDirection.ToNativeVector(), targetIntensity);
        }
    }
}
