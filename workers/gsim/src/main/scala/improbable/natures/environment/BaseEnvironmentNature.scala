package improbable.natures.environment

import improbable.behaviours.environment.EnvironmentResourceManagementBehaviour
import improbable.corelib.natures.{BaseNature, NatureApplication, NatureDescription}
import improbable.corelibrary.transforms.TransformNature
import improbable.entity.EntityInfoState
import improbable.environment_states.EnvironmentInfoState
import improbable.math.Vector3d
import improbable.papi.entity.behaviour.EntityBehaviourDescriptor
import improbable.util.EnvironmentType.EnvironmentType
import improbable.util.{EnvironmentProperties, EnvironmentType, GameSettings}

import scala.util.Random

object BaseEnvironmentNature extends NatureDescription {
  override def dependencies = Set[NatureDescription](BaseNature, TransformNature)

  override def activeBehaviours: Set[EntityBehaviourDescriptor] = {
    Set(
      descriptorOf[EnvironmentResourceManagementBehaviour]
    )
  }

  def apply(position: Vector3d = Vector3d.zero, environmentType: EnvironmentType = EnvironmentType.Grass, resources: Float = 100f): NatureApplication = {
    val tags: List[String] = List(GameSettings.EnvironmentTag) // for search as part of a wipe all existing feature
    val traversable = EnvironmentProperties.getTraversableFromType(environmentType)
    val size = EnvironmentProperties.getSizeFromResources(resources)
    val rotation = Random.nextFloat()

    application(
      states = Seq(
        EntityInfoState(EnvironmentProperties.getPrefabFromType(environmentType).name),
        EnvironmentInfoState(environmentType, resources, traversable, size, rotation)
      ),
      natures = Seq(
        BaseNature(EnvironmentProperties.getPrefabFromType(environmentType), tags),
        TransformNature(position)
      )
    )
  }
}
