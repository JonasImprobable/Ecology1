using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class MathUtils : MonoBehaviour
    {
        public static Vector3 FlattenVector(Vector3 v)
        {
            return new Vector3(v.x, 0.0f, v.z);
        }

        public static Vector3 FlattenVectorRotation(Vector3 v)
        {
            return new Vector3(0.0f, v.y, 0.0f);
        }

        public static Vector3 GetAverageVector(IList<Vector3> vectors)
        {
            Vector3 result = Vector3.zero;
            for (int i = 0; i < vectors.Count; i++)
            {
                result += vectors[i]/vectors.Count;
            }
            return result;
        }

        public static Vector3 ClampNormalise(Vector3 v)
        {
            if (v.magnitude > 1f)
            {
                return v.normalized;
            }
            return v;
        }
    }
}

