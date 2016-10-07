package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.animal.{AnimalGameInfoState, AnimalGameInfoStateWriter, AnimalMemoryStateWriter}
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.{GameSettings, GameUtils, MathUtils, ProtoFloatArray}

class AnimalMemoryBehaviour(entity: Entity, world: World, logger: Logger, animalGameInfoStateWriter: AnimalGameInfoStateWriter, animalMemoryStateWriter: AnimalMemoryStateWriter) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      updateMemoryMap()
    }
    entity.watch[AnimalGameInfoState].bind.mapSize {
      mapSize => initialiseMemoryMap()
    }
    entity.watch[AnimalGameInfoState].bind.gridSize {
      gridSize => initialiseMemoryMap()
    }
  }

  def initialiseMemoryMap(): Unit = {
    animalMemoryStateWriter.update.memoryMap(GameUtils.arrayToList2d(Array.ofDim[Float](animalGameInfoStateWriter.mapSize, animalGameInfoStateWriter.mapSize))).finishAndSend()
  }

  def updateMemoryMap(): Unit = {
    val coordinates = getMapCoordinate(entity.position.toVector3d)
    if (coordinates._1 >= 0 && coordinates._1 < animalGameInfoStateWriter.mapSize && coordinates._2 >= 0 && coordinates._2 < animalGameInfoStateWriter.mapSize) {
      animalMemoryStateWriter.update
        .memoryMap(insertIntoMemoryMap(animalMemoryStateWriter.memoryMap, coordinates._1, coordinates._2, GameSettings.MemoryGain))
        .finishAndSend()
    }
  }

  def getMapCoordinate(position: Vector3d): (Int, Int) = {
    val xCoord = math.floor(position.x / animalGameInfoStateWriter.gridSize) + animalGameInfoStateWriter.mapSize / 2
    val yCoord = math.floor(position.z / animalGameInfoStateWriter.gridSize) + animalGameInfoStateWriter.mapSize / 2
    (xCoord.toInt, yCoord.toInt)
  }

  def insertIntoMemoryMap(memoryMap: List[ProtoFloatArray], xCoord: Int, yCoords: Int, difference: Float): List[ProtoFloatArray] = {
    val arrayMap = GameUtils.listToArray2d(animalMemoryStateWriter.memoryMap)
    arrayMap(xCoord)(yCoords) = MathUtils.clamp(arrayMap(xCoord)(yCoords) + difference, 0f, 1f)
    GameUtils.arrayToList2d(arrayMap)
  }
}
