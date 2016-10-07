package improbable.behaviours.animal

import improbable.animal.AnimalInfoStateWriter
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util.{AnimalProperties, AnimalVitalityStatus, GameSettings}

class AnimalLifCycleBehavior(world: World, entity: Entity, animalInfoStateWriter: AnimalInfoStateWriter, deathBehaviourInterface: DeathBehaviourInterface) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      if (animalInfoStateWriter.isAlive == AnimalVitalityStatus.Alive) {
        val newAge = animalInfoStateWriter.age + 1f
        animalInfoStateWriter.update
          .age(newAge)
          .remainingLifeExpectancy(math.max(animalInfoStateWriter.remainingLifeExpectancy - 1f, 0f))
          .isAdult(newAge >= AnimalProperties.getAgeOfMajorityFromType(animalInfoStateWriter.species))
          .size(AnimalProperties.getSizeFromAge(animalInfoStateWriter.species, newAge, animalInfoStateWriter.priority))
          .finishAndSend()

        if (animalInfoStateWriter.remainingLifeExpectancy <= 0f && !GameSettings.ImmortalAnimals) {
          deathBehaviourInterface.die(true)
        }
      }
    }
  }
}