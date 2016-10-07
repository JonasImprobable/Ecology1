package improbable.behaviours.animal

import improbable.logging.Logger
import improbable.animal.{AnimalGameInfoState, AnimalMovementState, AnimalPerceptionState, AnimalStateMachineState}
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.unity.fabric.PhysicsEngineConstraint

class AnimalStateDelegationBehaviour(entity: Entity, logger: Logger) extends EntityBehaviour {
  override def onReady(): Unit = {
    entity.delegateState[AnimalGameInfoState](PhysicsEngineConstraint)
    entity.delegateState[AnimalPerceptionState](PhysicsEngineConstraint)
    entity.delegateState[AnimalStateMachineState](PhysicsEngineConstraint)
    entity.delegateState[AnimalMovementState](PhysicsEngineConstraint)
  }
}
