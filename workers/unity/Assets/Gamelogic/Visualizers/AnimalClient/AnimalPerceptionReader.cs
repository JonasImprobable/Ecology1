using Improbable;
using Improbable.Animal;
using Improbable.Unity.Visualizer;
using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.Util;
using Improbable.Collections;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalPerceptionReader : MonoBehaviour
    {
        [Require] public AnimalPerceptionStateReader AnimalPerceptionStateReader;
        public IDictionary<EntityId, PerceivedEnvironmentInfo> PerceivedEnvironmentEntities = new Dictionary<EntityId, PerceivedEnvironmentInfo>();
        public IDictionary<EntityId, PerceivedAnimalInfo> PerceivedAnimals = new Dictionary<EntityId, PerceivedAnimalInfo>();

        void OnEnable()
        {
            AnimalPerceptionStateReader.PerceivedEnvironmentEntitiesUpdated += OnPerceivedEnvironmentEntitiesUpdated;
            AnimalPerceptionStateReader.PerceivedAnimalsUpdated += OnPerceivedAnimalsUpdated;
        }

        void OnDisable()
        {
            AnimalPerceptionStateReader.PerceivedEnvironmentEntitiesUpdated -= OnPerceivedEnvironmentEntitiesUpdated;
            AnimalPerceptionStateReader.PerceivedAnimalsUpdated -= OnPerceivedAnimalsUpdated;
        }

        void OnPerceivedEnvironmentEntitiesUpdated(Map<EntityId, PerceivedEnvironmentInfo> p)
        {
            PerceivedEnvironmentEntities = GameUtils.CopyMapToDictionary(p);

        }

        void OnPerceivedAnimalsUpdated(Map<EntityId, PerceivedAnimalInfo> p)
        {
            PerceivedAnimals = GameUtils.CopyMapToDictionary(p);
        }
    }
}
