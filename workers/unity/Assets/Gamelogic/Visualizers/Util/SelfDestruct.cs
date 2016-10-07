using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class SelfDestruct : MonoBehaviour
    {
        public float RemainingLifeTime = 2f;

        void Update()
        {
            RemainingLifeTime -= Time.deltaTime;
            if (RemainingLifeTime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}

