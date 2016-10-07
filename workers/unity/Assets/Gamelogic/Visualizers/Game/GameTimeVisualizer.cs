using Assets.Gamelogic.Visualizers.Global;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Game
{
    public class GameTimeVisualizer : MonoBehaviour
    {
        public GameInfoReader GameInfoReader;
        public Light LightSource;
        public float GametimeHours;
        public float xMin = 30f; //at night
        public float xMax = 60f; //at noon
        public float yMin = -80f; //morning
        public float yMax = 80f; //night

        void OnEnable()
        {
            LightSource = GetComponent<Light>();
        }

        void Update()
        {
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
                if (GameInfoReader)
                {
                    GametimeHours = GameInfoReader.GameTime;
                }
            }
        }

        void FixedUpdate()
        {
            GametimeHours = (GametimeHours + Time.deltaTime) % 24;
            UpdateLightIntensity();
            UpdateLightRotation();
        }

        private void UpdateLightIntensity()
        {
            LightSource.intensity = -Mathf.Cos(GametimeHours / 12f * Mathf.PI);
        }

        void UpdateLightRotation()
        {
            float xRoation = Mathf.Lerp(xMin, xMax, Mathf.Clamp(-Mathf.Cos(GametimeHours / 12f * Mathf.PI), 0f, 1f));
            float yRoation = Mathf.Lerp(yMin, yMax, (-Mathf.Sin(GametimeHours / 12f * Mathf.PI) + 1) / 2f);
            transform.rotation = Quaternion.Euler(xRoation, yRoation, 0f);
        }
    }
}
