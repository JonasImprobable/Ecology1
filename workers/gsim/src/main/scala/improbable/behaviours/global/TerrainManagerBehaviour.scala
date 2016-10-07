package improbable.behaviours.global

import improbable.logging.Logger
import improbable.global.GameInfoStateWriter
import improbable.math.Vector3d
import improbable.natures.animal.BaseAnimalNature
import improbable.natures.environment.BaseEnvironmentNature
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.AnimalGender.AnimalGender
import improbable.util.AnimalType.AnimalType
import improbable.util.EnvironmentType.EnvironmentType
import improbable.util._

import scala.concurrent.duration._
import scala.util.Random

trait TerrainManagerBehaviourInterface extends EntityBehaviourInterface {
  def generateTerrainNoiseMap(mapSize: Int, octaveCount: Int, persistence: Float): Unit
  def spawnEnvironment(mapSize: Int, gridSize: Float, vegetationDensity: Float): Unit
  def spawnAnimals(mapSize: Int, gridSize: Float): Unit
}

class TerrainManagerBehaviour(entity: Entity, world: World, logger: Logger, gameInfoStateWriter: GameInfoStateWriter, entityManagerBehaviourInterface: EntityManagerBehaviourInterface) extends EntityBehaviour with TerrainManagerBehaviourInterface {
  var terrainNoiseMap: Array[Array[Float]] = Array.ofDim[Float](0,0)

  override def onReady(): Unit = {
    if (GameFlags.isLocal.get) {
      logger.info("Running with configuration: local")
      generateTerrainNoiseMap(gameInfoStateWriter.mapSize, gameInfoStateWriter.octaveCount, gameInfoStateWriter.persistence)
      spawnEnvironment(gameInfoStateWriter.mapSize, gameInfoStateWriter.gridSize, gameInfoStateWriter.vegetationDensity)
      spawnAnimals(gameInfoStateWriter.mapSize, gameInfoStateWriter.gridSize)

    } else {
      logger.info("Running with configuration: deployment")
      generateTerrainNoiseMap(gameInfoStateWriter.mapSize, gameInfoStateWriter.octaveCount, gameInfoStateWriter.persistence)
      spawnEnvironment(gameInfoStateWriter.mapSize, gameInfoStateWriter.gridSize, gameInfoStateWriter.vegetationDensity)
      spawnAnimals(gameInfoStateWriter.mapSize, gameInfoStateWriter.gridSize)
    }
  }

  def generateTerrainNoiseMap(mapSize: Int, octaveCount: Int, persistence: Float): Unit = {
    terrainNoiseMap = MathUtils.generatePerlinNoise(mapSize, mapSize, octaveCount, persistence)
    gameInfoStateWriter.update.terrainNoiseMapScaled(GameUtils.arrayToList2d(rescaleTerrainNoiseMap(terrainNoiseMap))).finishAndSend()
  }

  def rescaleTerrainNoiseMap(terrainNoiseMap: Array[Array[Float]], miniMapResolution: Int = GameSettings.MiniMapResolution): Array[Array[Float]] = {
    val result = Array.ofDim[Float](miniMapResolution, miniMapResolution)
    for {
      i <- terrainNoiseMap.indices
      j <- terrainNoiseMap(i).indices
    } {
      val xCoord = Math.floor(i.toFloat / terrainNoiseMap.length * miniMapResolution).toInt
      val yCoord = Math.floor(j.toFloat / terrainNoiseMap.length * miniMapResolution).toInt
      result(xCoord)(yCoord) += terrainNoiseMap(i)(j)
    }
    val max = result.map(_.max).max
    for {
      i <- result.indices
      j <- result(i).indices
    } {
      result(i)(j) /= max
    }
    result
  }

  def spawnEnvironment(mapSize: Int, gridSize: Float, vegetationDensity: Float): Unit = {
    //entityManagerBehaviourInterface.removeAllEnvironment()

    val treeNoiseThreshold = 0.9f
    val treeRadius = 3
    val bushThreshold = 0.75f
    val grassThreshold = 0.5f
    val waterUpperThreshold = 0.2f
    val waterRadius = 5
    val treeMap = Array.ofDim[Boolean](mapSize, mapSize)
    val waterMap = Array.ofDim[Boolean](mapSize, mapSize)

    for (i <- GameSettings.WorldBoundaryRadius until mapSize - GameSettings.WorldBoundaryRadius; j <- GameSettings.WorldBoundaryRadius until mapSize - GameSettings.WorldBoundaryRadius) {
      if (terrainNoiseMap(i)(j) >= treeNoiseThreshold && !treeMap(i)(j)) {
        markObstacleRadius(treeMap, i, j, treeRadius)
        spawnEnvironmentEntity(EnvironmentType.Tree, i, j, mapSize, gridSize)
      }
      else if (terrainNoiseMap(i)(j) >= bushThreshold && Random.nextFloat() + vegetationDensity >= 1.0f) {
        spawnEnvironmentEntity(EnvironmentType.Bush, i, j, mapSize, gridSize)
      }
      if (terrainNoiseMap(i)(j) >= grassThreshold && Random.nextFloat() + vegetationDensity >= 1.0f) {
        spawnEnvironmentEntity(EnvironmentType.Grass, i, j, mapSize, gridSize)
      }
      if (terrainNoiseMap(i)(j) < waterUpperThreshold && !waterMap(i)(j)) {
        markObstacleRadius(waterMap, i, j, waterRadius)
        spawnEnvironmentEntity(EnvironmentType.Lake, i, j, mapSize, gridSize)
      }
    }
    logger.info("Terrain spawned.")
  }

  def spawnEnvironmentEntity(environmentType: EnvironmentType, x: Int, y: Int, mapSize: Int, gridSize: Float): Unit = {
    val resources = (MathUtils.clamp(Random.nextFloat(), 0.2f, 1.0f) * 100.0f).toInt.toFloat
    val posOffsetX = Random.nextFloat() * gridSize
    val posOffsetY = Random.nextFloat() * gridSize
    val xPos = ((x - (mapSize / 2)) * gridSize) + posOffsetX
    val yPos = ((y - (mapSize / 2)) * gridSize) + posOffsetY
    entityManagerBehaviourInterface.spawnEntity(BaseEnvironmentNature(Vector3d(xPos, 0.0, yPos), environmentType ,resources), EntityCategory.Environment)
  }

  def markObstacleRadius(map: Array[Array[Boolean]], x: Int, y: Int, radius: Int): Unit = {
    val xMin = math.max(0, x - radius)
    val xMax = math.min(map.length, x + radius + 1)
    val yMin = math.max(0, y - radius)
    val yMax = math.min(map(0).length, y + radius + 1)
    for (i <- xMin until xMax; j <- yMin until yMax) {
      if (gridDistance(x, y, i, j) <= radius) {
        map(i)(j) = true
      }
    }
  }

  def gridDistance(x0: Int, y0: Int, x1: Int, y1: Int): Int = {
    math.abs(x1 - x0) + math.abs(y1 - y0)
  }

  def spawnAnimals(mapSize: Int, gridSize: Float): Unit = {
    //entityManagerBehaviourInterface.removeAllAnimals()

    val herdDensity = 0.002f
    val herdRadiusInner = 10
    val herdRadiusOuter = 20
    val herdMaleRatio = 0.1f
    val herdSize = 10.0f
    val animalMap = Array.ofDim[Boolean](mapSize, mapSize)

    var animalCount = 0

    for (i <- GameSettings.WorldBoundaryRadius + herdRadiusInner until mapSize - herdRadiusInner - GameSettings.WorldBoundaryRadius; j <- GameSettings.WorldBoundaryRadius + herdRadiusInner until mapSize - herdRadiusInner - GameSettings.WorldBoundaryRadius) {
      if (Random.nextFloat() + herdDensity * (1.0f - 2 * math.abs(terrainNoiseMap(i)(j) - 0.5f)) >= 1.0f && !animalMap(i)(j)) {
        markObstacleRadius(animalMap, i, j, herdRadiusOuter)
        val species = AnimalType(Random.nextInt(6))
        //logger.info(s"Animal seed: $species")
        spawnAnimalEntity(species, AnimalGender.Male, i, j, 0, mapSize, gridSize) // seed animal, always male
        animalCount += 1
        val numHerdMembers = math.max((Random.nextGaussian() * herdSize * 0.2f + herdSize).toInt, 0)
        //logger.info(s"num: $numHerdMembers")
        for (n <- 1 to numHerdMembers) {
          val gender = if(Random.nextFloat() + herdMaleRatio >= 1.0f) AnimalGender.Male else AnimalGender.Female
          spawnAnimalEntity(species, gender, i, j, herdRadiusInner, mapSize, gridSize)
          animalCount += 1
        }
      }
    }
    logger.info(s"Animals spawned. $animalCount")
  }

  def spawnAnimalEntity(species: AnimalType, gender: AnimalGender, x: Int, y: Int, radius: Int, mapSize: Int, gridSize: Float): Unit = {
    val posOffsetX = Random.nextFloat() * gridSize + (Random.nextFloat() - 0.5f) * 2 * radius
    val posOffsetY = Random.nextFloat() * gridSize + (Random.nextFloat() - 0.5f) * 2 * radius
    val xPos = ((x - (mapSize / 2)) * gridSize) + posOffsetX
    val yPos = ((y - (mapSize / 2)) * gridSize) + posOffsetY

    val delay = (x * mapSize + y) * 100
    world.timing.after(delay.microseconds) {
      entityManagerBehaviourInterface.spawnEntity(BaseAnimalNature(Vector3d(xPos, 0.0, yPos), species, gender), EntityCategory.Animal)
    }
  }
}
