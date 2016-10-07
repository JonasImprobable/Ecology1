using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class FollowParent: MonoBehaviour
    {
        public GameObject Parent;

        void Update()
        {
            if (Parent != null)
            {
                transform.position = Parent.transform.position;
            }       
        }
    }
}
