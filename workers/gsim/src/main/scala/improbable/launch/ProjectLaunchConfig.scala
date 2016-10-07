package improbable.launch

import improbable.corelib.launcher.DefaultConstraintEngineDescriptorResolver
import improbable.dapi.LaunchConfig
import improbable.launch.bridgesettings.ProjectBridgeSettingsResolver
import improbable.util.AppList

class ProjectLaunchConfig(dynamicallySpoolUpEngines: Boolean) extends {} with LaunchConfig(
  AppList.appsToStart,
  dynamicallySpoolUpEngines,
  ProjectBridgeSettingsResolver,
  DefaultConstraintEngineDescriptorResolver
)

object ManualEngineStartupLaunchConfig extends ProjectLaunchConfig(dynamicallySpoolUpEngines = false)

object AutomaticEngineStartupLaunchConfig extends ProjectLaunchConfig(dynamicallySpoolUpEngines = true)
