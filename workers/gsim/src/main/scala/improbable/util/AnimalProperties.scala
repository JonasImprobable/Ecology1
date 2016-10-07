package improbable.util

import improbable.animal.{AnimalInfoState, AnimalStateMachineState, PerceivedAnimalInfo, PerceivedEnvironmentInfo}
import improbable.papi.entity.{Entity, EntityPrefab}
import improbable.util.AnimalCatergory.AnimalCatergory
import improbable.util.AnimalGender.AnimalGender
import improbable.util.AnimalResource.AnimalResource
import improbable.util.AnimalType.AnimalType
import improbable.util.SteeringSourceType.SteeringSourceType

object AnimalProperties {
  // used in pick need and state machine(determine if need satisfied)
  def getAnimalResourceStatus(entity: Entity): List[(AnimalResource, Float)] = {
    val animalInfoStateWatcher = entity.watch[AnimalInfoState]
    List[(AnimalResource, Float)](
      (AnimalResource.Water, animalInfoStateWatcher.water.get),
      (AnimalResource.Food, animalInfoStateWatcher.food.get),
      (AnimalResource.Mating, animalInfoStateWatcher.mating.get)
    )
  }

  def isNeedTargetEnvironment(entity: Entity, need: AnimalResource, otherEntity: PerceivedEnvironmentInfo): Boolean = {
    need match {
      case AnimalResource.Water => {
        (EnvironmentProperties.getCategoryFromType(otherEntity.environmentType) == EnvironmentCategory.WaterSource || EnvironmentProperties.getCategoryFromType(otherEntity.environmentType) == EnvironmentCategory.Plant) &&
          otherEntity.resources > 0f
      }
      case AnimalResource.Food => {
        getCategoryFromType(entity.watch[AnimalInfoState].species.get) match {
          case AnimalCatergory.Herbivore => {
            EnvironmentProperties.getCategoryFromType(otherEntity.environmentType) == EnvironmentCategory.Plant &&
              otherEntity.resources > 0f
          }
          case AnimalCatergory.Carnivore => false
        }
      }
      case _ => false
    }
  }

  def isNeedTargetAnimal(entity: Entity, need: AnimalResource, otherEntity: PerceivedAnimalInfo): Boolean = {
    need match {
      case x if x == AnimalResource.Water || x == AnimalResource.Water => { //todo: wtf??
        getCategoryFromType(entity.watch[AnimalInfoState].species.get) match {
          case AnimalCatergory.Herbivore => false
          case AnimalCatergory.Carnivore => {
            getCategoryFromType(otherEntity.species) == AnimalCatergory.Herbivore &&
              otherEntity.isAlive != AnimalVitalityStatus.DeadNatural
            // remaining resources should be > 0
          }
        }
      }
      case AnimalResource.Mating => {
        val animalInfoStateWatcher = entity.watch[AnimalInfoState]
        animalInfoStateWatcher.isAdult.get &&
          otherEntity.species == animalInfoStateWatcher.species.get &&
          otherEntity.gender != animalInfoStateWatcher.gender.get &&
          otherEntity.isAlive == AnimalVitalityStatus.Alive &&
          otherEntity.isAdult &&
          !otherEntity.isPregnant &&
          otherEntity.entityId != animalInfoStateWatcher.mother.get
      }
      case _ => false
    }
  }

  // used in pick need and avoid steering
  def isObstacleEnvironment(entity: Entity, otherEntity: PerceivedEnvironmentInfo): Boolean = {
    !otherEntity.traversable && //Don't avoid traversable terrain
      !isNeedTargetEnvironment(entity, entity.watch[AnimalStateMachineState].targetNeed.get, otherEntity) //Don't avoid potential alternative targets
  }

  def isObstacleAnimal(entity: Entity, otherEntity: PerceivedAnimalInfo): Boolean = {
    val animalInfoStateWatcher = entity.watch[AnimalInfoState]
    otherEntity.species != animalInfoStateWatcher.species.get  && //Don't avoid animals from the same species, to be dealt with separately in HerdBehaviour
      AnimalProperties.getCategoryFromType(animalInfoStateWatcher.species.get) == AnimalCatergory.Herbivore && //avoiding other species is feature creeped into herdsteering, this here is only for occlusion detection and drastic avoidance
      AnimalProperties.getCategoryFromType(otherEntity.species) == AnimalCatergory.Carnivore
  }

  def getSizeFromAge(animalType: AnimalType, age: Float, maxSize: Float): Float = {
    MathUtils.clamp(age / getAgeOfMajorityFromType(animalType), GameSettings.EntityMinSize, maxSize)
  }

  def getCategoryFromType(animalType: AnimalType): AnimalCatergory = {
    animalType match {
      case AnimalType.Elephant => AnimalCatergory.Herbivore
      case AnimalType.Giraffe => AnimalCatergory.Herbivore
      case AnimalType.Wildebeest => AnimalCatergory.Herbivore

      case AnimalType.Cheetah => AnimalCatergory.Carnivore
      case AnimalType.Hyena => AnimalCatergory.Carnivore
      case AnimalType.Lion => AnimalCatergory.Carnivore
    }
  }

  def getPrefabFromType(animalType: AnimalType, gender: AnimalGender): EntityPrefab = {
    animalType match {
      case AnimalType.Elephant => EntityPrefabs.Elephant
      case AnimalType.Giraffe => EntityPrefabs.Giraffe
      case AnimalType.Wildebeest => EntityPrefabs.Wildebeest

      case AnimalType.Cheetah => EntityPrefabs.Cheetah
      case AnimalType.Hyena => EntityPrefabs.Hyena
      case AnimalType.Lion => if (gender == AnimalGender.Male) EntityPrefabs.LionMale else EntityPrefabs.LionFemale
    }
  }

  def getLifeExpectancyFromType(animalType: AnimalType): Float = {
    animalType match {
      case AnimalType.Elephant => 1000f // 63 years
      case AnimalType.Giraffe => 1000f// 23 years
      case AnimalType.Wildebeest => 1000f// 18 years

      case AnimalType.Cheetah => 1000f// 11 years
      case AnimalType.Hyena => 1000f// 23 years
      case AnimalType.Lion => 1000f// 12 years
    }
  }

  def getAgeOfMajorityFromType(animalType: AnimalType): Float = {
    animalType match {
      case AnimalType.Elephant => 40f // 12 years
      case AnimalType.Giraffe => 40f// 4 years
      case AnimalType.Wildebeest => 40f// 2 years

      case AnimalType.Cheetah => 40f// 2 years
      case AnimalType.Hyena => 40f// 2 years
      case AnimalType.Lion => 40f// 3 years
    }
  }

  def getGestationPeriodFromType(animalType: AnimalType): Float = {
    animalType match {
      case AnimalType.Elephant => 20f // 2 years
      case AnimalType.Giraffe => 20f// 460 days
      case AnimalType.Wildebeest => 20f// 255 days

      case AnimalType.Cheetah => 20f// 90 days
      case AnimalType.Hyena => 20f// 110 days
      case AnimalType.Lion => 20f// 110 days
    }
  }

  // represents average litter size
  def getLitterSizeFromType(animalType: AnimalType): Float = {
    animalType match {
      case AnimalType.Elephant => 1f
      case AnimalType.Giraffe => 1f
      case AnimalType.Wildebeest => 1f

      case AnimalType.Cheetah => 3f
      case AnimalType.Hyena => 3f
      case AnimalType.Lion => 3f
    }
  }

  def defaultSteeringWeightsMap(): Map[SteeringSourceType, Float] = {
    Map[SteeringSourceType, Float](
      SteeringSourceType.Wander -> 1f,
      SteeringSourceType.Herd -> 2f,
      SteeringSourceType.Follow -> 4f,
      SteeringSourceType.Seek -> 16f,
      SteeringSourceType.Avoid -> 8f
    )
  }

  def getSteeringWeightsMap(w: Float, h: Float, f: Float, s: Float, a: Float): Map[SteeringSourceType, Float] = {
    Map[SteeringSourceType, Float](
      SteeringSourceType.Wander -> w,
      SteeringSourceType.Herd -> h,
      SteeringSourceType.Follow -> f,
      SteeringSourceType.Seek -> s,
      SteeringSourceType.Avoid -> a
    )
  }
}
