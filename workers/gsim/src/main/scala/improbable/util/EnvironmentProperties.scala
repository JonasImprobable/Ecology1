package improbable.util

import improbable.papi.entity.EntityPrefab
import improbable.util.EnvironmentCategory.EnvironmentCategory
import improbable.util.EnvironmentType.EnvironmentType

object EnvironmentProperties {
  def getSizeFromResources(resources: Float): Float = {
    MathUtils.clamp(resources / 100f * 2f, GameSettings.EntityMinSize, GameSettings.EntityMaxSize)
  }

  def getCategoryFromType(environmentType: EnvironmentType): EnvironmentCategory = {
    environmentType match {
      case EnvironmentType.Lake => EnvironmentCategory.WaterSource
      case EnvironmentType.Steak => EnvironmentCategory.Meat
      case EnvironmentType.Rock => EnvironmentCategory.Obstacle

      case EnvironmentType.Grass => EnvironmentCategory.Plant
      case EnvironmentType.Bush => EnvironmentCategory.Plant
      case EnvironmentType.Tree => EnvironmentCategory.Plant
    }
  }

  def getPrefabFromType(environmentType: EnvironmentType): EntityPrefab = {
    environmentType match {
      case EnvironmentType.Lake => EntityPrefabs.Lake
      case EnvironmentType.Steak => EntityPrefabs.Steak
      case EnvironmentType.Rock => EntityPrefabs.Rock

      case EnvironmentType.Grass => EntityPrefabs.Grass
      case EnvironmentType.Bush => EntityPrefabs.Bush
      case EnvironmentType.Tree => EntityPrefabs.Tree
    }
  }

  def getTraversableFromType(environmentType: EnvironmentType): Boolean = {
    environmentType match {
      case EnvironmentType.Lake => false
      case EnvironmentType.Steak => false
      case EnvironmentType.Rock => false

      case EnvironmentType.Grass => true
      case EnvironmentType.Bush => true
      case EnvironmentType.Tree => false
    }
  }
}
