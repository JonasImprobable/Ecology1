using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class EnvironmentProperties : MonoBehaviour
    {
        public static EnvironmentCategory GetCategoryFromType(EnvironmentType environmentType)
        {
            EnvironmentCategory result = EnvironmentCategory.Plant;
            switch (environmentType)
            {
                case EnvironmentType.Lake: result = EnvironmentCategory.WaterSource; break;
                case EnvironmentType.Steak: result = EnvironmentCategory.Meat; break;
                case EnvironmentType.Rock: result = EnvironmentCategory.Obstacle; break;

                case EnvironmentType.Grass: result = EnvironmentCategory.Plant; break;
                case EnvironmentType.Bush: result = EnvironmentCategory.Plant; break;
                case EnvironmentType.Tree: result = EnvironmentCategory.Plant; break;
                default: break;
            }
            return result;
        }
    }
}
