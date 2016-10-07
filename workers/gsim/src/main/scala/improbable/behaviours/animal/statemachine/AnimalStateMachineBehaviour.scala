package improbable.behaviours.animal.statemachine

import improbable.logging.Logger
import improbable.animal._
import improbable.behaviours.animal.{DeathBehaviourInterface, DeathMsg}
import improbable.behaviours.player.MoveEntityMsg
import improbable.math.Vector3d
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.AnimalState.AnimalState
import improbable.util._

import scala.util.Random

class AnimalStateMachineBehaviour(entity: Entity, world: World, logger: Logger, animalStateMachineStateWriter: AnimalStateMachineStateWriter, animalInfoStateWriter: AnimalInfoStateWriter, deathBehaviourInterface: DeathBehaviourInterface) extends EntityBehaviour {
  override def onReady(): Unit = {
    entity.watch[AnimalStateMachineState].onAttackPreyEvent {
      case AttackPreyEventData(targetEntityId) => {
        world.messaging.sendToEntity(targetEntityId, DeathMsg(false))
      }
    }

    world.messaging.onReceive {
      case MoveEntityMsg(position) => {
        animalStateMachineStateWriter.update.targetPosition(position).currentState(AnimalState.SeekingPosition).finishAndSend() // why is this even working? ;)
      }
    }
  }
}
