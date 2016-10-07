package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.animal._
import improbable.papi.EntityId
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.papi.world.messaging.CustomMsg
import improbable.util._

import scala.util.Random

case class MatingRequestMsg(sender: EntityId) extends CustomMsg
case object MatingAckMsg extends CustomMsg
case object MatingFinishedMsg extends CustomMsg

class MatingBehaviour(entity: Entity, world: World, logger: Logger, animalGsimStateWriter: AnimalGsimStateWriter, animalInfoStateWriter: AnimalInfoStateWriter, animalResourceManagementBehaviourInterface: AnimalResourceManagementBehaviourInterface) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate * 100){
      if (animalInfoStateWriter.gender == AnimalGender.Female && Random.nextFloat() > 0.9) {
        getPregnant()
      }
    }

    world.timing.every(GameSettings.GameHourTickRate) {
      val animalStateMachineStateWatcher = entity.watch[AnimalStateMachineState]

      if (animalInfoStateWriter.gender == AnimalGender.Male && animalStateMachineStateWatcher.currentState.get == AnimalState.SeekingEntity && animalStateMachineStateWatcher.targetNeed.get == AnimalResource.Mating) {
        world.messaging.sendToEntity(animalStateMachineStateWatcher.targetEntity.get, MatingRequestMsg(entity.entityId))
      }
      if (animalInfoStateWriter.gender == AnimalGender.Male && animalStateMachineStateWatcher.currentState.get == AnimalState.Interacting && animalStateMachineStateWatcher.targetNeed.get == AnimalResource.Mating) {
        if (animalStateMachineStateWatcher.currentStateDuration.get > GameSettings.MatingDuration) {
          world.messaging.sendToEntity(animalStateMachineStateWatcher.targetEntity.get, MatingFinishedMsg)
          animalGsimStateWriter.update.triggerTransitionToStateEventData(AnimalState.Neutral).finishAndSend() ///
          //animalNeedPickingBehaviourInterface.setCooldowns()
          //animalStateMachineBehaviourInterface.transitionToState(AnimalState.Neutral)
          animalResourceManagementBehaviourInterface.setResource(AnimalResource.Mating, 100f)
        }
      }
    }
    world.messaging.onReceive {
      //sent to females
      case MatingRequestMsg(sender) => {
        if (true) { //(Random.nextFloat() + (100f - animalInfoStateWriter.mating) / 100f >= 1f) {
          world.messaging.sendToEntity(sender, MatingAckMsg)
          animalGsimStateWriter.update.triggerTransitionToStateEventData(AnimalState.Interacting).finishAndSend() ///
          //animalStateMachineStateWriter.update.targetNeed(AnimalResource.Mating).targetEntity(sender).currentState(AnimalState.Interacting).currentStateDuration(0.0f).finishAndSend()
            // todo: Manual state transition to ensure heart particle shows up at females
        }
      }
      //sent to males
      case MatingAckMsg => {
        animalGsimStateWriter.update.triggerTransitionToStateEventData(AnimalState.Interacting).finishAndSend() ///
        //animalStateMachineBehaviourInterface.transitionToState(AnimalState.Interacting)
      }
      //sent to females
      case MatingFinishedMsg => {
        //animalNeedPickingBehaviourInterface.setCooldowns()
        //animalStateMachineBehaviourInterface.transitionToState(AnimalState.Neutral)
        animalGsimStateWriter.update.triggerTransitionToStateEventData(AnimalState.Neutral).finishAndSend() ///
        animalResourceManagementBehaviourInterface.setResource(AnimalResource.Mating, 100f)
        getPregnant()
      }
    }
  }

  def getPregnant(): Unit = {
    animalInfoStateWriter.update.isPregnant(true).remainingPregnancyTime(AnimalProperties.getGestationPeriodFromType(animalInfoStateWriter.species)).finishAndSend()
  }
}
