package improbable.launch

import improbable.dapi.{LaunchConfig, Launcher}
import scala.sys.process._

class ProjectLauncher(launchConfig: LaunchConfig) {
  val options = Seq(
    "--engine_startup_retries=3",
    "--game_chunk_size=100",
    "--game_world_edge_length=8000",
    "--entity_activator=improbable.corelib.entity.CoreLibraryEntityActivator",
    "--use_spatial_build_workflow=true",
    "--resource_based_config_name=one-gsim-one-jvm",
    "--is_local=true"
  )
  Launcher.startGame(launchConfig, options: _*)
}

object ManualEngineSpoolUpDemonstrationLauncher extends ProjectLauncher(ManualEngineStartupLaunchConfig) with App
object AutoEngineSpoolUpDemonstrationLauncher extends ProjectLauncher(AutomaticEngineStartupLaunchConfig) with App

object SpatialOSLauncher extends App {
  "spatial build gsim".!
  "spatial local start default_launch.pb.json".!
}