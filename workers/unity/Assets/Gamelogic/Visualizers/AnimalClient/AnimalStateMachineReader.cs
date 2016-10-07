using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.Util;
using Improbable;
using Improbable.Animal;
using Improbable.Collections;
using Improbable.Math;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalStateMachineReader : MonoBehaviour
    {
        [Require] public AnimalStateMachineStateReader AnimalStateMachineStateReader;
        public GameObject StateIndicatorPrefab;
        public GameObject StateIndicator;
        public GameObject DestinationIndicator;
        public GameObject HeartParticlePrefab;

        public AnimalState CurrentState;
        public float CurrentStateDuration;
        public EntityId TargetEntity;
        public AnimalResource TargetNeed;
        public Vector3 TargetPosition;
        public IDictionary<EntityId, float> EntityCooldowns = new Dictionary<EntityId, float>();
        public IDictionary<AnimalResource, float> NeedsCooldowns = new Dictionary<AnimalResource, float>();
        
        void OnEnable()
        {
            StateIndicatorPrefab = Resources.Load<GameObject>("Indicators/StateIndicator");
            HeartParticlePrefab = Resources.Load<GameObject>("Particles/HeartParticle");
            AnimalStateMachineStateReader.CurrentStateUpdated += OnCurrentStateUpdated;
            AnimalStateMachineStateReader.CurrentStateDurationUpdated += OnCurrentStateDurationUpdated;
            AnimalStateMachineStateReader.TargetEntityUpdated += OnTargetEntityUpdated;
            AnimalStateMachineStateReader.TargetNeedUpdated += OnTargetNeedUpdated;
            AnimalStateMachineStateReader.TargetPositionUpdated += OnTargetPositionUpdated;
            AnimalStateMachineStateReader.EntityCooldownsUpdated += OnEntityCooldownsUpdated;
            AnimalStateMachineStateReader.NeedsCooldownsUpdated += OnNeedsCooldownsUpdated;
        }

        void OnDisable()
        {
            AnimalStateMachineStateReader.CurrentStateUpdated -= OnCurrentStateUpdated;
            AnimalStateMachineStateReader.CurrentStateDurationUpdated -= OnCurrentStateDurationUpdated;
            AnimalStateMachineStateReader.TargetEntityUpdated -= OnTargetEntityUpdated;
            AnimalStateMachineStateReader.TargetNeedUpdated -= OnTargetNeedUpdated;
            AnimalStateMachineStateReader.TargetPositionUpdated -= OnTargetPositionUpdated;
            AnimalStateMachineStateReader.EntityCooldownsUpdated -= OnEntityCooldownsUpdated;
            AnimalStateMachineStateReader.NeedsCooldownsUpdated -= OnNeedsCooldownsUpdated;
        }

        void OnCurrentStateUpdated(AnimalState c)
        {
            CurrentState = c;

            if (CurrentState != AnimalState.Neutral)
            {
                if (StateIndicator == null)
                {
                    StateIndicator = (GameObject) Instantiate(StateIndicatorPrefab, transform.position, transform.rotation);
                    StateIndicator.transform.parent = transform;
                    StateIndicator.transform.localPosition = new Vector3(0.0f, GetComponent<BoxCollider>().size.y, 0.0f);
                }
                ChangeStateIndicatorColor(CurrentState);
            }
            else if (StateIndicator != null)
            {
                Destroy(StateIndicator);
            }

            if (CurrentState == AnimalState.SeekingPosition)
            {
                if (DestinationIndicator != null)
                {
                    Destroy(DestinationIndicator);
                }
                DestinationIndicator = (GameObject) Instantiate(StateIndicatorPrefab, TargetPosition, Quaternion.identity);
            }
            else if (DestinationIndicator != null)
            {
                Destroy(DestinationIndicator);
            }

            if (CurrentState == AnimalState.Interacting && TargetNeed == AnimalResource.Mating) // todo: heart particle only shows up at male's side, race condition
            {
                Instantiate(HeartParticlePrefab, transform.position + Vector3.up * GetComponent<BoxCollider>().size.y * 1.2f, Quaternion.identity);
            }
        }

        void ChangeStateIndicatorColor(AnimalState state)
        {
            Color indicatorColor = Color.white;
            switch (state)
            {
                case AnimalState.SeekingEntity: indicatorColor = Color.green; break;
                case AnimalState.Interacting: indicatorColor = Color.blue; break;
                case AnimalState.Hunting: indicatorColor = Color.yellow; break;
                case AnimalState.Fleeing: indicatorColor = Color.red; break;
                case AnimalState.SeekingPosition: indicatorColor = Color.magenta; break;
                case AnimalState.Sleeping: indicatorColor = Color.grey; break;
                default: break;
            }
            foreach (Transform childTransform in StateIndicator.transform)
            {
                childTransform.gameObject.GetComponent<Renderer>().material.color = indicatorColor;
            }
        }

        void OnCurrentStateDurationUpdated(float c)
        {
            CurrentStateDuration = c;
        }

        void OnTargetEntityUpdated(EntityId t)
        {
            TargetEntity = t;
        }

        void OnTargetNeedUpdated(AnimalResource t)
        {
            TargetNeed = t;
        }

        void OnTargetPositionUpdated(Vector3d t)
        {
            TargetPosition = t.ToUnityVector();
            if (DestinationIndicator != null)
            {
                DestinationIndicator.transform.position = TargetPosition;
            }
        }

        void OnEntityCooldownsUpdated(Map<EntityId, float> e)
        {
            EntityCooldowns = GameUtils.CopyMapToDictionary(e);
        }

        void OnNeedsCooldownsUpdated(Map<int, float> n)
        {
            NeedsCooldowns = new Dictionary<AnimalResource, float>();
            foreach (var item in n)
            {
                NeedsCooldowns.Add((AnimalResource)item.Key, item.Value);
            }
        }
    }
}
