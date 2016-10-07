package improbable.launch.bridgesettings

import improbable.fapi.bridge.CompositeBridgeSettingsResolver
import improbable.unity.fabric.bridge.UnityFSimBridgeSettings

object ProjectBridgeSettingsResolver extends CompositeBridgeSettingsResolver(
  ProjectUnityClientBridgeSettings,
  ProjectUnityFSimBridgeSettings
)
