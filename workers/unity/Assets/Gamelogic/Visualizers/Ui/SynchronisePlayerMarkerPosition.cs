using Assets.Gamelogic.Visualizers.Global;
using Assets.Gamelogic.Visualizers.Player;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Ui
{
    public class SynchronisePlayerMarkerPosition : MonoBehaviour
    {
        public GameObject PlayerObject;
        public RectTransform OwnRectTransform;
        public GameInfoReader GameInfoReader;
        public int MiniMapResolution = 200;

        void OnEnable()
        {
            OwnRectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (!PlayerObject)
            {
                RtsCameraController rtsCameraController = FindObjectOfType<RtsCameraController>();
                if (rtsCameraController)
                {
                    PlayerObject = rtsCameraController.gameObject;
                }
            }
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
            }
            if (PlayerObject && GameInfoReader)
            {
                OwnRectTransform.anchoredPosition3D = new Vector3(PlayerObject.transform.position.x / GameInfoReader.MapSize / GameInfoReader.GridSize * MiniMapResolution, PlayerObject.transform.position.z / GameInfoReader.MapSize / GameInfoReader.GridSize * MiniMapResolution, 0f);
            }
        }
    }
}
