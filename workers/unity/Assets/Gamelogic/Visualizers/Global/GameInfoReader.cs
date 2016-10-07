using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.Game;
using Assets.Gamelogic.Visualizers.Util;
using Improbable.Collections;
using UnityEngine;
using Improbable.Global;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine.UI;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.Global
{
    public class GameInfoReader : MonoBehaviour
    {
        [Require] public GameInfoStateReader GameInfoStateReader;
        public TerrainHeatMapDrawer TerrainHeatMapDrawer;
        public float GameTime;
        public int MapSize;
        public float GridSize;
        public float[,] TerrainNoiseMap = new float[1,1];
        public IDictionary<SteeringSourceType, float> SteeringSourceWeights = new Dictionary<SteeringSourceType, float>();
        public IDictionary<string, int> EntityRegistry = new Dictionary<string, int>();

        void OnEnable()
        {
            TerrainHeatMapDrawer = FindObjectOfType<TerrainHeatMapDrawer>();
            GameInfoStateReader.GameTimeUpdated += OnGameTimeUpdated;
            GameInfoStateReader.MapSizeUpdated += OnMapSizeUpdated;
            GameInfoStateReader.GridSizeUpdated += OnGridSizeUpdated;
            GameInfoStateReader.TerrainNoiseMapScaledUpdated += OnTerrainNoiseMapScaledUpdated;
            GameInfoStateReader.SteeringSourceWeightsUpdated += OnSteeringSourceWeightsUpdated;
            GameInfoStateReader.EntityRegistryUpdated += OnEntityRegistryUpdated;
        }

        void OnDisable()
        {
            GameInfoStateReader.GameTimeUpdated -= OnGameTimeUpdated;
            GameInfoStateReader.MapSizeUpdated -= OnMapSizeUpdated;
            GameInfoStateReader.GridSizeUpdated -= OnGridSizeUpdated;
            GameInfoStateReader.TerrainNoiseMapScaledUpdated -= OnTerrainNoiseMapScaledUpdated;
            GameInfoStateReader.SteeringSourceWeightsUpdated -= OnSteeringSourceWeightsUpdated;
            GameInfoStateReader.EntityRegistryUpdated -= OnEntityRegistryUpdated;
        }

        void OnGameTimeUpdated(float g)
        {
            GameTime = g;
        }

        void FixedUpdate()
        {
            GameTime += Time.deltaTime;
        }

        void OnMapSizeUpdated(int m)
        {
            MapSize = m;
            if (TerrainHeatMapDrawer != null) TerrainHeatMapDrawer.ResizeHeatMap(MapSize, GridSize);
        }

        void OnGridSizeUpdated(float g)
        {
            GridSize = g;
            if (TerrainHeatMapDrawer != null) TerrainHeatMapDrawer.ResizeHeatMap(MapSize, GridSize);
        }

        void OnTerrainNoiseMapScaledUpdated(global::Improbable.Collections.List<ProtoFloatArray> t)
        {
            if (t.Count == 0)
            {
                return;
            }

            TerrainNoiseMap = new float[t.Count, t[0].array.Count];
            for (int i = 0; i < t.Count; i++)
            {
                for (int j = 0; j < t[0].array.Count; j++)
                {
                    TerrainNoiseMap[i, j] = t[i].array[j];
                }
            }
            DrawTerrainHeatMap();
        }

        public void DrawTerrainHeatMap()
        {
            if (TerrainHeatMapDrawer != null)
            {
                Texture2D noiseTexture = TerrainHeatMapDrawer.DrawHeatMap(TerrainNoiseMap, GridSize, 2);
                TerrainHeatMapDrawer.ResizeHeatMap(MapSize, GridSize);
                GameObject miniMap = GameObject.Find("MiniMap");
                if (miniMap) miniMap.GetComponent<RawImage>().texture = noiseTexture;
            }
        }

        void OnSteeringSourceWeightsUpdated(Map<int, float> s)
        {
            SteeringSourceWeights = new Dictionary<SteeringSourceType, float>();
            foreach (var item in s)
            {
                SteeringSourceWeights.Add((SteeringSourceType)item.Key, item.Value);
            }
        }

        void OnEntityRegistryUpdated(Map<string, int> e)
        {
            EntityRegistry = GameUtils.CopyMapToDictionary(e);
        }
    }
}
