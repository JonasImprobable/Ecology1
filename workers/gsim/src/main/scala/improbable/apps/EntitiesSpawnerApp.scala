package improbable.apps

import improbable.logging.Logger
import improbable.math.Vector3d
import improbable.natures.animal.BaseAnimalNature
import improbable.natures.environment.BaseEnvironmentNature
import improbable.natures.global.GlobalEntityNature
import improbable.papi.world.AppWorld
import improbable.papi.worldapp.WorldApp
import improbable.util.AnimalGender.AnimalGender
import improbable.util.AnimalType.AnimalType
import improbable.util.EnvironmentType.EnvironmentType
import improbable.util._

import scala.util.Random

class EntitiesSpawnerApp(appWorld: AppWorld, logger: Logger) extends WorldApp {
  appWorld.entities.spawnEntity(GlobalEntityNature())
}
