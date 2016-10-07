using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Util;
using Improbable;
using Improbable.Animal;
using Improbable.Unity;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;
using System.Linq;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalStateMachineManager : MonoBehaviour
    {
        [Require] public AnimalStateMachineStateWriter AnimalStateMachineStateWriter;
        [Require] public AnimalGsimStateReader AnimalGsimStateReader;
        public AnimalInfoReader AnimalInfoReader;
        public AnimalGameInfoReader AnimalGameInfoReader;
        public AnimalPerceptionReader AnimalPerceptionReader;
        public AnimalStateMachineReader AnimalStateMachineReader;
        public float AnimalTickCounter;
        public float GameHourTickCounter;

        void OnEnable()
        {
            AnimalInfoReader = GetComponent<AnimalInfoReader>();
            AnimalGameInfoReader = GetComponent<AnimalGameInfoReader>();
            AnimalPerceptionReader = GetComponent<AnimalPerceptionReader>();
            AnimalStateMachineReader = GetComponent<AnimalStateMachineReader>();

            AnimalGsimStateReader.TransitionToStateEventData += OnTransitionToStateEventData;
        }

        void OnDisable()
        {
            AnimalGsimStateReader.TransitionToStateEventData -= OnTransitionToStateEventData;
        }

        void OnTransitionToStateEventData(TransitionToStateEventData t)
        {
            switch (t.state)
            {
                case AnimalState.Interacting:
                {
                    TransitionToState(AnimalState.Interacting, true, AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                    break;
                }
                case AnimalState.Neutral:
                {
                    SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                    TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    break;
                }
                default:
                    break;
            }
        }

        void FixedUpdate()
        {
            if (AnimalInfoReader.IsAlive == AnimalVitalityStatus.Alive)
            {
                AnimalTickCounter += Time.deltaTime;
                if (AnimalTickCounter >= GameSettings.AnimalTickRate)
                {
                    AnimalTickCounter -= GameSettings.AnimalTickRate;
                    MakeStateTransitionDecision();
                }
                GameHourTickCounter += Time.deltaTime;
                if (GameHourTickCounter >= GameSettings.GameHourTickRate)
                {
                    GameHourTickCounter -= GameSettings.GameHourTickRate;
                    IncrementCurrentStateDuration();
                    DecrementCooldowns();
                }
            }
            else
            {
                if (AnimalStateMachineReader.CurrentState != AnimalState.Neutral)
                {
                    TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                }
            }
        }

        void MakeStateTransitionDecision()
        {
            // React to Predators
            if (AnimalProperties.GetCategoryFromType(AnimalInfoReader.Species) == AnimalCatergory.Herbivore && AnimalStateMachineReader.CurrentState != AnimalState.SeekingPosition && AnimalStateMachineReader.CurrentState != AnimalState.Fleeing)
            {
                IDictionary<EntityId, PerceivedAnimalInfo> perceivedAnimals = AnimalPerceptionReader.PerceivedAnimals;
                foreach (var item in perceivedAnimals)
                { 
                    if (AnimalProperties.GetCategoryFromType(item.Value.species) == AnimalCatergory.Carnivore && item.Value.state == AnimalState.Hunting && (transform.position - item.Value.position.ToUnityVector()).magnitude <= AnimalProperties.GetAvoidDistance(AnimalStateMachineReader.CurrentState))
                    {
                        TransitionToState(AnimalState.Fleeing, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                        break;
                    }
                }
            }

            switch (AnimalStateMachineReader.CurrentState)
            {
                case AnimalState.Neutral:
                {
                    // Fall asleep
                    if (GameSettings.SleepAtNight && AnimalProperties.GetCategoryFromType(AnimalInfoReader.Species) == AnimalCatergory.Herbivore && !IsDayTime() && Random.value + GameSettings.SleepPropensity >= 1f)
                    {
                        TransitionToState(AnimalState.Sleeping, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    else
                    {
                        PickNeedTarget();
                    }
                    break;
                }
                case AnimalState.SeekingEntity:
                {
                    
                    // Weird error
                    if (AnimalStateMachineReader.TargetNeed == AnimalResource.Invalid || AnimalStateMachineReader.TargetEntity == EntityId.InvalidEntityId)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Target out of sight
                    else if (!AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && !AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity))
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Timeout
                    else if (AnimalStateMachineReader.CurrentStateDuration > GameSettings.MaxStateDuration)
                    {
                        SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Reach target
                    else if ((AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && (transform.position - AnimalPerceptionReader.PerceivedEnvironmentEntities[AnimalStateMachineReader.TargetEntity].position.ToUnityVector()).sqrMagnitude <= GameSettings.InteractionRange) ||
                            (AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity) && (transform.position - AnimalPerceptionReader.PerceivedAnimals[AnimalStateMachineReader.TargetEntity].position.ToUnityVector()).sqrMagnitude <= GameSettings.InteractionRange ||
                            (AnimalStateMachineReader.TargetNeed == AnimalResource.Water && AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && (transform.position - AnimalPerceptionReader.PerceivedEnvironmentEntities[AnimalStateMachineReader.TargetEntity].position.ToUnityVector()).sqrMagnitude <= 600)))
                    {
                        if (AnimalStateMachineReader.TargetNeed != AnimalResource.Mating)
                        {
                            TransitionToState(AnimalState.Interacting, true, AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        }
                    }
                    else
                    {
                        //PickNeedTarget();
                    }
                    break;
                }
                case AnimalState.Interacting:
                {
                    // Mating case
                    if (AnimalStateMachineReader.TargetNeed == AnimalResource.Mating)
                    {
                        // Fallthrough, to be handled in MatingBehaviour
                    }
                    // Weird error
                    else if (AnimalStateMachineReader.TargetNeed == AnimalResource.Invalid || AnimalStateMachineReader.TargetEntity == EntityId.InvalidEntityId)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Target out of sight
                    else if (!AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && !AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity))
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Timeout
                    else if (AnimalStateMachineReader.CurrentStateDuration > GameSettings.MaxStateDuration)
                    {
                        SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Need satisfied
                    else if (GetResourceStatus(AnimalStateMachineReader.TargetNeed) >= 100f)
                    {
                        SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    break;
                }
                case AnimalState.Hunting:
                {
                    // Weird error
                    if (AnimalStateMachineReader.TargetNeed == AnimalResource.Invalid || AnimalStateMachineReader.TargetEntity == EntityId.InvalidEntityId)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Target out of sight
                    else if (!AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && !AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity))
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Timeout
                    else if (AnimalStateMachineReader.CurrentStateDuration > GameSettings.MaxStateDuration)
                    {
                        SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Abandon chase
                    else if (GetResourceStatus(AnimalResource.Stamina) <= 0f)
                    {
                        SetCooldowns(AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    // Reach target
                    else if ((AnimalPerceptionReader.PerceivedEnvironmentEntities.ContainsKey(AnimalStateMachineReader.TargetEntity) && (transform.position - AnimalPerceptionReader.PerceivedEnvironmentEntities[AnimalStateMachineReader.TargetEntity].position.ToUnityVector()).sqrMagnitude <= GameSettings.InteractionRange) ||
                            (AnimalPerceptionReader.PerceivedAnimals.ContainsKey(AnimalStateMachineReader.TargetEntity) && (transform.position - AnimalPerceptionReader.PerceivedAnimals[AnimalStateMachineReader.TargetEntity].position.ToUnityVector()).sqrMagnitude <= GameSettings.InteractionRange))
                    {
                        AnimalStateMachineStateWriter.Update.TriggerAttackPreyEvent(AnimalStateMachineReader.TargetEntity).FinishAndSend();
                        TransitionToState(AnimalState.Interacting, true, AnimalStateMachineReader.TargetNeed, AnimalStateMachineReader.TargetEntity);
                    }
                    else
                    {
                        PickNeedTarget();
                    }
                    break;
                }
                case AnimalState.Fleeing:
                {
                    IDictionary<EntityId, PerceivedAnimalInfo> perceivedAnimals = AnimalPerceptionReader.PerceivedAnimals;
                    bool carnivoreInSight = false;
                    foreach (var item in perceivedAnimals)
                    {
                        if (AnimalProperties.GetCategoryFromType(item.Value.species) == AnimalCatergory.Carnivore && (transform.position - item.Value.position.ToUnityVector()).magnitude <= AnimalProperties.GetAvoidDistance(AnimalStateMachineReader.CurrentState))
                        {
                            carnivoreInSight = true;
                            break;
                        }
                    }
                    if (!carnivoreInSight)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    break;
                }
                case AnimalState.Sleeping:
                {
                    if (IsDayTime() && Random.value + GameSettings.SleepPropensity >= 1f)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    break;
                }
                case AnimalState.SeekingPosition:
                {
                    if ((transform.position - AnimalStateMachineReader.TargetPosition).sqrMagnitude <= GameSettings.InteractionRange)
                    {
                        TransitionToState(AnimalState.Neutral, true, AnimalResource.Invalid, EntityId.InvalidEntityId);
                    }
                    break;
                }
                default: break;
            }
        }

        void TransitionToState(AnimalState newState, bool resetStateDuration, AnimalResource need, EntityId entityId)
        {
            AnimalStateMachineReader.CurrentState = newState;
            AnimalStateMachineReader.CurrentStateDuration = resetStateDuration ? 0f : AnimalStateMachineReader.CurrentStateDuration;
            AnimalStateMachineReader.TargetNeed = need;
            AnimalStateMachineReader.TargetEntity = entityId;
            AnimalStateMachineStateWriter.Update.CurrentState(newState).CurrentStateDuration(resetStateDuration ? 0f : AnimalStateMachineReader.CurrentStateDuration).TargetNeed(need).TargetEntity(entityId).FinishAndSend();
        }

        void IncrementCurrentStateDuration()
        {
            AnimalStateMachineReader.CurrentStateDuration += 1f;
            if (!GameSettings.PerformanceSaverMode)
            {
                AnimalStateMachineStateWriter.Update.CurrentStateDuration(AnimalStateMachineStateWriter.CurrentStateDuration + 1f).FinishAndSend();
            }
        }

        //// Util

        bool IsDayTime()
        {
            float dayHour = AnimalGameInfoReader.GameTime % 24;
            return dayHour >= 6f && dayHour <= 18f;
        }

        float GetResourceStatus(AnimalResource resource)
        {
            float result = 0f;
            switch (resource)
            {
                case AnimalResource.Water: result = AnimalInfoReader.Water; break;
                case AnimalResource.Food: result = AnimalInfoReader.Food; break;
                case AnimalResource.Mating: result = AnimalInfoReader.Mating; break;
                case AnimalResource.Stamina: result = AnimalInfoReader.Stamina; break;
            }
            return result;
        }

        GameUtils.AnimalResourceTuple[] GetAllResourceStatusOrdered()
        {
            GameUtils.AnimalResourceTuple[] result = new GameUtils.AnimalResourceTuple[2]
            {
                new GameUtils.AnimalResourceTuple(AnimalResource.Food, AnimalInfoReader.Food),
                new GameUtils.AnimalResourceTuple(AnimalResource.Water, AnimalInfoReader.Water)
                //new GameUtils.AnimalResourceTuple(AnimalResource.Mating, AnimalInfoReader.Mating)
            };
            System.Array.Sort(result, AnimalResourceComparer);
            return result;
        }

        //// AnimalTargetPicking

        int AnimalResourceComparer(GameUtils.AnimalResourceTuple a, GameUtils.AnimalResourceTuple b)
        {
            return a.Value.CompareTo(b.Value);
        }

        void PickNeedTarget()
        {
            GameUtils.AnimalResourceTuple[] consideredAnimalResourceStatusOrdered = GetAllResourceStatusOrdered().Where(item => !AnimalStateMachineReader.NeedsCooldowns.ContainsKey(item.AnimalResource)).ToArray();
            PerceivedEnvironmentInfo[] consideredEnvironmentEntities = AnimalPerceptionReader.PerceivedEnvironmentEntities.Values.Where(item => !AnimalStateMachineReader.EntityCooldowns.ContainsKey(item.entityId)).ToArray();
            PerceivedAnimalInfo[] consideredAnimals = AnimalPerceptionReader.PerceivedAnimals.Values.Where(item => !AnimalStateMachineReader.EntityCooldowns.ContainsKey(item.entityId)).ToArray();
            for (int i = 0; i < consideredAnimalResourceStatusOrdered.Length; i++)
            {
                if (consideredAnimalResourceStatusOrdered[i].AnimalResource == AnimalResource.Mating && AnimalInfoReader.Mating > 50f)
                {
                    continue;
                }
                EntityId possibleTarget = GetClosestNotOccludedNeedTarget(consideredAnimalResourceStatusOrdered[i].AnimalResource, consideredEnvironmentEntities, consideredAnimals);
                if (possibleTarget != EntityId.InvalidEntityId && possibleTarget != AnimalStateMachineReader.TargetEntity)
                {
                    bool resetStateDuration = AnimalStateMachineReader.CurrentState == AnimalState.Neutral;
                    if (AnimalProperties.GetCategoryFromType(AnimalInfoReader.Species) == AnimalCatergory.Carnivore && consideredAnimalResourceStatusOrdered[i].AnimalResource == AnimalResource.Food &&
                        AnimalPerceptionReader.PerceivedAnimals.ContainsKey(possibleTarget) && AnimalPerceptionReader.PerceivedAnimals[possibleTarget].isAlive == AnimalVitalityStatus.Alive)
                    {
                        TransitionToState(AnimalState.Hunting, resetStateDuration, consideredAnimalResourceStatusOrdered[i].AnimalResource, possibleTarget);
                    }
                    else
                    {
                        TransitionToState(AnimalState.SeekingEntity, resetStateDuration, consideredAnimalResourceStatusOrdered[i].AnimalResource, possibleTarget);
                    }
                    break;
                }
            }
        }

        EntityId GetClosestNotOccludedNeedTarget(AnimalResource need, PerceivedEnvironmentInfo[] environmentEntities, PerceivedAnimalInfo[] animals)
        {
            System.Array.Sort(environmentEntities, EnvironmentDistanceComparer);
            System.Array.Sort(animals, AnimalDistanceComparer);
            int numSlots = 8; // hemisphere based on 8 compartments
            bool[] hemisphereMap = new bool[numSlots];
            int i = 0;
            int j = 0;
            while (i < environmentEntities.Length || j < animals.Length)
            {
                bool useNextEnvironment = false; //at most one of the two bools may be true
                bool useNextAnimal = false;
                if (i < environmentEntities.Length && j < animals.Length)
                {
                    if ((transform.position - environmentEntities[i].position.ToUnityVector()).sqrMagnitude <= (transform.position - animals[j].position.ToUnityVector()).sqrMagnitude)
                    {
                        useNextEnvironment = true;
                    }
                    else
                    {
                        useNextAnimal = true;
                    }
                }
                else
                {
                    useNextEnvironment = i < environmentEntities.Length;
                    useNextAnimal = j < animals.Length;
                }

                if (useNextEnvironment)
                {
                    if (!hemisphereMap[GetHemisphereSlot(environmentEntities[i].position.ToUnityVector(), numSlots)] && IsNeedTargetEnvironment(need, environmentEntities[i]))
                    {
                        return environmentEntities[i].entityId;
                    }
                    else if (GameUtils.IsObstacleEnvironment(need, environmentEntities[i]))
                    {
                        hemisphereMap[GetHemisphereSlot(environmentEntities[i].position.ToUnityVector(), numSlots)] = true;
                    }
                    i++;
                }
                if (useNextAnimal)
                {
                    if (!hemisphereMap[GetHemisphereSlot(animals[j].position.ToUnityVector(), numSlots)] && IsNeedTargetAnimal(need, animals[j]))
                    {
                        return animals[j].entityId;
                    }
                    else if (GameUtils.IsObstacleAnimal(AnimalInfoReader.Species, animals[j]))
                    {
                        hemisphereMap[GetHemisphereSlot(animals[j].position.ToUnityVector(), numSlots)] = true;
                    }
                    j++;
                }
            }
            return EntityId.InvalidEntityId;
        }

        int GetHemisphereSlot(Vector3 otherPosition, int numSlots)
        {
            Vector3 direction = (otherPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.z, direction.x)/Mathf.PI; // from -1 to 1
            return (int) (Mathf.Min((angle + 1f)/2f, 0.9999f)*numSlots);
        }

        int EnvironmentDistanceComparer(PerceivedEnvironmentInfo a, PerceivedEnvironmentInfo b)
        {
            return (transform.position - a.position.ToUnityVector()).sqrMagnitude.CompareTo((transform.position - b.position.ToUnityVector()).sqrMagnitude);
        }

        int AnimalDistanceComparer(PerceivedAnimalInfo a, PerceivedAnimalInfo b)
        {
            return (transform.position - a.position.ToUnityVector()).sqrMagnitude.CompareTo((transform.position - b.position.ToUnityVector()).sqrMagnitude);
        }

        bool IsNeedTargetEnvironment(AnimalResource need, PerceivedEnvironmentInfo otherEntity)
        {
            bool result = false;
            switch (need)
            {
                case AnimalResource.Water:
                {
                    result = EnvironmentProperties.GetCategoryFromType(otherEntity.environmentType) == EnvironmentCategory.WaterSource  &&
                             otherEntity.resources >= 10f;
                    break;
                }
                case AnimalResource.Food: 
                {
                    result = (AnimalProperties.GetCategoryFromType(AnimalInfoReader.Species) == AnimalCatergory.Herbivore &&
                             EnvironmentProperties.GetCategoryFromType(otherEntity.environmentType) == EnvironmentCategory.Plant) &&
                             otherEntity.resources >= 10f;
                    break;
                }
                default:
                    break;
            }
            return result;
        }

        bool IsNeedTargetAnimal(AnimalResource need, PerceivedAnimalInfo otherEntity)
        {
            bool result = false;
            switch (need)
            {
                case AnimalResource.Water:
                {
                    break;
                }
                case AnimalResource.Food:
                {
                    result = AnimalProperties.GetCategoryFromType(AnimalInfoReader.Species) == AnimalCatergory.Carnivore &&
                             AnimalProperties.GetCategoryFromType(otherEntity.species) == AnimalCatergory.Herbivore &&
                             otherEntity.isAlive != AnimalVitalityStatus.DeadNatural; //todo: resources >= 0
                    break;
                }
                case AnimalResource.Mating:
                {
                    result = AnimalInfoReader.IsAdult &&
                             otherEntity.species == AnimalInfoReader.Species &&
                             otherEntity.gender != AnimalInfoReader.Gender &&
                             otherEntity.isAlive == AnimalVitalityStatus.Alive &&
                             otherEntity.isAdult &&
                             !otherEntity.isPregnant &&
                             otherEntity.entityId != AnimalInfoReader.Mother;
                    break;
                }
                default:
                    break;
            }
            return result;
        }

        //// Cooldowns

        void DecrementCooldowns()
        {
            IDictionary<AnimalResource, float> newNeedsCooldowns = new Dictionary<AnimalResource, float>();
            foreach (var item in AnimalStateMachineReader.NeedsCooldowns)
            {
                if (item.Value >= 1f)
                {
                    newNeedsCooldowns.Add(item.Key, item.Value - 1f);
                }
            }
            IDictionary<EntityId, float> newEntityCooldowns = new Dictionary<EntityId, float>();
            foreach (var item in AnimalStateMachineReader.EntityCooldowns)
            {
                if (item.Value >= 1f)
                {
                    newEntityCooldowns.Add(item.Key, item.Value - 1f);
                }
            }
            AnimalStateMachineReader.NeedsCooldowns = newNeedsCooldowns;
            AnimalStateMachineReader.EntityCooldowns = newEntityCooldowns;
            if (!GameSettings.PerformanceSaverMode)
            {
                IDictionary<int, float> newNeedsCooldownsState = new Dictionary<int, float>();
                foreach (var item in newNeedsCooldowns)
                {
                    newNeedsCooldownsState.Add((int)item.Key, item.Value);
                }
                AnimalStateMachineStateWriter.Update.NeedsCooldowns(newNeedsCooldownsState).EntityCooldowns(newEntityCooldowns).FinishAndSend();
            }
        }

        void SetCooldowns(AnimalResource need, EntityId entityId)
        {
            if (need != AnimalResource.Invalid)
            {
                IDictionary<AnimalResource, float> newNeedsCooldowns = AnimalStateMachineReader.NeedsCooldowns;
                newNeedsCooldowns[need] = GameSettings.NeedCooldownPeriod;
                AnimalStateMachineReader.NeedsCooldowns = newNeedsCooldowns;
                if (!GameSettings.PerformanceSaverMode)
                {
                    IDictionary<int, float> newNeedsCooldownsState = new Dictionary<int, float>();
                    foreach (var item in newNeedsCooldowns)
                    {
                        newNeedsCooldownsState.Add((int)item.Key, item.Value);
                    }
                    AnimalStateMachineStateWriter.Update.NeedsCooldowns(newNeedsCooldownsState).FinishAndSend();
                }
            }
            if (entityId != EntityId.InvalidEntityId)
            {
                IDictionary<EntityId, float> newEntityCooldowns = AnimalStateMachineReader.EntityCooldowns;
                newEntityCooldowns[entityId] = GameSettings.EntityCooldownPeriod;
                AnimalStateMachineReader.EntityCooldowns = newEntityCooldowns;
                if (!GameSettings.PerformanceSaverMode)
                {
                    AnimalStateMachineStateWriter.Update.EntityCooldowns(newEntityCooldowns).FinishAndSend();
                }
            }
        }
    }
}
