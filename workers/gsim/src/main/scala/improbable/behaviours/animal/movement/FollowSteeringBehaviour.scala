package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal._
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.{AnimalVitalityStatus, GameSettings, MathUtils, SteeringSourceType}

class FollowSteeringBehaviour(entity: Entity, world: World, logger: Logger, animalInfoStateWriter: AnimalInfoStateWriter, animalPerceptionStateWriter: AnimalPerceptionStateWriter, animalMovementStateWriter: AnimalMovementStateWriter) extends EntityBehaviour {
  /*
  override def onReady(): Unit = {
    world.timing.every(GameSettings.AnimalTickRate) {
      val steeringSource = SteeringSource(getTargetDirection(), 1f)
      animalMovementStateWriter.update.steeringSources(animalMovementStateWriter.steeringSources + (SteeringSourceType.Follow.id -> steeringSource)).finishAndSend()
    }
  }

  def getTargetDirection(): Vector3d = {
    val motherId = animalInfoStateWriter.mother
    val followMotherActive = motherId != -1 &&
      !animalInfoStateWriter.isAdult &&
      animalPerceptionStateWriter.perceivedEntities.contains(motherId) &&
      animalPerceptionStateWriter.perceivedEntities(motherId).perceivedAnimalInfo.isAlive == AnimalVitalityStatus.Alive
    if (followMotherActive) {
      MathUtils.flattenVector(animalPerceptionStateWriter.perceivedEntities(animalInfoStateWriter.mother).position - entity.position.toVector3d).normalised
    } else {
      Vector3d.zero
    }
  }*/
}
