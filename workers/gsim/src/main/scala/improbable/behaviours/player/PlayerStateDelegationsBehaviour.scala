package improbable.behaviours.player

import improbable.corelib.util.EntityOwnerUtils
import improbable.corelibrary.transforms.TransformInterface
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.player.{LocalPlayerCheckState, PlayerControlsState}
import improbable.unity.papi.SpecificEngineConstraint

class PlayerStateDelegationsBehaviour(entity: Entity, transformInterface: TransformInterface) extends EntityBehaviour {

  override def onReady(): Unit = {
    entity.delegateState[LocalPlayerCheckState](SpecificEngineConstraint(EntityOwnerUtils.ownerIdOf(entity)))
    transformInterface.delegatePhysicsToClientOwner()
    entity.delegateState[PlayerControlsState](SpecificEngineConstraint(EntityOwnerUtils.ownerIdOf(entity)))
  }

}
