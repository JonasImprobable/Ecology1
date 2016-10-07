using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Util;
using Improbable;
using Improbable.Player;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Player
{
    public class PlayerControlsManager : MonoBehaviour
    {
        [Require] public PlayerControlsStateWriter PlayerControlsStateWriter;
        public Camera PlayerCamera;
        public int MapSize = 100;
        public float GridSize = 4.0f;
        public int OctaveCount = 5;
        public float Persistence = 0.5f;
        public float VegetationDensity = 0.5f;

        public float Wander = 1f;
        public float Herd = 2f;
        public float Follow = 4f;
        public float Seek = 16f;
        public float Avoid = 8f;

        void OnEnable()
        {
            PlayerCamera = GetComponentInChildren<Camera>();
        }

        public void UpdateMap() //Called from Editor button
        {
            PlayerControlsStateWriter.Update.TriggerSetMapParametersEvent(MapSize, GridSize, OctaveCount, Persistence, VegetationDensity).FinishAndSend();
        }

        public void RefreshTerrain() //Called from Editor button
        {
            PlayerControlsStateWriter.Update.TriggerRefreshTerrainEvent(MapSize, GridSize, VegetationDensity).FinishAndSend();
        }

        public void SpawnAnimals() //Called from Editor button
        {
            PlayerControlsStateWriter.Update.TriggerSpawnAnimalsEvent(MapSize, GridSize).FinishAndSend();
        }

        public void SetAnimalSteeringParameters() //Called from Editor button
        {
            PlayerControlsStateWriter.Update.TriggerSetAnimalSteeringParametersEvent(Wander, Herd, Follow, Seek, Avoid).FinishAndSend();
        }

        void Update()
        {
           if (Input.GetKeyDown(GameSettings.ToggleAnimalGenderKey))
            {
                AnimalGender gender = PlayerControlsStateWriter.ActiveAnimalGender;
                switch (gender)
                {
                    case AnimalGender.Male: gender = AnimalGender.Female; break;
                    case AnimalGender.Female: gender = AnimalGender.Male; break;
                    default: break;
                }
                PlayerControlsStateWriter.Update.ActiveAnimalGender(gender).FinishAndSend();
            }

            if (Input.GetKeyDown(GameSettings.SpawnLakeKey))
            {
                SpawnEnvironment(EnvironmentType.Lake);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnSteakKey))
            {
                SpawnEnvironment(EnvironmentType.Steak);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnRockKey))
            {
                SpawnEnvironment(EnvironmentType.Rock);
            }

            else if (Input.GetKeyDown(GameSettings.SpawnGrassKey))
            {
                SpawnEnvironment(EnvironmentType.Grass);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnBushKey))
            {
                SpawnEnvironment(EnvironmentType.Bush);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnTreeKey))
            {
                SpawnEnvironment(EnvironmentType.Tree);
            }

            else if (Input.GetKeyDown(GameSettings.SpawnElephantKey))
            {
                SpawnAnimal(AnimalType.Elephant);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnGiraffeKey))
            {
                SpawnAnimal(AnimalType.Giraffe);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnWildebeestKey))
            {
                SpawnAnimal(AnimalType.Wildebeest);
            }

            else if (Input.GetKeyDown(GameSettings.SpawnCheetahKey))
            {
                SpawnAnimal(AnimalType.Cheetah);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnHyenaKey))
            {
                SpawnAnimal(AnimalType.Hyena);
            }
            else if (Input.GetKeyDown(GameSettings.SpawnLionKey))
            {
                SpawnAnimal(AnimalType.Lion);
            }
        }

        void SpawnEnvironment(EnvironmentType environmentType)
        {
            RaycastHit hit;
            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Terrain"))))
            {
                PlayerControlsStateWriter.Update.TriggerSpawnEnvironmentEvent(environmentType, MathUtils.FlattenVector(hit.point).ToNativeVector()).FinishAndSend();
            }
        }

        void SpawnAnimal(AnimalType animalType)
        {
            RaycastHit hit;
            Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << LayerMask.NameToLayer("Terrain"))))
            {
                PlayerControlsStateWriter.Update.TriggerSpawnAnimalEvent(animalType, MathUtils.FlattenVector(hit.point).ToNativeVector()).FinishAndSend();
            }
        }

        public void KillEntity(GameObject entity) // Used in entityselector
        {
            switch (entity.tag)
            {
                case "Environment": PlayerControlsStateWriter.Update.TriggerDestroyEntityEvent(entity.EntityId()).FinishAndSend(); break;
                case "Animal":
                    if (entity.GetComponent<AnimalInfoReader>().IsAlive == AnimalVitalityStatus.Alive)
                    {
                        PlayerControlsStateWriter.Update.TriggerKillEntityEvent(entity.EntityId()).FinishAndSend();
                    }
                    else
                    {
                        PlayerControlsStateWriter.Update.TriggerDestroyEntityEvent(entity.EntityId()).FinishAndSend();
                    }
                    break;
                default: break;
            }
        }

        public void MoveEntityEvent(EntityId entityId, Vector3 position) // Used in entityselector
        {
            PlayerControlsStateWriter.Update.TriggerMoveEntityEvent(entityId, position.ToNativeVector()).FinishAndSend();
        }
    }
}
