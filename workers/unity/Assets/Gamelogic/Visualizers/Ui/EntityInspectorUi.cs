using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.Environment;
using System;
using Improbable.Unity.Common.Core.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.Visualizers.Ui
{
    public class EntityInspectorUi : MonoBehaviour
    {
        public static AnimalInfoReader AnimalInfoReader;
        public static AnimalPerceptionReader AnimalPerceptionReader;
        public static AnimalMovementReader AnimalMovementReader;
        public static AnimalStateMachineReader AnimalStateMachineReader;
        public static EnvironmentInfoReader EnvironmentInfoReader;
        public Text TextField;
        public Slider StaminaSlider;
        public Slider WaterSlider;
        public Slider FoodSlider;
        public Slider MatingSlider;
        public Slider LifeExpectancySlider;
        public Slider PregnancySlider;

        void OnEnable()
        {
            TextField = GetComponentInChildren<Text>();
            StaminaSlider = transform.GetChild(1).GetComponent<Slider>();
            WaterSlider = transform.GetChild(2).GetComponent<Slider>();
            FoodSlider = transform.GetChild(3).GetComponent<Slider>();
            MatingSlider = transform.GetChild(4).GetComponent<Slider>();
            LifeExpectancySlider = transform.GetChild(5).GetComponent<Slider>();
            PregnancySlider = transform.GetChild(6).GetComponent<Slider>();
        }

        void Update()
        {
            RefreshDisplay();
            UpdateAnimalInfoText();
            UpdateAnimalStateMachineText();
            UpdateAnimalPreceptionText();
            UpdateAnimalMovementText();
            UpdateEnvironmentInfoText();
        }

        void RefreshDisplay()
        {
            TextField.text = "";
            StaminaSlider.gameObject.SetActive(false);
            WaterSlider.gameObject.SetActive(false);
            FoodSlider.gameObject.SetActive(false);
            MatingSlider.gameObject.SetActive(false);
            LifeExpectancySlider.gameObject.SetActive(false);
            PregnancySlider.gameObject.SetActive(false);
        }

        void UpdateAnimalInfoText()
        {
            if (AnimalInfoReader != null)
            {
                String isAdultappendix = (AnimalInfoReader.IsAdult) ? ", adult" : ", child";
                String isPregnantAppendix = (AnimalInfoReader.IsPregnant) ? ", pregnant" : "";
                TextField.text += "[" + AnimalInfoReader.gameObject.EntityId() + "] " + AnimalInfoReader.Species + " (" + AnimalInfoReader.Gender + isAdultappendix + isPregnantAppendix + "), Status: " + AnimalInfoReader.IsAlive + "\n";
                TextField.text += "Stamina: " + AnimalInfoReader.Stamina + "\n";
                TextField.text += "Water: " + AnimalInfoReader.Water + "\n";
                TextField.text += "Food: " + AnimalInfoReader.Food + "\n";
                TextField.text += "Mating: " + AnimalInfoReader.Mating + "\n";
                TextField.text += "Life: " + AnimalInfoReader.RemainingLifeExpectancy + "\n";
                TextField.text += "Pregnancy: " + AnimalInfoReader.RemainingPregnancyTime + "\n";
                TextField.text += "Mother: " + AnimalInfoReader.Mother + ", FoodRes: " + AnimalInfoReader.MeatAsFoodResource + "\n";
                TextField.text += "Priority: " + AnimalInfoReader.Priority + "\n";
                TextField.text += "\n";

                StaminaSlider.gameObject.SetActive(true);
                WaterSlider.gameObject.SetActive(true);
                FoodSlider.gameObject.SetActive(true);
                MatingSlider.gameObject.SetActive(true);
                LifeExpectancySlider.gameObject.SetActive(true);
                PregnancySlider.gameObject.SetActive(true);
                StaminaSlider.value = AnimalInfoReader.Stamina;
                WaterSlider.value = AnimalInfoReader.Water;
                FoodSlider.value = AnimalInfoReader.Food;
                MatingSlider.value = AnimalInfoReader.Mating;
                LifeExpectancySlider.value = AnimalInfoReader.RemainingLifeExpectancy;
                PregnancySlider.value = AnimalInfoReader.RemainingPregnancyTime;
            }
        }

        void UpdateAnimalStateMachineText()
        {
            if (AnimalStateMachineReader != null)
            {
                TextField.text += "State: " + AnimalStateMachineReader.CurrentState + ", duration: " + AnimalStateMachineReader.CurrentStateDuration + "\n";
                TextField.text += "Target Need: " + AnimalStateMachineReader.TargetNeed + ", Target Entity: " + AnimalStateMachineReader.TargetEntity + "\n";
                TextField.text += "Entity Cds: " + AnimalStateMachineReader.EntityCooldowns.Count + ", Needs Cds: " + AnimalStateMachineReader.NeedsCooldowns.Count + " \n";
            }
        }

        void UpdateAnimalPreceptionText()
        {
            if (AnimalPerceptionReader != null)
            {
                TextField.text += "Perc Environment: " + AnimalPerceptionReader.PerceivedEnvironmentEntities.Count + " \n";
                TextField.text += "Perc Animals: " + AnimalPerceptionReader.PerceivedAnimals.Count + " \n";
            }
        }

        void UpdateAnimalMovementText()
        {
            if (AnimalMovementReader != null)
            {
                TextField.text += "Target Direction: " + AnimalMovementReader.SteeringSourceAggregate.direction.ToUnityVector() + "\n";
                TextField.text += "Target Speed: " + AnimalMovementReader.SteeringSourceAggregate.intensity + "\n";
                TextField.text += "Steering Sources: " + AnimalMovementReader.SteeringSources.Count + " [\n";
                string mapString = "";
                foreach (var item in AnimalMovementReader.SteeringSources)
                {
                    mapString += item.Key + ", dir: " + item.Value.direction.ToUnityVector() + ", mag: " + item.Value.direction.ToUnityVector().magnitude + ", i: " + item.Value.intensity + ";\n";
                }
                TextField.text += mapString + "]\n";
            }
        }

        void UpdateEnvironmentInfoText()
        {
            if (EnvironmentInfoReader != null)
            {
                TextField.text += "[" + EnvironmentInfoReader.gameObject.EntityId() + "] " + EnvironmentInfoReader.EnvironmentType + "\n";
                TextField.text += "Resources: " + EnvironmentInfoReader.Resources + "\n";
                TextField.text += "\n";

                StaminaSlider.gameObject.SetActive(true);
                StaminaSlider.value = EnvironmentInfoReader.Resources;
            }
        }

        public static void SelectAnimal(GameObject selection)
        {
            AnimalInfoReader = selection.GetComponent<AnimalInfoReader>();
            AnimalStateMachineReader = selection.GetComponent<AnimalStateMachineReader>();
            AnimalPerceptionReader = selection.GetComponent<AnimalPerceptionReader>();
            AnimalMovementReader = selection.GetComponent<AnimalMovementReader>();
        }

        public static void SelectEnvironment(GameObject selection)
        {
            EnvironmentInfoReader = selection.GetComponent<EnvironmentInfoReader>();
        }

        public static void DeselectCurrent()
        {
            AnimalInfoReader = null;
            AnimalPerceptionReader = null;
            AnimalMovementReader = null;
            AnimalStateMachineReader = null;
            EnvironmentInfoReader = null;
        }
    }
}
