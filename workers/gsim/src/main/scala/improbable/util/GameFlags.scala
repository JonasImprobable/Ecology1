package improbable.util

import improbable.flagz.{FlagContainer, FlagInfo, ScalaFlagz}


object GameFlags extends FlagContainer {
  @FlagInfo(name = "is_local", help = "Simulation is this running locally")
  val isLocal = ScalaFlagz.valueOf(false)
}
