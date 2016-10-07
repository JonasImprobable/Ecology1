package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.animal._
import improbable.behaviours.environment.ConsumeEnvTickMsg
import improbable.papi.entity.behaviour.EntityBehaviourInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.AnimalResource.AnimalResource
import improbable.util.{AnimalResource, AnimalState, GameSettings, MathUtils}

import scala.concurrent.duration._

trait AnimalResourceManagementBehaviourInterface extends EntityBehaviourInterface {
  def setResource(animalResource: AnimalResource, value: Float): Unit
}

class AnimalResourceManagementBehaviour(entity: Entity, world: World, logger: Logger, animalInfoStateWriter: AnimalInfoStateWriter, animalMovementStateWriter: AnimalMovementStateWriter, deathBehaviourInterface: DeathBehaviourInterface) extends EntityBehaviour with AnimalResourceManagementBehaviourInterface {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      decrementCooldowns()
      regenerateResources()
      consumeResources()
    }
  }

  def setResource(resource: AnimalResource, value: Float) = {
    resource match {
      case AnimalResource.Stamina => {
        animalInfoStateWriter.update.stamina(MathUtils.clamp(value, 0f, 100f)).finishAndSend()
      }
      case AnimalResource.Water => {
        animalInfoStateWriter.update.water(MathUtils.clamp(value, 0f, 100f)).finishAndSend()
      }
      case AnimalResource.Food => {
        animalInfoStateWriter.update.food(MathUtils.clamp(value, 0f, 100f)).finishAndSend()
      }
      case AnimalResource.Mating => {
        animalInfoStateWriter.update.mating(MathUtils.clamp(value, 0f, 100f)).finishAndSend()
      }
    }
  }

  def regenerateResources(): Unit = {
    val currentState = entity.watch[AnimalStateMachineState].currentState.get
    val targetEntityId = entity.watch[AnimalStateMachineState].targetEntity.get
    val targetNeed = entity.watch[AnimalStateMachineState].targetNeed.get
    if (currentState == AnimalState.Interacting && targetEntityId != -1 && targetNeed != AnimalResource.Invalid) {
      if (targetNeed == AnimalResource.Water || targetNeed == AnimalResource.Food) {
        animalInfoStateWriter.update
          .water(MathUtils.clamp(animalInfoStateWriter.water + GameSettings.WaterGain, 0f, 100f))
          .food(MathUtils.clamp(animalInfoStateWriter.food + GameSettings.FoodGain, 0f, 100f))
          .resourceLossCooldowns(animalInfoStateWriter.resourceLossCooldowns + (AnimalResource.Water.id -> GameSettings.ResourceLossCooldown) + (AnimalResource.Food.id -> GameSettings.ResourceLossCooldown))
          .finishAndSend()
        world.messaging.sendToEntity(targetEntityId, ConsumeEnvTickMsg)
      }
    }
    // Stamina
    val targetSpeed = animalMovementStateWriter.steeringSourceAggregate.intensity
    if (targetSpeed <= 0.5f) {
      val newStamina = if (animalInfoStateWriter.resourceLossCooldowns.contains(AnimalResource.Stamina.id)) animalInfoStateWriter.stamina else MathUtils.clamp(animalInfoStateWriter.stamina + GameSettings.StaminaGain, 0f, 100f)
      animalInfoStateWriter.update.stamina(newStamina).finishAndSend()
    }
  }

  def consumeResources(): Unit = {
    val newWater = if (animalInfoStateWriter.resourceLossCooldowns.contains(AnimalResource.Water.id)) animalInfoStateWriter.water else MathUtils.clamp(animalInfoStateWriter.water - GameSettings.WaterLoss, 0f, 100f)
    val newFood = if (animalInfoStateWriter.resourceLossCooldowns.contains(AnimalResource.Food.id)) animalInfoStateWriter.food else MathUtils.clamp(animalInfoStateWriter.food - GameSettings.FoodLoss, 0f, 100f)
    val newMating = if (animalInfoStateWriter.resourceLossCooldowns.contains(AnimalResource.Mating.id) || !animalInfoStateWriter.isAdult || animalInfoStateWriter.isPregnant) animalInfoStateWriter.mating else MathUtils.clamp(animalInfoStateWriter.mating - GameSettings.MatingLoss, 0f, 100f)
    animalInfoStateWriter.update
      .water(newWater)
      .food(newFood)
      .mating(newMating)
      .finishAndSend()
    if (!GameSettings.ImmortalAnimals && (newWater == 0.0f || newFood == 0.0f)) {
      //deathBehaviourInterface.die(true)
    }
    // Stamina
    val targetSpeed = animalMovementStateWriter.steeringSourceAggregate.intensity
    if (targetSpeed > 0.5f) {
      val newStamina = MathUtils.clamp(animalInfoStateWriter.stamina - GameSettings.StaminaLoss, 0f, 100f)
      animalInfoStateWriter.update
        .stamina(newStamina)
        .resourceLossCooldowns(animalInfoStateWriter.resourceLossCooldowns + (AnimalResource.Stamina.id -> GameSettings.ResourceLossCooldown))
        .finishAndSend()
    }
  }

  def decrementCooldowns(): Unit = {
    animalInfoStateWriter.update.resourceLossCooldowns(
      animalInfoStateWriter.resourceLossCooldowns.map(elem => (elem._1, elem._2 - 1.0f)).filter(elem => elem._2 > 0.0f)
    ).finishAndSend()
  }
}
