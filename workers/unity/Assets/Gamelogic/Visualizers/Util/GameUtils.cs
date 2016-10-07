using System.Collections.Generic;
using Improbable.Animal;
using Improbable.Collections;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class GameUtils : MonoBehaviour
    {
        public struct AnimalResourceTuple
        {
            public AnimalResource AnimalResource;
            public float Value;

            public AnimalResourceTuple(AnimalResource a, float v)
            {
                AnimalResource = a;
                Value = v;
            }
        }

        public static bool IsObstacleEnvironment(AnimalResource ownNeed, PerceivedEnvironmentInfo otherEntity) //need denotes either target need or current need
        {
            return (otherEntity.environmentType == EnvironmentType.Lake && ownNeed != AnimalResource.Water) ||
                   (!otherEntity.traversable && ownNeed == AnimalResource.Invalid);
        }

        public static bool IsObstacleAnimal(AnimalType ownSpecies, PerceivedAnimalInfo otherEntity)
        {
            return AnimalProperties.GetCategoryFromType(ownSpecies) == AnimalCatergory.Herbivore && //avoiding other species is feature creeped into herdsteering, this here is only for occlusion detection and drastic avoidance
                   AnimalProperties.GetCategoryFromType(otherEntity.species) == AnimalCatergory.Carnivore;
        }

        public static IDictionary<A, B> CopyMapToDictionary<A, B>(Map<A, B> m)
        {
            IDictionary<A,B> result = new Dictionary<A, B>();
            foreach (var item in m)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
