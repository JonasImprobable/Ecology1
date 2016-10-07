package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal.{AnimalMovementStateWriter, SteeringSource}
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.{GameSettings, MathUtils, SteeringSourceType}

import scala.util.Random

class WanderSteeringBehaviour(entity: Entity, world: World, animalMovementStateWriter: AnimalMovementStateWriter, logger: Logger) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.AnimalTickRate * 4) {
      val steeringSource = SteeringSource(getTargetDirection(), 1f)
      animalMovementStateWriter.update.steeringSources(animalMovementStateWriter.steeringSources + (SteeringSourceType.Wander.id -> steeringSource)).finishAndSend()
    }
  }

  def getTargetDirection(): Vector3d = {
    MathUtils.flattenVector(MathUtils.getRandomFlatVector()).normalised
  }
}
