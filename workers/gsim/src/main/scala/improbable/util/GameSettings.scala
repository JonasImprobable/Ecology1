package improbable.util

import scala.concurrent.duration._

object GameSettings {
  // Game
  val GlobalEntityTag = "Global"
  val PlayerTag = "player"
  val AnimalTag = "animal"
  val EnvironmentTag = "environment" // used for global searches for admin wipes

  val ClientCheckoutRadius = 4
  val FsimCheckoutRadius = 2
  val GameHourTickRate = 1.seconds

  val WorldBoundaryRadius = 5

  val MiniMapResolution = 100

  // Entity
  val EntityMinSize = 0.2f
  val EntityMaxSize = 2f

  // Terrain
  val MapSize = 1000
  val GridSize = 8f
  val OctaveCount = 5
  val Persistence = 0.5f
  val VegetationDensity = 0.6f

  val LocalMapSize = 100
  val MiniMapScaler = 100f

  // Animals
  val AnimalTickRate = 500.milliseconds
  val ImmortalAnimals = false

  /*
  // Perception
  val PerceptionRadius = 80f
  val AvoidRadiusFactorAwake = 0.7f
  val AvoidRadiusFactorAsleep = 0.4f
  val AnimalInteractionRadius = 5.0f
  val AnimalFollowMotherRadius = 0f // todo
  */

  // Resources
  val ResourceLossCooldown = 5f
  val WaterLoss = 1f
  val FoodLoss = 0.5f
  val MatingLoss = 1f
  val WaterGain = 10f
  val FoodGain = 10f
  val StaminaLoss = 4f
  val StaminaGain = 4f
  val EnvResourcesGain = 1f
  val EnvResourcesLoss = 1f

  // States
  val EntityNeedCooldown = 60f
  val StateMaxDuration = 30f
  val AnimalSeekSlowdownRadius = 10f
  val SleepPropensity = 0.6f

  // Reproduction
  val MatingDuration = 4f
  val LitterStandardDeviationScale = 0.25
  val MaleRatio = 0.3f

  // Memory
  val MemoryGain = 0.04f
}
