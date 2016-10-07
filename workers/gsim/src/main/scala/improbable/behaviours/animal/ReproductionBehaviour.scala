package improbable.behaviours.animal

import improbable.animal.AnimalInfoStateWriter
import improbable.behaviours.global.SpawnEntityMsg
import improbable.natures.animal.BaseAnimalNature
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.util._

import scala.util.Random

class ReproductionBehaviour(world: World, entity: Entity, animalInfoStateWriter: AnimalInfoStateWriter) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      if (animalInfoStateWriter.isPregnant) {
        val newRemainingPregnancyTime = math.max(animalInfoStateWriter.remainingPregnancyTime - 1f, 0f)
        if (newRemainingPregnancyTime <= 0f) {
          giveBirth()
        } else {
          animalInfoStateWriter.update.remainingPregnancyTime(newRemainingPregnancyTime).finishAndSend()
        }
      }
    }
  }

  def giveBirth(): Unit = {
    val averageLitterSize = AnimalProperties.getLitterSizeFromType(animalInfoStateWriter.species)
    val litterSize = MathUtils.getGaussian(averageLitterSize, averageLitterSize * GameSettings.LitterStandardDeviationScale).round.max(1).toInt
    for (_ <- 0 until litterSize) {
      val childGender = if (Random.nextFloat() + GameSettings.MaleRatio >= 1f) AnimalGender.Male else AnimalGender.Female
      world.messaging.sendToEntity(0, SpawnEntityMsg(BaseAnimalNature(entity.position.toVector3d, animalInfoStateWriter.species, childGender, 0f, entity.entityId), EntityCategory.Animal)) // todo: hardcoded entityid for global entity, bad!
    }
    animalInfoStateWriter.update.isPregnant(false).remainingPregnancyTime(0f).finishAndSend()
  }
}