using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class GameSettings : MonoBehaviour
    {
        // General
        public static bool PerformanceSaverMode = true;
        public static bool AnimalJumpEnabled = false;
        public static float ParticleCooldown = 2f;
        public static float GameHourTickRate = 1f;
        public static int WorldBoundaryRadius = 5;
        
        // Animal
        public static float AnimalTickRate = 0.5f;
        public static float AnimalMovementSpeed = 200f;
        public static bool SleepAtNight = true;
        public static float SleepPropensity = 0.6f;

        // Perception
        public static float PerceptionRadius = 80f;
        public static float AvoidRadiusFactorAwake = 0.8f;
        public static float AvoidRadiusFactorAsleep = 0.5f;
        public static float AnimalInteractionRadius = 5f;
        public static float AnimalFollowMotherRadius = 0f;// todo

        // State Machine
        public static float NeedCooldownPeriod = 12f;
        public static float EntityCooldownPeriod = 24f;
        public static float MaxStateDuration = 30f;
        public static float InteractionRange = 4f;

        // Controls
        public static KeyCode ToggleCameraControls = KeyCode.F1;
        public static KeyCode ToggleAnimalGenderKey = KeyCode.G;
        public static KeyCode KillEntityKey = KeyCode.K;

        public static KeyCode SpawnLakeKey = KeyCode.Z;
        public static KeyCode SpawnSteakKey = KeyCode.X;
        public static KeyCode SpawnRockKey = KeyCode.C;

        public static KeyCode SpawnGrassKey = KeyCode.V;
        public static KeyCode SpawnBushKey = KeyCode.B;
        public static KeyCode SpawnTreeKey = KeyCode.N;

        public static KeyCode SpawnElephantKey = KeyCode.Alpha1;
        public static KeyCode SpawnGiraffeKey = KeyCode.Alpha2;
        public static KeyCode SpawnWildebeestKey = KeyCode.Alpha3;

        public static KeyCode SpawnCheetahKey = KeyCode.Alpha4;
        public static KeyCode SpawnHyenaKey = KeyCode.Alpha5;
        public static KeyCode SpawnLionKey = KeyCode.Alpha6;
    }
}
