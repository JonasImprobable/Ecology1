using Improbable.Animal;
using Improbable.Unity.Visualizer;
using System.Collections.Generic;
using Improbable.Collections;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalMovementReader : MonoBehaviour
    {
        [Require] public AnimalMovementStateReader AnimalMovementStateReader;
        public SteeringSource SteeringSourceAggregate;
        public IDictionary<SteeringSourceType, SteeringSource> SteeringSources = new Dictionary<SteeringSourceType, SteeringSource>();

        void OnEnable()
        {
            AnimalMovementStateReader.SteeringSourceAggregateUpdated += OnSteeringSourceAggregateUpdated;
            AnimalMovementStateReader.SteeringSourcesUpdated += OnSteeringSourcesUpdated;
        }

        void OnDisable()
        {
            AnimalMovementStateReader.SteeringSourceAggregateUpdated -= OnSteeringSourceAggregateUpdated;
            AnimalMovementStateReader.SteeringSourcesUpdated -= OnSteeringSourcesUpdated;
        }

        void OnSteeringSourceAggregateUpdated(SteeringSource s)
        {
            SteeringSourceAggregate = s;
        }

        void OnSteeringSourcesUpdated(Map<int, SteeringSource> s)
        {
            SteeringSources = new Dictionary<SteeringSourceType, SteeringSource>();
            foreach (var item in s)
            {
                SteeringSources.Add((SteeringSourceType)item.Key, item.Value);
            }
        }
    }
}
