package improbable.behaviours.animal.statemachine

import improbable.logging.Logger
import improbable.animal._
import improbable.math.Vector3d
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.AnimalResource.AnimalResource
import improbable.util._

import scala.collection.mutable.ListBuffer

trait AnimalTargetPickingBehaviourInterface extends EntityBehaviourInterface {
  //def pickNeed(): Unit
  //def setCooldowns(): Unit
}

class AnimalTargetPickingBehaviour(entity: Entity, world: World, logger: Logger, animalInfoStateWriter: AnimalInfoStateWriter, animalPerceptionStateWriter: AnimalPerceptionStateWriter, animalStateMachineStateWriter: AnimalStateMachineStateWriter) extends EntityBehaviour with AnimalTargetPickingBehaviourInterface {

  /*
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      decrementCooldowns()
    }
  }

  def pickNeed(): Unit = {
    val consideredOrderedNeeds = AnimalProperties.getAnimalResourceStatus(entity).filter(elem => !animalStateMachineStateWriter.needsCooldowns.keys.toList.map(AnimalResource(_)).contains(elem._1)).sortBy(_._2)
    val consideredEnvironment = animalPerceptionStateWriter.perceivedEnvironment.values.toList.filter(elem => !animalStateMachineStateWriter.entityCooldowns.contains(elem.entityId))
    val consideredAnimals = animalPerceptionStateWriter.perceivedAnimals.values.toList.filter(elem => !animalStateMachineStateWriter.entityCooldowns.contains(elem.entityId))
    var needPicked = false

    consideredOrderedNeeds.foreach {
      need =>
        if (!needPicked) { // pseudo break
          val targetEnvironmentEntities = getNotOccludedNeedTargets(need._1, animalPerceptionStateWriter.perceivedEnvironment.values.toList)
          if (targetEntities.nonEmpty) { //} && (Random.nextFloat() + (100f - need._2) / 100f >= 1f)) {
            //pick need
            needPicked = true
            if (!(need._1 == AnimalResource.Mating && animalInfoStateWriter.gender == AnimalGender.Female)) {
              val targetEntity = targetEntities.sortBy(elem => elem.position.distanceTo(entity.position.toVector3d)).head //always pick closest target
              animalStateMachineStateWriter.update
                .targetNeed(need._1)
                .targetEntity(targetEntity.entityId)
                .finishAndSend()
            }
          }
        }
    }
    // reset state
    if (!needPicked) {
      animalStateMachineStateWriter.update
        .targetNeed(AnimalResource.Invalid)
        .targetEntity(-1)
        .finishAndSend()
    }
  }

  def getNotOccludedNeedTargets(need: AnimalResource, entities: List[PerceivedEntity]): List[PerceivedEntity] = {
    val contextMap = new Array[Boolean](8)
    var notOccludedNeedTargets = new ListBuffer[PerceivedEntity]()
    entities.sortBy(elem => entity.position.toVector3d.distanceTo(elem.position)).foreach {
      elem =>
        if (AnimalTargetPickingBehaviour.isNeedTarget(need, entity, elem) && !contextMap(getHemisphereSlot(entity.position.toVector3d, elem.position, 8))) {
          notOccludedNeedTargets += elem
        } else if (AnimalTargetPickingBehaviour.isObstacle(entity, elem)){
          contextMap(getHemisphereSlot(entity.position.toVector3d, elem.position, 8)) = true
        }
    }
    notOccludedNeedTargets.toList
  }

  // returns a value between 0 and 7 based on direction
  def getHemisphereSlot(myPos: Vector3d, otherPos: Vector3d, numSlots: Int): Int = {
    val direction = (otherPos - myPos).normalised
    val angle = math.atan2(direction.z, direction.x) / math.Pi // from -1 to 1
    (math.min((angle + 1.0) / 2.0, 0.9999) * numSlots).toInt
  }

  def decrementCooldowns(): Unit = {
    animalStateMachineStateWriter.update
      .entityCooldowns(animalStateMachineStateWriter.entityCooldowns.map(elem => (elem._1, elem._2 - 1.0f)).filter(elem => elem._2 > 0.0f))
      .needsCooldowns(animalStateMachineStateWriter.needsCooldowns.map(elem => (elem._1, elem._2 - 1.0f)).filter(elem => elem._2 > 0.0f))
      .finishAndSend()
  }

  def setCooldowns(): Unit = {
    val oldTargetEntity = animalStateMachineStateWriter.targetEntity
    val oldNeed = animalStateMachineStateWriter.targetNeed
    if (oldTargetEntity != -1) {
      animalStateMachineStateWriter.update.entityCooldowns(animalStateMachineStateWriter.entityCooldowns + (oldTargetEntity -> GameSettings.EntityNeedCooldown)).finishAndSend()
    }
    if (oldNeed != AnimalResource.Invalid) {
      animalStateMachineStateWriter.update.needsCooldowns(animalStateMachineStateWriter.needsCooldowns + (oldNeed.id -> GameSettings.EntityNeedCooldown)).finishAndSend()
    }
  }*/
}
