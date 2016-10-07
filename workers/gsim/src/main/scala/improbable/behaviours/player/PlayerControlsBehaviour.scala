package improbable.behaviours.player

import improbable.logging.Logger
import improbable.behaviours.animal.DeathMsg
import improbable.behaviours.global._
import improbable.entity.EntityInfoState
import improbable.math.Vector3d
import improbable.natures.animal.BaseAnimalNature
import improbable.natures.environment.BaseEnvironmentNature
import improbable.papi.EntityId
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.papi.world.entities.EntityFindByTag
import improbable.papi.world.messaging.CustomMsg
import improbable.player.PlayerControlsState
import improbable.util.{AnimalProperties, EntityCategory, GameSettings}

case class MoveEntityMsg(position: Vector3d) extends CustomMsg

class PlayerControlsBehaviour(entity: Entity, world: World, logger: Logger) extends EntityBehaviour {
  override def onReady(): Unit = {
    var globalEntityId: EntityId = -1
    world.entities.find(EntityFindByTag(GameSettings.GlobalEntityTag)).foreach(elem => globalEntityId = elem.entityId)

    // Editor Events
    entity.watch[PlayerControlsState].onSetMapParametersEvent {
      event => world.messaging.sendToEntity(globalEntityId, SetTerrainNoiseMapParametersMsg(event.mapSize, event.gridSize, event.octaveCount, event.persistence, event.vegetationDensity))
    }
    entity.watch[PlayerControlsState].onRefreshTerrainEvent {
      event => world.messaging.sendToEntity(globalEntityId, RefreshTerrainMsg(event.mapSize, event.gridSize, event.vegetationDensity))
    }
    entity.watch[PlayerControlsState].onSpawnAnimalsEvent {
      event => world.messaging.sendToEntity(globalEntityId, SpawnAnimalsMsg(event.mapSize, event.gridSize))
    }
    entity.watch[PlayerControlsState].onSetAnimalSteeringParametersEvent {
      event => world.messaging.sendToEntity(globalEntityId, SetAnimalSteeringParametersMsg(event.wander, event.herd, event.follow, event.seek, event.avoid))
    }

    // Ingame Events
    entity.watch[PlayerControlsState].onSpawnEnvironmentEvent {
      spawnEnvironmentEvent => {
        world.messaging.sendToEntity(0, SpawnEntityMsg(BaseEnvironmentNature(spawnEnvironmentEvent.position, spawnEnvironmentEvent.environmentType), EntityCategory.Environment)) // todo: hardcoded entityid for global entity, bad!
      }
    }
    entity.watch[PlayerControlsState].onSpawnAnimalEvent {
      spawnAnimalEvent => {
        val activeGender = entity.watch[PlayerControlsState].activeAnimalGender.get
        world.messaging.sendToEntity(0, SpawnEntityMsg(BaseAnimalNature(spawnAnimalEvent.position, spawnAnimalEvent.species, activeGender, AnimalProperties.getAgeOfMajorityFromType(spawnAnimalEvent.species)), EntityCategory.Animal)) // todo: hardcoded entityid for global entity, bad!
      }
    }
    entity.watch[PlayerControlsState].onKillEntityEvent {
      killEntityEvent => world.messaging.sendToEntity(killEntityEvent.entity, DeathMsg(false))
    }
    entity.watch[PlayerControlsState].onDestroyEntityEvent {
      destroyEntityEvent => {
        val entitySnapshot = world.entities.find(destroyEntityEvent.entity).get
        world.messaging.sendToEntity(0, DestroyEntityMsg(destroyEntityEvent.entity, entitySnapshot.get[EntityInfoState].get.entityType)) // todo: hardcoded entityid for global entity, bad!
      }
    }
    entity.watch[PlayerControlsState].onMoveEntityEvent {
      moveEntityEvent => world.messaging.sendToEntity(moveEntityEvent.entity, MoveEntityMsg(moveEntityEvent.position))
    }
  }
}
