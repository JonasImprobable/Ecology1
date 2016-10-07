package improbable.behaviours.global

import improbable.logging.Logger
import improbable.global.GameInfoStateWriter
import improbable.papi.entity.{Entity, EntityBehaviour}
import improbable.papi.world.World
import improbable.papi.world.messaging.CustomMsg
import improbable.util.{AnimalProperties, GameSettings, MathUtils}

case class SetTerrainNoiseMapParametersMsg(mapSize: Int, gridSize: Float, octaveCount: Int, persistence: Float, vegetationDensity: Float) extends CustomMsg

case class RefreshTerrainMsg(mapSize: Int, gridSize: Float, vegetationDensity: Float) extends CustomMsg

case class SpawnAnimalsMsg(mapSize: Int, gridSize: Float) extends CustomMsg

case class SetAnimalSteeringParametersMsg(wander: Float, herd: Float, follow: Float, seek: Float, avoid: Float) extends CustomMsg

class GameInfoBehaviour(entity: Entity, world: World, logger: Logger, gameInfoStateWriter: GameInfoStateWriter, terrainManagerBehaviourInterface: TerrainManagerBehaviourInterface) extends EntityBehaviour {
  override def onReady(): Unit = {
    world.timing.every(GameSettings.GameHourTickRate) {
      gameInfoStateWriter.update.gameTime(gameInfoStateWriter.gameTime + 1f).finishAndSend()
    }

    world.messaging.onReceive {
      case SetTerrainNoiseMapParametersMsg(m, g, o, p, v) => {
        gameInfoStateWriter.update.mapSize(m).gridSize(g).octaveCount(o).persistence(MathUtils.clamp(p, 0.0f, 1.0f)).vegetationDensity(MathUtils.clamp(v, 0.0f, 1.0f)).finishAndSend()
        terrainManagerBehaviourInterface.generateTerrainNoiseMap(m, o, p)
      }
      case RefreshTerrainMsg(m, g, v) => {
        gameInfoStateWriter.update.mapSize(m).gridSize(g).vegetationDensity(MathUtils.clamp(v, 0.0f, 1.0f)).finishAndSend()
        terrainManagerBehaviourInterface.spawnEnvironment(m, g, v)
      }
      case SpawnAnimalsMsg(m, g) => {
        gameInfoStateWriter.update.mapSize(m).gridSize(g).finishAndSend()
        terrainManagerBehaviourInterface.spawnAnimals(m, g)
      }
      case SetAnimalSteeringParametersMsg(w, h, f, s, a) => gameInfoStateWriter.update.steeringSourceWeights(AnimalProperties.getSteeringWeightsMap(w, h, f, s, a).map(elem => (elem._1.id, elem._2))).finishAndSend()
    }
  }
}
