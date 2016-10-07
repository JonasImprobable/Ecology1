using System.Collections.Generic;
using Improbable.Animal;
using Improbable.Collections;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalGameInfoReader : MonoBehaviour
    {
        [Require] public AnimalGameInfoStateReader AnimalGameInfoStateReader;
        public float GameTime;
        public int MapSize;
        public float GridSize;
        public IDictionary<SteeringSourceType, float> SteeringSourceWeights = new Dictionary<SteeringSourceType, float>();

        void OnEnable()
        {
            AnimalGameInfoStateReader.GameTimeUpdated += GameTimeUpdated;
            AnimalGameInfoStateReader.MapSizeUpdated += OnMapSizeUpdated;
            AnimalGameInfoStateReader.GridSizeUpdated += OnGridSizeUpdated;
            AnimalGameInfoStateReader.SteeringSourceWeightsUpdated += OnSteeringSourceWeightsUpdated;
        }

        void OnDisable()
        {
            AnimalGameInfoStateReader.GameTimeUpdated -= GameTimeUpdated;
            AnimalGameInfoStateReader.MapSizeUpdated -= OnMapSizeUpdated;
            AnimalGameInfoStateReader.GridSizeUpdated -= OnGridSizeUpdated;
            AnimalGameInfoStateReader.SteeringSourceWeightsUpdated -= OnSteeringSourceWeightsUpdated;
        }

        void GameTimeUpdated(float g)
        {
            GameTime = g;
        }

        void OnMapSizeUpdated(int m)
        {
            MapSize = m;
            GetComponent<AnimalMemoryReader>().InValidateMemoryMap();
        }

        void OnGridSizeUpdated(float g)
        {
            GridSize = g;
            GetComponent<AnimalMemoryReader>().InValidateMemoryMap();
        }

        void OnSteeringSourceWeightsUpdated(Map<int, float> s)
        {
            SteeringSourceWeights = new Dictionary<SteeringSourceType, float>();
            foreach (var item in s)
            {
                SteeringSourceWeights.Add((SteeringSourceType)item.Key, item.Value);
            }
        }
    }
}
