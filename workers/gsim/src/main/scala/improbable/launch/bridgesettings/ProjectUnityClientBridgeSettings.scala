package improbable.launch.bridgesettings

import improbable.fapi.bridge._
import improbable.fapi.network.RakNetLinkSettings
import improbable.unity.fabric.AuthoritativeEntityOnly
import improbable.unity.fabric.engine.EnginePlatform._
import improbable.unity.fabric.satisfiers.SatisfyVisual
import improbable.util.GameSettings

object ProjectUnityClientBridgeSettings extends BridgeSettingsResolver {

  private def clientEngineBridgeSettings = BridgeSettings(
    ProjectClientAssetContextDiscriminator(),
    RakNetLinkSettings(),
    UNITY_CLIENT_ENGINE,
    SatisfyVisual,
    AuthoritativeEntityOnly(GameSettings.ClientCheckoutRadius),
    ConstantEngineLoadPolicy(0.5),
    PerEntityOrderedStateUpdateQos,
    TagStreamingQueryPolicy(GameSettings.GlobalEntityTag)
  )

  override def engineTypeToBridgeSettings(engineType: String, metadata: String): Option[BridgeSettings] = {
    engineType match {
      case UNITY_CLIENT_ENGINE =>
        Some(clientEngineBridgeSettings)
      case _ =>
        None
    }
  }

}
