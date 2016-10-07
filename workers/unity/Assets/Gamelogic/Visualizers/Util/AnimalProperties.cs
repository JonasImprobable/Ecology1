using System.Collections.Generic;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class AnimalProperties : MonoBehaviour
    {
        public static AnimalCatergory GetCategoryFromType(AnimalType animalType)
        {
            AnimalCatergory result = AnimalCatergory.Herbivore;
            switch (animalType)
            {
                case AnimalType.Elephant: result = AnimalCatergory.Herbivore; break;
                case AnimalType.Giraffe: result = AnimalCatergory.Herbivore; break;
                case AnimalType.Wildebeest: result = AnimalCatergory.Herbivore; break;

                case AnimalType.Cheetah: result = AnimalCatergory.Carnivore; break;
                case AnimalType.Hyena: result = AnimalCatergory.Carnivore; break;
                case AnimalType.Lion: result = AnimalCatergory.Carnivore; break;
                default: break;
            }
            return result;
        }

        public static float GetAvoidDistance(AnimalState state)
        {
            return (state == AnimalState.Sleeping) ? GameSettings.AvoidRadiusFactorAsleep * GameSettings.PerceptionRadius : GameSettings.AvoidRadiusFactorAwake * GameSettings.PerceptionRadius;
        }

        // Movement
        public static IDictionary<AnimalState, IDictionary<SteeringSourceType, float>> SteeringSourceModifiersByState = new Dictionary<AnimalState, IDictionary<SteeringSourceType, float>>()
        {
            {AnimalState.Neutral, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 1f},
                {SteeringSourceType.Herd, 1f},
                {SteeringSourceType.Follow, 1f},
                {SteeringSourceType.Seek, 0f},
                {SteeringSourceType.Avoid, 1f}
            }},
            {AnimalState.SeekingEntity, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 0f},
                {SteeringSourceType.Herd, 0f},
                {SteeringSourceType.Follow, 0f},
                {SteeringSourceType.Seek, 1f},
                {SteeringSourceType.Avoid, 0f}
            }},
            {AnimalState.Interacting, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 0f},
                {SteeringSourceType.Herd, 0f},
                {SteeringSourceType.Follow, 0f},
                {SteeringSourceType.Seek, 0f},
                {SteeringSourceType.Avoid, 0f}
            }},
            {AnimalState.Hunting, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 0f},
                {SteeringSourceType.Herd, 0f},
                {SteeringSourceType.Follow, 0f},
                {SteeringSourceType.Seek, 1f},
                {SteeringSourceType.Avoid, 0f}
            }},
            {AnimalState.Fleeing, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 1f},
                {SteeringSourceType.Herd, 1f},
                {SteeringSourceType.Follow, 1f},
                {SteeringSourceType.Seek, 0f},
                {SteeringSourceType.Avoid, 1f}
            }},
            {AnimalState.SeekingPosition, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 0f},
                {SteeringSourceType.Herd, 0f},
                {SteeringSourceType.Follow, 0f},
                {SteeringSourceType.Seek, 1f},
                {SteeringSourceType.Avoid, 0f}
            }},
            {AnimalState.Sleeping, new Dictionary<SteeringSourceType, float>()
            {
                {SteeringSourceType.Wander, 0f},
                {SteeringSourceType.Herd, 0f},
                {SteeringSourceType.Follow, 0f},
                {SteeringSourceType.Seek, 0f},
                {SteeringSourceType.Avoid, 0f}
            }}
        };

        public static IDictionary<AnimalState, float> MovementSpeedsByState = new Dictionary<AnimalState, float>()
        {
            {AnimalState.Neutral, 0.5f},
            {AnimalState.SeekingEntity, 0.5f},
            {AnimalState.Interacting, 0f},
            {AnimalState.Hunting, 2f},
            {AnimalState.Fleeing, 1f},
            {AnimalState.SeekingPosition, 0.5f},
            {AnimalState.Sleeping, 0f}
        };
    }
}
