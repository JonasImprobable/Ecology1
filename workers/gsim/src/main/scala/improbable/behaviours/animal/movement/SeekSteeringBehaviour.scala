package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal._
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util._

class SeekSteeringBehaviour(entity: Entity, world: World, logger: Logger, animalMovementStateWriter: AnimalMovementStateWriter, animalStateMachineStateWriter: AnimalStateMachineStateWriter, animalPerceptionStateWriter: AnimalPerceptionStateWriter) extends EntityBehaviour {
  /*
  override def onReady(): Unit = {
    world.timing.every(GameSettings.AnimalTickRate) {
      val steeringSource = SteeringSource(getTargetDirection(), 1f)
      animalMovementStateWriter.update.steeringSources(animalMovementStateWriter.steeringSources + (SteeringSourceType.Seek.id -> steeringSource)).finishAndSend()
    }
  }

  def getTargetDirection(): Vector3d = {
    var targetDirection = Vector3d.zero
    animalStateMachineStateWriter.currentState match {
      case state if (state == AnimalState.SeekingEntity || state == AnimalState.Hunting)=> {
        if (animalPerceptionStateWriter.perceivedEntities.contains(animalStateMachineStateWriter.targetEntity)) // todo: temporary hack, unethical
          targetDirection = animalPerceptionStateWriter.perceivedEntities(animalStateMachineStateWriter.targetEntity).position - entity.position.toVector3d // original line
        else
          Vector3d.zero
      }
      case AnimalState.SeekingPosition => {
        targetDirection = animalStateMachineStateWriter.targetPosition - entity.position.toVector3d
      }
      case _ => {}
    }
    MathUtils.flattenVector(targetDirection).normalised
  }*/
}
