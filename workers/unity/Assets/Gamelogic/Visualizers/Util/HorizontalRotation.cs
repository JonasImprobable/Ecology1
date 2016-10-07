using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class HorizontalRotation : MonoBehaviour
    {

        public float RotationSpeed = 80.0f;

        void Update()
        {
            transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }
}
