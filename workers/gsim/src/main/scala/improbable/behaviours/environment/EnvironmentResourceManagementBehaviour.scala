package improbable.behaviours.environment

import improbable.environment_states.EnvironmentInfoStateWriter
import improbable.papi.entity.EntityBehaviour
import improbable.papi.world.World
import improbable.papi.world.messaging.CustomMsg
import improbable.util.{EnvironmentProperties, GameSettings, MathUtils}

case object ConsumeEnvTickMsg extends CustomMsg

class EnvironmentResourceManagementBehaviour(world: World, environmentInfoStateWriter: EnvironmentInfoStateWriter) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate * 24) {
      regenerateResources()
    }
    world.messaging.onReceive {
      case ConsumeEnvTickMsg =>
        consumeResources()
    }
  }

  def regenerateResources(): Unit = {
    val newResources = MathUtils.clamp(environmentInfoStateWriter.resources + GameSettings.EnvResourcesGain, 0f, 100f)
    environmentInfoStateWriter.update
      .resources(newResources)
      .size(EnvironmentProperties.getSizeFromResources(newResources))
      .finishAndSend()
  }

  def consumeResources(): Unit = {
    val newResources = MathUtils.clamp(environmentInfoStateWriter.resources - GameSettings.EnvResourcesLoss, 0f, 100f)
    environmentInfoStateWriter.update
      .resources(newResources)
      .size(EnvironmentProperties.getSizeFromResources(newResources))
      .finishAndSend()
  }
}
