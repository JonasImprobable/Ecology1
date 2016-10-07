using System.Collections.Generic;
using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Global;
using Assets.Gamelogic.Visualizers.Util;
using Improbable.Animal;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;
using Time = UnityEngine.Time;

namespace Assets.Gamelogic.Visualizers.AnimalFsim
{
    [EngineType(EnginePlatform.FSim)]
    public class AnimalGameInfoManager : MonoBehaviour
    {
        [Require] public AnimalGameInfoStateWriter AnimalGameInfoStateWriter;
        public AnimalGameInfoReader AnimalGameInfoReader;
        public GameInfoReader GameInfoReader;
        public float GameTime;

        void OnEnable()
        {
            AnimalGameInfoReader = GetComponent<AnimalGameInfoReader>();
        }

        void Update()
        {
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
                if (GameInfoReader)
                {
                    GameTime = GameInfoReader.GameTime;
                }
            }
        }

        void FixedUpdate()
        {
            GameTime += Time.deltaTime;

            AnimalGameInfoReader.GameTime = GameTime;
            if (!GameSettings.PerformanceSaverMode)
            {
                AnimalGameInfoStateWriter.Update.GameTime(GameTime).FinishAndSend();
            }
            
            if (AnimalGameInfoNeedsUpdate())
            {
                AnimalGameInfoReader.MapSize = GameInfoReader.MapSize;
                AnimalGameInfoReader.GridSize = GameInfoReader.GridSize;
                AnimalGameInfoReader.SteeringSourceWeights = GameInfoReader.SteeringSourceWeights;

                if (!GameSettings.PerformanceSaverMode)
                {
                    IDictionary<int, float> newSteeringSourceWeights = new Dictionary<int, float>();
                    foreach (var item in GameInfoReader.SteeringSourceWeights)
                    {
                        newSteeringSourceWeights.Add((int)item.Key, item.Value);
                    }
                    AnimalGameInfoStateWriter.Update.MapSize(GameInfoReader.MapSize).GridSize(GameInfoReader.GridSize).SteeringSourceWeights(newSteeringSourceWeights).FinishAndSend();
                }
            }
        }

        bool AnimalGameInfoNeedsUpdate()
        {
            if (!GameInfoReader)
            {
                return false;
            }
            return AnimalGameInfoReader.MapSize != GameInfoReader.MapSize ||
                   Mathf.Abs(AnimalGameInfoReader.GridSize - GameInfoReader.GridSize) > 0.001f ||
                   !CompareSteeringSourceDictionaryEquality(AnimalGameInfoReader.SteeringSourceWeights, GameInfoReader.SteeringSourceWeights);
        }

        bool CompareSteeringSourceDictionaryEquality(IDictionary<SteeringSourceType, float> a, IDictionary<SteeringSourceType, float> b)
        {
            if (a.Count != b.Count)
            {
                return false;
            }
            foreach (var item in a)
            {
                if (!b.ContainsKey(item.Key) || b[item.Key] != item.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
