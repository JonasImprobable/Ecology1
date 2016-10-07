package improbable.util

import improbable.apps._

object AppList {

  val appsToStart = Seq(
    classOf[PlayerLifeCycleManager],
    classOf[EntitiesSpawnerApp]
  )

}
