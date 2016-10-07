using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Environment;
using Assets.Gamelogic.Visualizers.Util;
using Improbable;
using Improbable.Animal;
using Improbable.Math;
using Improbable.Unity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using System.Collections.Generic;
using UnityEngine;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalPerceptionManager : MonoBehaviour
    {
        [Require] public AnimalPerceptionStateWriter AnimalPerceptionStateWriter;
        public AnimalInfoReader AnimalInfoReader;
        public AnimalPerceptionReader AnimalPerceptionReader;
        public AnimalStateMachineReader AnimalStateMachineReader;
        public float AnimalTickCounter;

        void OnEnable()
        {
            AnimalInfoReader = GetComponent<AnimalInfoReader>();
            AnimalPerceptionReader = GetComponent<AnimalPerceptionReader>();
            AnimalStateMachineReader = GetComponent<AnimalStateMachineReader>();
        }

        void FixedUpdate()
        {
            if (AnimalInfoReader.IsAlive == AnimalVitalityStatus.Alive)
            {
                AnimalTickCounter += Time.deltaTime;
                if (AnimalTickCounter >= GameSettings.AnimalTickRate)
                {
                    AnimalTickCounter -= GameSettings.AnimalTickRate;
                    RefreshPerception();
                }
            }
        }

        void RefreshPerception()
        {
            int perceptionLimit = 16; // Limitation: At most 16 entities perset
            Collider[] entitiesNearby = Physics.OverlapSphere(transform.position, GameSettings.PerceptionRadius);
            System.Array.Sort(entitiesNearby, ColliderDistanceComparer);
            IDictionary<EntityId, PerceivedEnvironmentInfo> perceivedEnvironmentEntities = new Dictionary<EntityId, PerceivedEnvironmentInfo>();
            IDictionary<EntityId, PerceivedAnimalInfo> perceivedAnimals = new Dictionary<EntityId, PerceivedAnimalInfo>();
            foreach (var entityCollider in entitiesNearby)
            {
                if (entityCollider.gameObject.IsEntityObject() && entityCollider.gameObject.EntityId() != gameObject.EntityId())
                {
                    switch (entityCollider.gameObject.tag)
                    {
                        case "Environment":
                        {
                            if (perceivedEnvironmentEntities.Count < perceptionLimit)
                            {
                                perceivedEnvironmentEntities.Add(entityCollider.gameObject.EntityId(), CreatePerceivedEnvironmentInfo(entityCollider.gameObject));
                            }
                            break;
                        }    
                        case "Animal":
                        {
                            if (perceivedAnimals.Count < perceptionLimit)
                            {
                                perceivedAnimals.Add(entityCollider.gameObject.EntityId(), CreatePerceivedAnimalInfo(entityCollider.gameObject));
                            }
                            break;
                        }
                        default: Debug.LogError("Empty switch in RefreshPerception()."); break;
                    }
                }
            }
            AnimalPerceptionReader.PerceivedEnvironmentEntities = perceivedEnvironmentEntities;
            AnimalPerceptionReader.PerceivedAnimals = perceivedAnimals;
            if (!GameSettings.PerformanceSaverMode)
            {
                AnimalPerceptionStateWriter.Update.PerceivedEnvironmentEntities(perceivedEnvironmentEntities).PerceivedAnimals(perceivedAnimals).FinishAndSend();
            }
        }

        //warning: function depends on local transform, not sharable
        int ColliderDistanceComparer(Collider a, Collider b)
        {
            return (transform.position - a.transform.position).sqrMagnitude.CompareTo((transform.position - b.transform.position).sqrMagnitude);
        }

        PerceivedEnvironmentInfo CreatePerceivedEnvironmentInfo(GameObject entity)
        {
            EntityId entityId = entity.EntityId();
            Vector3d position = entity.transform.position.ToNativeVector();
            EnvironmentInfoReader otherEnvironmentInfoReader = entity.GetComponent<EnvironmentInfoReader>();
            return new PerceivedEnvironmentInfo(entityId, position, otherEnvironmentInfoReader.EnvironmentType, otherEnvironmentInfoReader.Resources, otherEnvironmentInfoReader.Traversable);
        }

        PerceivedAnimalInfo CreatePerceivedAnimalInfo(GameObject entity)
        {
            EntityId entityId = entity.EntityId();
            Vector3d position = MathUtils.FlattenVector(entity.transform.position).ToNativeVector();
            Vector3d velocity = MathUtils.FlattenVector(entity.GetComponent<Rigidbody>().velocity).ToNativeVector();
            AnimalInfoReader otherAnimalInfoReader = entity.GetComponent<AnimalInfoReader>();
            AnimalStateMachineReader otherAnimalStateMachineReader = entity.GetComponent<AnimalStateMachineReader>();
            return new PerceivedAnimalInfo(entityId, position, velocity, otherAnimalInfoReader.Species, otherAnimalInfoReader.Gender, otherAnimalStateMachineReader.CurrentState, otherAnimalInfoReader.IsAlive, otherAnimalInfoReader.IsAdult, otherAnimalInfoReader.IsPregnant, otherAnimalInfoReader.Priority);
        }
    }
}
