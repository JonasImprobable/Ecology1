package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.animal.AnimalInfoStateWriter
import improbable.behaviours.global.DestroyEntityMsg
import improbable.entity.EntityInfoStateWriter
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.papi.world.messaging.CustomMsg
import improbable.util._

import scala.concurrent.duration._

case class DeathMsg(natural: Boolean) extends CustomMsg

trait DeathBehaviourInterface extends EntityBehaviourInterface {
  def die(natural: Boolean): Unit
}

class DeathBehaviour(entity: Entity, world: World, logger: Logger, animalInfoStateWriter: AnimalInfoStateWriter, entityInfoStateWriter: EntityInfoStateWriter) extends EntityBehaviour with DeathBehaviourInterface {
  override def onReady(): Unit = {
    world.messaging.onReceive {
      case DeathMsg(natural) => {
        die(natural)
      }
    }
  }

  def die(natural: Boolean): Unit = {
    logger.info(s"Entity ${entity.entityId} has died. Natural cause: $natural.")
    if (natural) {
      animalInfoStateWriter.update.isAlive(AnimalVitalityStatus.DeadNatural).finishAndSend()
    } else {
      animalInfoStateWriter.update.isAlive(AnimalVitalityStatus.DeadKilled).finishAndSend()
    }
    /*
    entity.removeBehaviours(Set( // leave resource management to let food reserves slowly deplete
      descriptorOf[SteeringAggregationBehaviour],
      descriptorOf[AnimalLifCycleBehavior],
      descriptorOf[AnimalStateMachineBehaviour],
      descriptorOf[ReproductionBehaviour]
    ))
    */
    world.timing.after(120.seconds) {
      world.messaging.sendToEntity(0, DestroyEntityMsg(entity.entityId, entityInfoStateWriter.entityType)) // todo: hardcoded entityid for global entity, bad!
    }
  }
}
