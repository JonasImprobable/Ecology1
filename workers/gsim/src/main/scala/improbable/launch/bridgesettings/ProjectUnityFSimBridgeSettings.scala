package improbable.launch.bridgesettings

import improbable.fapi.bridge._
import improbable.fapi.network.RakNetLinkSettings
import improbable.unity.fabric.AuthoritativeEntityOnly
import improbable.unity.fabric.bridge.FSimAssetContextDiscriminator
import improbable.unity.fabric.engine.EnginePlatform._
import improbable.unity.fabric.satisfiers.SatisfyPhysics
import improbable.util.GameSettings

object ProjectUnityFSimBridgeSettings extends BridgeSettingsResolver {

  private def fsimEngineBridgeSettings = BridgeSettings(
    FSimAssetContextDiscriminator(),
    RakNetLinkSettings(),
    UNITY_FSIM_ENGINE,
    SatisfyPhysics,
    AuthoritativeEntityOnly(GameSettings.FsimCheckoutRadius),
    MetricsEngineLoadPolicy,
    PerEntityOrderedStateUpdateQos,
    TagStreamingQueryPolicy(GameSettings.GlobalEntityTag)
  )

  override def engineTypeToBridgeSettings(engineType: String, metadata: String): Option[BridgeSettings] = {
    engineType match {
      case UNITY_FSIM_ENGINE =>
        Some(fsimEngineBridgeSettings)
      case _ =>
        None
    }
  }

}
