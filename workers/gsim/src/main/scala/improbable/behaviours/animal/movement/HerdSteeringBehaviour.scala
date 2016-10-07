package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal._
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util._

class HerdSteeringBehaviour(entity: Entity, world: World, logger: Logger, animalMovementStateWriter: AnimalMovementStateWriter, animalPerceptionStateWriter: AnimalPerceptionStateWriter, animalInfoStateWriter: AnimalInfoStateWriter, animalStateMachineStateWriter: AnimalStateMachineStateWriter) extends EntityBehaviour {
  val alignmentWeight = 1f
  val cohesionWeight = 1f
  val separationWeight = 1f
/*
  override def onReady(): Unit = {
    world.timing.every(GameSettings.AnimalTickRate) {
      val steeringSource = SteeringSource(getTargetDirection(), 1f)
      animalMovementStateWriter.update.steeringSources(animalMovementStateWriter.steeringSources + (SteeringSourceType.Herd.id -> steeringSource)).finishAndSend()
    }
  }

  def isTargetEntity(entity: PerceivedEntity): Boolean = {
    entity.entityCategory == EntityCategory.Animal &&
      entity.perceivedAnimalInfo.species == animalInfoStateWriter.species &&
      entity.perceivedAnimalInfo.isAlive == AnimalVitalityStatus.Alive
  }

  def getTargetDirection(): Vector3d = {
    val targetEntities = animalPerceptionStateWriter.perceivedEntities.values.toList.filter(isTargetEntity)
    MathUtils.flattenVector(getAlignmentDirection(targetEntities) * alignmentWeight +
      getCohesionDirection(targetEntities) * cohesionWeight +
      getSeparationDirection(animalPerceptionStateWriter.perceivedEntities.values.toList) * separationWeight
    )
  }

  // Steer towards average direction of mates
  def getAlignmentDirection(targetEntities: List[PerceivedEntity]): Vector3d = {
    val averageVelocity = MathUtils.getAverageVector(targetEntities.map(_.velocity))
    if(averageVelocity.magnitude >= 0.01) {
      averageVelocity.normalised
    } else {
      Vector3d.zero
    }
  }

  // Move towards average position of mates
  def getCohesionDirection(targetEntities: List[PerceivedEntity]): Vector3d = {
    if (targetEntities.isEmpty) Vector3d.zero
    else {
      val averagePosition = MathUtils.getAverageVector(targetEntities.map(_.position))
      (averagePosition - entity.position.toVector3d).normalised
    }
  }

  // Avoid others, scaled by distance to entity
  // feature creep slightly avoid other species here
  def getSeparationDirection(perceivedEntities: List[PerceivedEntity]): Vector3d = {
    val targetEntities = perceivedEntities
      .filter(elem => elem.entityCategory == EntityCategory.Animal && elem.perceivedAnimalInfo.isAlive == AnimalVitalityStatus.Alive)
      .filter(_.entityId != animalStateMachineStateWriter.targetEntity) // don't avoid target entity
    targetEntities.map(_.position).map(position => (entity.position.toVector3d - position) / math.max(0.0001, position.distanceTo(entity.position.toVector3d))).fold(Vector3d.zero)(_ + _).normalised
  }*/
}
