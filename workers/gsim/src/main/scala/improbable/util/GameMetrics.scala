package improbable.util

import improbable.metrics.UserMetricsReporter
import improbable.metrics.Metrics.{Counter, Gauge, Histogram}

object GameMetrics extends UserMetricsReporter("demo") {
  override val subsystem: String = "ecology"

  final val entitiesCreatedByPrefab: Counter = counterBuilder("entities_created_by_prefab")
    .labelNames("prefab")
    .help("entities_created_by_prefab")
    .register()

  final val entitiesDestroyedByPrefab: Counter = counterBuilder("entities_destroyed_by_prefab")
    .labelNames("prefab")
    .help("entities_destroyed_by_prefab")
    .register()
}