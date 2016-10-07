package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal._
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.util.{AnimalState, MathUtils, SteeringSourceType}
import improbable.util.AnimalState.AnimalState
import improbable.util.SteeringSourceType.SteeringSourceType

class SteeringAggregationBehaviour(entity: Entity, logger: Logger, animalMovementStateWriter: AnimalMovementStateWriter, animalStateMachineStateWriter: AnimalStateMachineStateWriter, animalInfoStateWriter: AnimalInfoStateWriter) extends EntityBehaviour {
  def getSteeringSourceWeight(state: AnimalState, steeringSource: SteeringSourceType): Float = {
    val values = Map[AnimalState, Map[SteeringSourceType, Float]](
      AnimalState.Neutral -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 1f,
        SteeringSourceType.Herd -> 1f,
        SteeringSourceType.Follow -> 1f,
        SteeringSourceType.Seek -> 0f,
        SteeringSourceType.Avoid -> 1f
      ),
      AnimalState.SeekingEntity -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 0f,
        SteeringSourceType.Herd -> 0f,
        SteeringSourceType.Follow -> 0f,
        SteeringSourceType.Seek -> 1f,
        SteeringSourceType.Avoid -> 0f
      ),
      AnimalState.Interacting -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 0f,
        SteeringSourceType.Herd -> 0f,
        SteeringSourceType.Follow -> 0f,
        SteeringSourceType.Seek -> 0f,
        SteeringSourceType.Avoid -> 0f
      ),
      AnimalState.Hunting -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 0f,
        SteeringSourceType.Herd -> 0f,
        SteeringSourceType.Follow -> 0f,
        SteeringSourceType.Seek -> 1f,
        SteeringSourceType.Avoid -> 0f
      ),
      AnimalState.Fleeing -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 1f,
        SteeringSourceType.Herd -> 1f,
        SteeringSourceType.Follow -> 1f,
        SteeringSourceType.Seek -> 0f,
        SteeringSourceType.Avoid -> 1f
      ),
      AnimalState.SeekingPosition -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 0f,
        SteeringSourceType.Herd -> 0f,
        SteeringSourceType.Follow -> 0f,
        SteeringSourceType.Seek -> 1f,
        SteeringSourceType.Avoid -> 0f
      ),
      AnimalState.Sleeping -> Map[SteeringSourceType, Float](
        SteeringSourceType.Wander -> 0f,
        SteeringSourceType.Herd -> 0f,
        SteeringSourceType.Follow -> 0f,
        SteeringSourceType.Seek -> 0f,
        SteeringSourceType.Avoid -> 0f
      )
    )
    values(state)(steeringSource)
  }

  def getMovementSpeedFromState(state: AnimalState): Float = {
    val values = Map[AnimalState, Float](
      AnimalState.Neutral -> 0.5f,
      AnimalState.SeekingEntity -> 0.5f,
      AnimalState.Interacting -> 0f,
      AnimalState.Hunting -> 1f,
      AnimalState.Fleeing -> 1f,
      AnimalState.SeekingPosition -> 0.5f,
      AnimalState.Sleeping -> 0f
    )
    values(state)
  }

  override def onReady(): Unit = {
    entity.watch[AnimalMovementState].bind.steeringSources {
      steeringSources => {
        val steeringSourceAggregate = SteeringSource(getTargetDirection(steeringSources.map(elem => (SteeringSourceType(elem._1), elem._2))), getTargetIntensity())
        animalMovementStateWriter.update.steeringSourceAggregate(steeringSourceAggregate).finishAndSend()
      }
    }
  }

  def getTargetDirection(steeringSources: Map[SteeringSourceType, SteeringSource]): Vector3d = {
    val currentState = animalStateMachineStateWriter.currentState
    val sumWeightedVelocities = steeringSources.map(elem => elem._2.direction * elem._2.intensity * getSteeringSourceWeight(currentState, elem._1) * entity.watch[AnimalGameInfoState].steeringSourceWeights.get(elem._1.id)).fold(Vector3d.zero)(_ + _)
    MathUtils.flattenVector(sumWeightedVelocities).normalised
  }

  def getTargetIntensity(): Float = {
    val staminaFactor = animalInfoStateWriter.stamina / 100f
    val intensityFactor = (getMovementSpeedFromState(animalStateMachineStateWriter.currentState) - 0.5f) / 0.5f
    0.5f + (0.5f * staminaFactor * intensityFactor)
  }
}
