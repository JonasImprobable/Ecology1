package improbable.natures.player

import improbable.behaviours.player.{PlayerControlsBehaviour, PlayerStateDelegationsBehaviour}
import improbable.corelib.natures.{BaseNature, NatureApplication, NatureDescription}
import improbable.corelib.util.EntityOwner
import improbable.corelibrary.transforms.TransformNature
import improbable.math.Vector3d
import improbable.papi.engine.EngineId
import improbable.papi.entity.behaviour.EntityBehaviourDescriptor
import improbable.player.{LocalPlayerCheckState, PlayerControlsState}
import improbable.util.{AnimalGender, EntityPrefabs, GameSettings}

object PlayerNature extends NatureDescription {
  override def dependencies = Set[NatureDescription](BaseNature, TransformNature)

  override def activeBehaviours: Set[EntityBehaviourDescriptor] = {
    Set(
      descriptorOf[PlayerStateDelegationsBehaviour],
      descriptorOf[PlayerControlsBehaviour]
    )
  }

  def apply(engineId: EngineId): NatureApplication = {
    val tags: List[String] = List(GameSettings.PlayerTag)

    application(
      states = Seq(
        EntityOwner(ownerId = Some(engineId)),
        LocalPlayerCheckState(),
        PlayerControlsState(AnimalGender.Male)
      ),
      natures = Seq(
        BaseNature(EntityPrefabs.Player, tags),
        TransformNature(Vector3d(0, 0.5, 0))
      )
    )
  }
}
