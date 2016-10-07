using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Game
{
    public class TerrainHeatMapDrawer : MonoBehaviour
    {
        public Renderer TerrainRenderer;
        public GradientColorKey[] HeatMapGck;
        public GradientAlphaKey[] HeatMapGak;
        public GradientColorKey[] TerrainGck;
        public GradientAlphaKey[] TerrainGak;

        void Awake()
        {
            TerrainRenderer = GetComponent<Renderer>();
        }

        void OnEnable()
        {
            HeatMapGck = new GradientColorKey[4];
            HeatMapGak = new GradientAlphaKey[4];
            HeatMapGck[0].color = Color.blue;
            HeatMapGck[0].time = 0f;
            HeatMapGck[1].color = Color.green;
            HeatMapGck[1].time = 0.33f;
            HeatMapGck[2].color = Color.yellow;
            HeatMapGck[2].time = 0.66f;
            HeatMapGck[3].color = Color.red;
            HeatMapGck[3].time = 1f;

            TerrainGck = new GradientColorKey[3];
            TerrainGak = new GradientAlphaKey[3];
            TerrainGck[0].color = new Color(200f / 255f, 180f / 255f, 70f / 255f, 1f);
            TerrainGck[0].time = 0.0f;
            TerrainGck[1].color = new Color(220f / 255f, 230f / 255f, 70f / 255f, 1f);
            TerrainGck[1].time = 0.5f;
            TerrainGck[2].color = new Color(70f / 255f, 160f / 255f, 70f / 255f, 1f);
            TerrainGck[2].time = 1.0f;
        }

        public Texture2D DrawHeatMap(float[,] map, float gridSize, int colorPaletteMode) // 1 = heat map, 2 = terrain
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            Color[] colourMap = new Color[width*height];

            Gradient gradient = new Gradient();
            switch (colorPaletteMode)
            {
                case 1:
                    gradient.SetKeys(HeatMapGck, HeatMapGak);
                    break;
                case 2:
                    gradient.SetKeys(TerrainGck, TerrainGak);
                    break;
                default:
                    gradient.SetKeys(HeatMapGck, HeatMapGak);
                    break;
            }
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    colourMap[i * width + j] = gradient.Evaluate(map[j, i]);
                }
            }
            Texture2D texture = new Texture2D(width, height);

            Texture2D miniMapTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            miniMapTexture.filterMode = FilterMode.Point;
            miniMapTexture.wrapMode = TextureWrapMode.Clamp;
            miniMapTexture.SetPixels(colourMap);
            miniMapTexture.Apply();

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colourMap);
            texture.Apply();

            TerrainRenderer.sharedMaterial.mainTexture = texture;
            TerrainRenderer.transform.localScale = new Vector3(width*gridSize/10.0f, 1, height*gridSize/10.0f);

            return miniMapTexture; //todo: unethical hacks
        }

        public void ResizeHeatMap(int mapSize, float gridSize)
        {
            transform.localScale = new Vector3(mapSize * gridSize / 10.0f, 1, mapSize * gridSize / 10.0f);
        }
    }
}
