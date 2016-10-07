using Assets.Gamelogic.Visualizers.AnimalClient;
using Improbable.Unity.Common.Core.Math;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class SteeringSourceIndicatorManager : MonoBehaviour
    {
        public AnimalMovementReader AnimalMovementReader;

        void Update()
        {
            transform.localScale = new Vector3(1f, 1f, AnimalMovementReader.SteeringSourceAggregate.intensity * 2f);
            transform.rotation = Quaternion.LookRotation(AnimalMovementReader.SteeringSourceAggregate.direction.ToUnityVector());
        }
    }
}

