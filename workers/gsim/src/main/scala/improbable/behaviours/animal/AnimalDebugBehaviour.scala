package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.GameSettings

class AnimalDebugBehaviour(entity: Entity, world: World, logger: Logger) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      if (entity.position.y < -1) {
        logger.error(s"Entity ${entity.entityId} might be off the ground, pos: ${entity.position}.")
      }
    }
  }
}
