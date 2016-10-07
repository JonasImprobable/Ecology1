package improbable.launch.bridgesettings

import improbable.fapi.bridge.{AssetContextDiscriminator, EngineEntity}
import improbable.unity.asset.PrefabContext

case class ProjectClientAssetContextDiscriminator() extends AssetContextDiscriminator {

  override def assetContextForEntity(entity: EngineEntity): String = {
    PrefabContext.DEFAULT
  }

}
