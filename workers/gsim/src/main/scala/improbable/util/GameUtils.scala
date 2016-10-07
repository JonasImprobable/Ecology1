package improbable.util

import improbable.math.Vector3d

object GameUtils {
  //potentially not used
  def getMiniMapCoordinate(position: Vector3d, mapSize: Int = GameSettings.MapSize, gridSize: Float = GameSettings.GridSize, miniMapResolution: Int = GameSettings.MiniMapResolution): (Int, Int) = {
    val coordX = Math.round(position.x / gridSize + (mapSize / 2) / mapSize * miniMapResolution).toInt
    val coordY = Math.round(position.z / gridSize + (mapSize / 2) / mapSize * miniMapResolution).toInt
    (coordX, coordY)
  }

  // true denotes day, false denotes night
  def isDayTime(gameTime: Float): Boolean = {
    val dayHour = gameTime % 24
    dayHour >= 6f && dayHour <= 18f
  }

  def arrayToList2d(array2d: Array[Array[Float]]): List[ProtoFloatArray] = {
    val out = scala.collection.mutable.ListBuffer.empty[ProtoFloatArray]
    array2d.foreach(elem => out += ProtoFloatArray(elem.toList))
    out.toList
  }

  def listToArray2d(list2d: List[ProtoFloatArray]): Array[Array[Float]] = {
    list2d.map(elem => elem.array.toArray).toArray
  }
}
