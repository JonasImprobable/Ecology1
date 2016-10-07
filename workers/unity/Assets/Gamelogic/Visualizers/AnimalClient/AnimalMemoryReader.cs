using Assets.Gamelogic.Visualizers.Game;
using Improbable.Animal;
using Improbable.Collections;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

//todo: currently deactivated
namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalMemoryReader : MonoBehaviour
    {
        [Require] public AnimalMemoryStateReader AnimalMemoryStateReader;
        [Require] public AnimalGameInfoStateReader AnimalGameInfoStateReader;
        public TerrainHeatMapDrawer TerrainHeatMapDrawer;
        public bool MapUptodate;
        public float[,] MemoryMap;


        void OnEnable()
        {
            TerrainHeatMapDrawer = FindObjectOfType<TerrainHeatMapDrawer>();
            AnimalMemoryStateReader.MemoryMapUpdated += OnMemoryMapUpdated;
            InValidateMemoryMap();
        }

        void OnDisable()
        {
            AnimalMemoryStateReader.MemoryMapUpdated -= OnMemoryMapUpdated;
        }

        void OnMemoryMapUpdated(List<ProtoFloatArray> m)
        {
            if (m.Count == 0) // happens if memory behaviour is disabled
            {
                return;
            }

            MemoryMap = new float[m.Count, m[0].array.Count];
            for (int i = 0; i < m.Count; i++)
            {
                for (int j = 0; j < m[0].array.Count; j++)
                {
                    MemoryMap[i, j] = m[i].array[j];
                }
            }
            InValidateMemoryMap();
        }

        public void InValidateMemoryMap()
        {
            MapUptodate = false;
        }

        public void DrawMemoryHeatMap()
        {
            AnimalMemoryReader AnimalMemoryReader = GetComponent<AnimalMemoryReader>();
            if (TerrainHeatMapDrawer != null && AnimalMemoryReader != null && !MapUptodate && AnimalMemoryReader.MemoryMap != null && AnimalMemoryReader.MemoryMap.Length != 0)
            {
                TerrainHeatMapDrawer.DrawHeatMap(AnimalMemoryReader.MemoryMap, AnimalGameInfoStateReader.GridSize, 1);
                MapUptodate = true;
            }

        }
    }
}
