using Assets.Gamelogic.Visualizers.Ui;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using Improbable.Util;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.Player
{
    public class PlayerControlsReader : MonoBehaviour
    {
        [Require] public PlayerControlsStateReader PlayerControlsStateReader;
        public AnimalGender ActiveAnimalGender;
        
        void OnEnable()
        {
            GameInfoUi.PlayerControlsReader = this;
            PlayerControlsStateReader.ActiveAnimalGenderUpdated += OnActiveAnimalGenderUpdated;
        }

        void OnDisable()
        {
            GameInfoUi.PlayerControlsReader = null;
            PlayerControlsStateReader.ActiveAnimalGenderUpdated -= OnActiveAnimalGenderUpdated;
        }

        void OnActiveAnimalGenderUpdated(AnimalGender a)
        {
            ActiveAnimalGender = a;
        }
    }
}
