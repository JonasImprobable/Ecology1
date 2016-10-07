package improbable.behaviours.global

import improbable.logging.Logger
import improbable.entity.EntityInfoState
import improbable.global.GameInfoStateWriter
import improbable.papi.EntityId
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour, EntityRecordTemplate}
import improbable.papi.world.World
import improbable.papi.world.entities.EntityFindByTag
import improbable.papi.world.messaging.CustomMsg
import improbable.util.EntityCategory.EntityCategory
import improbable.util._

import scala.concurrent.duration._

case class SpawnEntityMsg(entityRecordTemplate: EntityRecordTemplate, entityCategory: EntityCategory) extends CustomMsg
case class DestroyEntityMsg(entityId: EntityId, prefabName: String) extends CustomMsg

trait EntityManagerBehaviourInterface extends EntityBehaviourInterface {
  def spawnEntity(entityRecordTemplate: EntityRecordTemplate, entityCategory: EntityCategory): Unit
  def destroyEntity(entityId: EntityId, prefabName: String): Unit
  def removeAllEnvironment(): Unit
  def removeAllAnimals(): Unit
}

class EntityManagerBehaviour(entity: Entity, world: World, logger: Logger, gameInfoStateWriter: GameInfoStateWriter) extends EntityBehaviour with EntityManagerBehaviourInterface {
  val LocalEntityRegistry = scala.collection.mutable.HashMap.empty[String, Int]
  var entityRegistryUpToDate = true

  val miniMapScaled = Array.ofDim[Float](GameSettings.MiniMapResolution, GameSettings.MiniMapResolution)
  var miniMapUpToDate = true

  override def onReady(): Unit = {
    world.messaging.onReceive {
      case SpawnEntityMsg(entityRecordTemplate, entityCategory) => spawnEntity(entityRecordTemplate, entityCategory)
      case DestroyEntityMsg(entityId, prefabName) => destroyEntity(entityId, prefabName)
    }
    world.timing.every(5.seconds) {
      if (!entityRegistryUpToDate) {
        synchroniseEntityRegistry()
      }
      if (!miniMapUpToDate) {

      }
    }
  }

  def spawnEntity(entityRecordTemplate: EntityRecordTemplate, entityCategory: EntityCategory): Unit = {
    world.entities.spawnEntity(entityRecordTemplate)
    if (entityCategory == EntityCategory.Animal) {
      if (LocalEntityRegistry.contains(entityRecordTemplate.prefab.name)) {
        LocalEntityRegistry(entityRecordTemplate.prefab.name) += 1
      } else {
        LocalEntityRegistry += (entityRecordTemplate.prefab.name -> 1)
      }
      entityRegistryUpToDate = false
    } else {
      /*
      var position = Vector3d.zero
      entityRecordTemplate.states.foreach(item => if (item.entityPosition.isDefined) {position = item.entityPosition.get.toVector3d})
      if (position == Vector3d.zero) {
        logger.error("Warning, entity spwaned with 0 position")
      }
      val miniMapCoordinate = GameUtils.getMinimapCoordinate(position)
      miniMapScaled(miniMapCoordinate._1)(miniMapCoordinate._2) = MathUtils.clamp(miniMapScaled(miniMapCoordinate._1)(miniMapCoordinate._2) + (1 / GameSettings.MiniMapScaler), 0f, 1f)
      miniMapUpToDate = false
      */
    }
    //GameMetrics.entitiesCreatedByPrefab.labels(entityRecordTemplate.prefab.name).inc() // work is put on hold, metrics are not exposed
  }

  def destroyEntity(entityId: EntityId, prefabName: String): Unit = {
    world.entities.destroyEntity(entityId)
    if (LocalEntityRegistry.contains(prefabName)) {
      LocalEntityRegistry(prefabName) -= 1
    }
    entityRegistryUpToDate = false
    //GameMetrics.entitiesCreatedByPrefab.labels(prefabName).inc() // work is put on hold, metrics are not exposed
  }

  def synchroniseEntityRegistry(): Unit = {
    gameInfoStateWriter.update.entityRegistry(LocalEntityRegistry.toMap).finishAndSend()
    entityRegistryUpToDate = true
  }

  def synchroniseMiniMap(): Unit = {
    //gameInfoStateWriter.update.terrainNoiseMapScaled(GameUtils.arrayToList2d(miniMapScaled)).finishAndSend()
    miniMapUpToDate = true
  }

  def removeAllEnvironment(): Unit = {
    world.entities.find(EntityFindByTag(GameSettings.EnvironmentTag)).foreach(elem => destroyEntity(elem.entityId, elem.get[EntityInfoState].get.entityType))
  }

  def removeAllAnimals(): Unit = {
    world.entities.find(EntityFindByTag(GameSettings.AnimalTag)).foreach(elem => destroyEntity(elem.entityId, elem.get[EntityInfoState].get.entityType))
  }
}
