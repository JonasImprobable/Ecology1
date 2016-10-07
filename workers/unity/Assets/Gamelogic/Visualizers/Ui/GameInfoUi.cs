using Assets.Gamelogic.Visualizers.Global;
using Assets.Gamelogic.Visualizers.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Visualizers.Ui
{
    public class GameInfoUi : MonoBehaviour
    {
        public GameInfoReader GameInfoReader;
        public static PlayerControlsReader PlayerControlsReader;
        public static PlayerInfoReader PlayerInfoReader;
        public Text TextField;
        public Slider GameTimeSlider;

        void OnEnable()
        {
            TextField = transform.GetChild(0).GetComponent<Text>();
            GameTimeSlider = transform.GetChild(1).GetComponent<Slider>();
        }

        void Update()
        {
            if (!GameInfoReader)
            {
                GameInfoReader = FindObjectOfType<GameInfoReader>();
            }
            RefreshDisplay();
            UpdateGameInfoText();
            UpdatePlayerInfoText();
            UpdatePlayerControlsText();
        }

        void RefreshDisplay()
        {
            TextField.text = "";
        }

        void UpdateGameInfoText()
        {
            if (GameInfoReader != null)
            {
                TextField.text += "Day " + (int)GameInfoReader.GameTime/24 + ", " + (int)GameInfoReader.GameTime%24 + "h\n";
                TextField.text += "\n";
                GameTimeSlider.value = GameInfoReader.GameTime%24;
                int totalCount = 0;
                foreach (var elem in GameInfoReader.EntityRegistry)
                {
                    TextField.text += elem.Key + ": " + elem.Value + "\n";
                    totalCount += elem.Value;
                }
                TextField.text += "Total: " + totalCount + "\n";
                TextField.text += "\n";
            }
        }

        void UpdatePlayerInfoText()
        {
            if (PlayerInfoReader != null)
            {
                TextField.text += "GlPos: " + PlayerInfoReader.GlobalPosition + "\n";
                TextField.text += "LoPos: " + PlayerInfoReader.LocalPosition + "\n";
            }
        }

        void UpdatePlayerControlsText()
        {
            if (PlayerControlsReader != null)
            {
                TextField.text += "Active Gender: " + PlayerControlsReader.ActiveAnimalGender + "\n";
            }
        }
    }
}
