using Assets.Gamelogic.Visualizers.Ui;
using Improbable.Corelibrary.Transforms.Global;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Player
{
    public class PlayerInfoReader : MonoBehaviour
    {
        [Require] private GlobalTransformStateReader GlobalTransformStateReader;
        public Vector3 GlobalPosition;
        public Vector3 LocalPosition;

        void OnEnable()
        {
            GameInfoUi.PlayerInfoReader = this;
        }

        void OnDisable()
        {
            GameInfoUi.PlayerInfoReader = null;
        }

        void FixedUpdate()
        {
            GlobalPosition = new Vector3((float)GlobalTransformStateReader.Position.X, (float)GlobalTransformStateReader.Position.Y, (float)GlobalTransformStateReader.Position.Z);
            LocalPosition = transform.position;
        }
    }
}

