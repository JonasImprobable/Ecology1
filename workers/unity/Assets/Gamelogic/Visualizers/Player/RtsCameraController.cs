using Assets.Gamelogic.Visualizers.Global;
using Assets.Gamelogic.Visualizers.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Player
{
    public class RtsCameraController : MonoBehaviour
    {
        public Transform CameraTransform;
        public GameInfoReader GameInfoReader;
        public RectTransform MiniMapRectTransform;
        public bool MouseMovementOn = false;
        public float MinDistance = 10f;
        public float MaxDistance = 100f;
        public float Distance = 30f;
        public float Rotation = 0f;
        public float MovementSpeed = 2f;
        public float EdgeSensitivity = 0.2f;

        void OnEnable()
        {
            CameraTransform = GetComponentInChildren<Camera>().transform;
            UpdateDistance(Distance);
            GameObject miniMap = GameObject.Find("MiniMap");
            if (miniMap) MiniMapRectTransform = miniMap.GetComponent<RectTransform>();
        }

        void Update()
        {
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
            }

            if (Input.GetKeyDown(GameSettings.ToggleCameraControls))
            {
                MouseMovementOn = !MouseMovementOn;
            }
            if (Input.GetMouseButton(1))
            {
                UpdateRotation(Rotation + Input.GetAxis("Mouse X") * 5);
            }
            UpdatePosition();
            Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * Distance, MinDistance, MaxDistance);
            UpdateDistance(Distance);

            if (Input.GetMouseButtonDown(0) && MiniMapRectTransform)
            {
                if (Mathf.Abs(Input.mousePosition.x - MiniMapRectTransform.position.x) <= 100 && Mathf.Abs(Input.mousePosition.y - MiniMapRectTransform.position.y) <= 100)
                {
                    float posX = (Input.mousePosition.x - MiniMapRectTransform.position.x) / 200 * GameInfoReader.MapSize * GameInfoReader.GridSize;
                    float posZ = (Input.mousePosition.y - MiniMapRectTransform.position.y) / 200 * GameInfoReader.MapSize * GameInfoReader.GridSize;
                    transform.position = new Vector3(posX, 0f, posZ);
                }
            }
        }

        private void UpdatePosition()
        {
            Vector3 movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            float actualMovementSpeed = (Input.GetKey(KeyCode.LeftShift)) ? MovementSpeed * 4f : MovementSpeed;
            transform.position += transform.rotation * movementDirection * actualMovementSpeed * Distance * Time.deltaTime;

            if (MouseMovementOn)
            {
                if (Input.mousePosition.x <= Screen.width * EdgeSensitivity && Input.mousePosition.x >= 0.0f)
                {
                    transform.position += transform.rotation * new Vector3(-actualMovementSpeed * Distance * Time.deltaTime, 0.0f, 0.0f);
                }

                if (Input.mousePosition.x >= Screen.width * (1.0f - EdgeSensitivity) && Input.mousePosition.x <= Screen.width)
                {
                    transform.position += transform.rotation * new Vector3(actualMovementSpeed * Distance * Time.deltaTime, 0.0f, 0.0f);
                }

                if (Input.mousePosition.y <= Screen.height * EdgeSensitivity && Input.mousePosition.y >= 0.0f)
                {
                    transform.position += transform.rotation * new Vector3(0.0f, 0.0f, -actualMovementSpeed * Distance * Time.deltaTime);
                }

                if (Input.mousePosition.y >= Screen.height * (1.0f - EdgeSensitivity) && Input.mousePosition.y <= Screen.height)
                {
                    transform.position += transform.rotation * new Vector3(0.0f, 0.0f, actualMovementSpeed * Distance * Time.deltaTime);
                }
            }
            if (GameInfoReader)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, GameInfoReader.MapSize * GameInfoReader.GridSize * -0.5f + GameSettings.WorldBoundaryRadius, GameInfoReader.MapSize * GameInfoReader.GridSize * 0.5f - GameSettings.WorldBoundaryRadius), 0f, Mathf.Clamp(transform.position.z, GameInfoReader.MapSize * GameInfoReader.GridSize * -0.5f + GameSettings.WorldBoundaryRadius, GameInfoReader.MapSize * GameInfoReader.GridSize * 0.5f - GameSettings.WorldBoundaryRadius));
            }
        }

        private void UpdateRotation(float r)
        {
            Rotation = r;
            transform.rotation = Quaternion.AngleAxis(r, new Vector3(0.0f, 1.0f, 0.0f));
        }

        private void UpdateDistance(float d)
        {
            float angleMax = 60.0f;
            float scale = 0.05f;
            float angle = angleMax * (1.0f - 1.0f / (scale * d + 1.0f));
            CameraTransform.localPosition = new Vector3(0.0f, d * Mathf.Sin(Mathf.Deg2Rad * angle), -d * Mathf.Cos(Mathf.Deg2Rad * angle));
            CameraTransform.localRotation = Quaternion.AngleAxis(angle, new Vector3(1.0f, 0.0f, 0.0f));
        }
    }
}
