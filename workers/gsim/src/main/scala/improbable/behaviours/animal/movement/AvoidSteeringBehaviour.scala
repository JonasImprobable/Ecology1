package improbable.behaviours.animal.movement

import improbable.logging.Logger
import improbable.animal._
import improbable.behaviours.animal.statemachine.AnimalTargetPickingBehaviour
import improbable.math.Vector3d
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.{GameSettings, MathUtils, SteeringSourceType}

class AvoidSteeringBehaviour(entity: Entity, world: World, logger: Logger, animalMovementStateWriter: AnimalMovementStateWriter, animalPerceptionStateWriter: AnimalPerceptionStateWriter, animalGameInfoStateWriter: AnimalGameInfoStateWriter) extends EntityBehaviour {
  /*
  val avoidEntitiesWeight = 2f
  val avoidWorldBoundariesWeight = 8f

  override def onReady(): Unit = {
    world.timing.every(GameSettings.AnimalTickRate) {
      val steeringSource = SteeringSource(getTargetDirection(), 1f)
      animalMovementStateWriter.update.steeringSources(animalMovementStateWriter.steeringSources + (SteeringSourceType.Avoid.id -> steeringSource)).finishAndSend()
    }
  }

  def getTargetDirection(): Vector3d = {
    MathUtils.flattenVector(
      getAvoidEntitiesDirection * avoidEntitiesWeight +
      getAvoidWorldBoundariesDirection() * avoidWorldBoundariesWeight
    ).normalised
  }

  def getAvoidEntitiesDirection(): Vector3d = {
    val targetEntities = animalPerceptionStateWriter.perceivedEntities.values.toList.filter(AnimalTargetPickingBehaviour.isObstacle(entity, _))
    targetEntities.map(_.position).map(position => (entity.position.toVector3d - position) / math.max(0.0001, position.distanceTo(entity.position.toVector3d))).fold(Vector3d.zero)(_ + _).normalised
  }

  def getAvoidWorldBoundariesDirection(): Vector3d = {
    var targetDirection = Vector3d.zero
    if (entity.position.x < ((-animalGameInfoStateWriter.mapSize / 2) + GameSettings.WorldBoundaryRadius) * animalGameInfoStateWriter.gridSize) targetDirection += Vector3d.unitX
    if (entity.position.x > ((animalGameInfoStateWriter.mapSize / 2) - GameSettings.WorldBoundaryRadius) * animalGameInfoStateWriter.gridSize) targetDirection -= Vector3d.unitX
    if (entity.position.z < ((-animalGameInfoStateWriter.mapSize / 2) + GameSettings.WorldBoundaryRadius) * animalGameInfoStateWriter.gridSize) targetDirection += Vector3d.unitZ
    if (entity.position.z > ((animalGameInfoStateWriter.mapSize / 2) - GameSettings.WorldBoundaryRadius) * animalGameInfoStateWriter.gridSize) targetDirection -= Vector3d.unitZ
    targetDirection.normalised
  }*/
}