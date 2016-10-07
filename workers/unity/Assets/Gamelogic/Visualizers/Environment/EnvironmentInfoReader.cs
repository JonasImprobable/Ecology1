using UnityEngine;
using Improbable.EnvironmentStates;
using Improbable.Unity.Visualizer;
using Improbable.Util;

namespace Assets.Gamelogic.Visualizers.Environment
{
    public class EnvironmentInfoReader : MonoBehaviour
    {
        [Require] public EnvironmentInfoStateReader EnvironmentInfoStateReader;
        public bool RotationSet;
        public EnvironmentType EnvironmentType;
        public float Resources;
        public bool Traversable;
        public float Size;
        public float Rotation;

        void OnEnable()
        {
            RotationSet = false;
            EnvironmentType = EnvironmentInfoStateReader.EnvironmentType;
            EnvironmentInfoStateReader.ResourcesUpdated += OnResourcesUpdated;
            EnvironmentInfoStateReader.TraversableUpdated += OnTraversableUpdated;
            EnvironmentInfoStateReader.SizeUpdated += OnSizeUpdated;
            EnvironmentInfoStateReader.RotationUpdated += OnRotationUpdated;
        }

        void OnDisable()
        {
            EnvironmentInfoStateReader.ResourcesUpdated -= OnResourcesUpdated;
            EnvironmentInfoStateReader.TraversableUpdated -= OnTraversableUpdated;
            EnvironmentInfoStateReader.SizeUpdated -= OnSizeUpdated;
            EnvironmentInfoStateReader.RotationUpdated -= OnRotationUpdated;
        }

        void OnResourcesUpdated(float r)
        {
            Resources = r;
        }

        void OnTraversableUpdated(bool t)
        {
            Traversable = t;
        }

        void OnSizeUpdated(float s)
        {
            Size = s;
            if (EnvironmentType != EnvironmentType.Lake)
            {
                transform.localScale = new Vector3(s, s, s);
            }
            else
            {
                transform.localScale = new Vector3(s * 2, 2f, s * 2);
            }
        }

        void OnRotationUpdated(float r)
        {
            Rotation = r;
        }

        void Update()
        {
            if (!RotationSet)
            {
                transform.rotation = Quaternion.Euler(0.0f, 360.0f * Rotation, 0.0f);
                RotationSet = true;
            }
        }
    }
}
