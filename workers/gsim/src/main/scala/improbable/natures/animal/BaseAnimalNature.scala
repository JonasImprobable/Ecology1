package improbable.natures.animal

import improbable.animal._
import improbable.behaviours.animal._
import improbable.behaviours.animal.statemachine.AnimalStateMachineBehaviour
import improbable.corelib.natures.{BaseNature, NatureApplication, NatureDescription}
import improbable.corelibrary.transforms.TransformNature
import improbable.entity.EntityInfoState
import improbable.math.Vector3d
import improbable.papi.EntityId
import improbable.papi.entity.behaviour.EntityBehaviourDescriptor
import improbable.util.AnimalGender.AnimalGender
import improbable.util.AnimalType.AnimalType
import improbable.util._

import scala.util.Random

object BaseAnimalNature extends NatureDescription {
  override def dependencies = Set[NatureDescription](BaseNature, TransformNature)

  override def activeBehaviours: Set[EntityBehaviourDescriptor] = {
    Set(
      descriptorOf[AnimalStateDelegationBehaviour],
      descriptorOf[AnimalDebugBehaviour],
      descriptorOf[AnimalLifCycleBehavior], // controls age, isAdult, size
      descriptorOf[AnimalResourceManagementBehaviour],
      descriptorOf[DeathBehaviour],
      descriptorOf[AnimalStateMachineBehaviour],
      descriptorOf[MatingBehaviour],
      descriptorOf[ReproductionBehaviour]
      //descriptorOf[AnimalMemoryBehaviour]
    )
  }

  def apply(position: Vector3d, species: AnimalType, gender: AnimalGender, age: Float = -1f, mother: EntityId = -1): NatureApplication = {
    val tags: List[String] = List(GameSettings.AnimalTag) // for search as part of a wipe all existing feature
    val gameTime = 0f
    val isAlive = AnimalVitalityStatus.Alive
    val startAge = if (age >= 0f) age else (Random.nextFloat() * AnimalProperties.getLifeExpectancyFromType(species)).round
    val remainingLifeExpectancy = AnimalProperties.getLifeExpectancyFromType(species) - age
    val isAdult = age >= AnimalProperties.getAgeOfMajorityFromType(species)
    val priority = 0.5f + Random.nextFloat()
    val size = AnimalProperties.getSizeFromAge(species, startAge, priority)
    val speed = 1f
    val stamina = 100f
    val water = 100f
    val food = 100f
    val mating = 100f
    val isPregnant = false
    val remainingPregnancyTime = 0f
    val meatAsFoodResource = size

    application(
      states = Seq(
        EntityInfoState(AnimalProperties.getPrefabFromType(species, gender).name),
        AnimalGameInfoState(gameTime, GameSettings.MapSize, GameSettings.GridSize, AnimalProperties.defaultSteeringWeightsMap().map(elem => (elem._1.id, elem._2))),
        AnimalInfoState(species, gender, isAlive, startAge, remainingLifeExpectancy, isAdult, size, priority, speed, stamina, water, food, mating, Map.empty, isPregnant, remainingPregnancyTime, mother, meatAsFoodResource),
        AnimalStateMachineState(AnimalState.Neutral, 0f, AnimalResource.Invalid, -1, Vector3d.zero, Map.empty, Map.empty),
        AnimalPerceptionState(Map.empty, Map.empty),
        AnimalMovementState(SteeringSource(Vector3d.zero, 0f), Map.empty),
        AnimalGsimState(),
        AnimalMemoryState(List.empty)
      ),
      natures = Seq(
        BaseNature(AnimalProperties.getPrefabFromType(species, gender), tags),
        TransformNature(position)
      )
    )
  }
}
