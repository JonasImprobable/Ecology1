using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Global;
using Assets.Gamelogic.Visualizers.Ui;
using Assets.Gamelogic.Visualizers.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Player
{
    public class EntitySelector : MonoBehaviour
    {
        public PlayerControlsManager PlayerControlsManager;
        public Camera PlayerCamera;
        public GameObject CurrentSelection;
        public AnimalStateMachineReader SelectionAnimalStateMachineReader; // temporary hack
        public GameObject SelectionIndicatorPrefab;
        public GameObject RangeIndicatorPrefab;
        public GameObject SelectionIndicator;
        public GameObject PerceptionRangeIndicator;
        public GameObject AvoidRangeIndicator;
        public GameInfoReader GameInfoReader;

        void OnEnable()
        {
            SelectionIndicatorPrefab = Resources.Load<GameObject>("Indicators/SelectionIndicator");
            RangeIndicatorPrefab = Resources.Load<GameObject>("Indicators/RangeIndicatorSphere");
            PlayerCamera = GetComponentInChildren<Camera>();
            PlayerControlsManager = GetComponent<PlayerControlsManager>();
            GameInfoReader = GetComponent<GameInfoReader>();
        }
        
        void Update()
        {
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
            }


            if (Input.GetKeyDown(GameSettings.KillEntityKey))
            {
                if (CurrentSelection != null)
                {
                    PlayerControlsManager.KillEntity(CurrentSelection);
                    DeselectCurrent();
                }
            }
            if (Input.GetMouseButtonDown(1) && CurrentSelection)
            {
                RaycastHit hit;
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Terrain"))))
                {
                    PlayerControlsManager.MoveEntityEvent(CurrentSelection.EntityId(), hit.point);
                }      
            }
            if (Input.GetMouseButtonDown(0))
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    if (hit.collider.gameObject.IsEntityObject())
                    {
                        GameObject newSelection = hit.collider.gameObject;
                        if (newSelection != CurrentSelection)
                        {
                            DeselectCurrent();
                            switch (hit.collider.gameObject.tag)
                            {
                                case "Environment": SelectEnvironment(newSelection); break;
                                case "Animal": SelectAnimal(newSelection); break;
                                default: break;
                            }
                        }
                    }
                    else
                    {
                        DeselectCurrent();
                    }
                }
            }

            if (CurrentSelection != null && CurrentSelection.tag == "Animal")
            {
                //CurrentSelection.GetComponent<AnimalMemoryReader>().DrawMemoryHeatMap();
                //temporary hack
                if (AvoidRangeIndicator)
                {
                    float AvoidRadiusFactor = AnimalProperties.GetAvoidDistance(SelectionAnimalStateMachineReader.CurrentState);
                    AvoidRangeIndicator.transform.localScale = new Vector3(AvoidRadiusFactor * 2.0f, 1.0f, AvoidRadiusFactor * 2.0f);
                }
            }
            
            if (CurrentSelection != null && !CurrentSelection.gameObject.activeSelf)
            {
                DeselectCurrent();
            }
        }

        void SelectAnimal(GameObject newSelection)
        {
            CurrentSelection = newSelection;

            //Selection Indicator
            SelectionIndicator = (GameObject)Instantiate(SelectionIndicatorPrefab, newSelection.transform.position, Quaternion.identity);
            SelectionIndicator.transform.localScale = new Vector3(newSelection.gameObject.GetComponent<BoxCollider>().size.z, 1.0f, newSelection.gameObject.GetComponent<BoxCollider>().size.z);
            SelectionIndicator.GetComponent<FollowParent>().Parent = newSelection;

            //Perception Range Indicator
            PerceptionRangeIndicator = (GameObject)Instantiate(RangeIndicatorPrefab, newSelection.transform.position, Quaternion.identity);
            PerceptionRangeIndicator.transform.localScale = new Vector3(GameSettings.PerceptionRadius * 2.0f, 1.0f, GameSettings.PerceptionRadius * 2.0f);
            PerceptionRangeIndicator.GetComponent<FollowParent>().Parent = newSelection;

            //Avoid Range Indicator
            AvoidRangeIndicator = (GameObject)Instantiate(RangeIndicatorPrefab, newSelection.transform.position, Quaternion.identity);
            SelectionAnimalStateMachineReader = newSelection.GetComponent<AnimalStateMachineReader>();
            float AvoidRadiusFactor = AnimalProperties.GetAvoidDistance(SelectionAnimalStateMachineReader.CurrentState);
            AvoidRangeIndicator.transform.localScale = new Vector3(AvoidRadiusFactor * 2.0f, 1.0f, AvoidRadiusFactor * 2.0f);
            AvoidRangeIndicator.GetComponent<Renderer>().material.color = new Color(1f, 0.17f, 0.17f, 0.2f);
            AvoidRangeIndicator.GetComponent<FollowParent>().Parent = newSelection;

            // Memory heat map
            //CurrentSelection.GetComponent<AnimalMemoryReader>().DrawMemoryHeatMap();

            EntityInspectorUi.SelectAnimal(newSelection);
        }

        void SelectEnvironment(GameObject newSelection)
        {
            //Selection Indicator
            SelectionIndicator = (GameObject)Instantiate(SelectionIndicatorPrefab, newSelection.transform.position, newSelection.transform.rotation);
            SelectionIndicator.transform.localScale = new Vector3(newSelection.transform.localScale.z, 1.0f, newSelection.transform.localScale.z);
            SelectionIndicator.GetComponent<FollowParent>().Parent = newSelection;
            CurrentSelection = newSelection;

            EntityInspectorUi.SelectEnvironment(newSelection);
        }

        void DeselectCurrent()
        {
            if (SelectionIndicator != null)
            {
                Destroy(SelectionIndicator);
            }
            if (PerceptionRangeIndicator != null)
            {
                Destroy(PerceptionRangeIndicator);
            }
            if (AvoidRangeIndicator != null)
            {
                Destroy(AvoidRangeIndicator);
            }
            if (CurrentSelection != null && CurrentSelection.tag == "Animal")
            {
                CurrentSelection.GetComponent<AnimalMemoryReader>().InValidateMemoryMap();
            }

            CurrentSelection = null;
            SelectionAnimalStateMachineReader = null;
            EntityInspectorUi.DeselectCurrent();

            //if (GameInfoReader) GameInfoReader.DrawTerrainHeatMap();
        }
    }
}
