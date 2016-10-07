package improbable.natures.global

import improbable.behaviours.global.{EntityManagerBehaviour, GameInfoBehaviour, TerrainManagerBehaviour}
import improbable.corelib.natures.{BaseNature, NatureApplication, NatureDescription}
import improbable.corelibrary.transforms.TransformNature
import improbable.math.Vector3d
import improbable.papi.entity.behaviour.EntityBehaviourDescriptor
import improbable.global.GameInfoState
import improbable.util.{AnimalProperties, EntityPrefabs, GameFlags, GameSettings}

object GlobalEntityNature extends NatureDescription {
  override def dependencies = Set[NatureDescription](BaseNature, TransformNature)

  override def activeBehaviours: Set[EntityBehaviourDescriptor] = {
    Set(
      descriptorOf[GameInfoBehaviour],
      descriptorOf[TerrainManagerBehaviour],
      descriptorOf[EntityManagerBehaviour]
    )
  }

  def apply(): NatureApplication = {
    val tags: List[String] = List(GameSettings.GlobalEntityTag) // used for world.entities.find
    val mapSize = if (GameFlags.isLocal.get) GameSettings.LocalMapSize else GameSettings.MapSize

    application(
      states = Seq(
        GameInfoState(0f, mapSize, GameSettings.GridSize, GameSettings.OctaveCount, GameSettings.Persistence, GameSettings.VegetationDensity, List.empty, AnimalProperties.defaultSteeringWeightsMap().map(elem => (elem._1.id, elem._2)), Map.empty)
      ),
      natures = Seq(
        BaseNature(EntityPrefabs.Global, tags),
        TransformNature(Vector3d.unitY * -1) // hide the model below the ground
      )
    )
  }
}
