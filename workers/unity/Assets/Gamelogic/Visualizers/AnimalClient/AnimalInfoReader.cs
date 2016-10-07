using Improbable;
using Improbable.Animal;
using Improbable.Util;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Visualizers.AnimalClient
{
    public class AnimalInfoReader : MonoBehaviour
    {
        [Require] public AnimalInfoStateReader AnimalInfoStateReader;
        public GameObject MaleIndicatorPrefab;
        public GameObject ChildIndicatorPrefab;
        public GameObject GenderIndicator;
        //private GameObject starParticlePrefab;
        //private GameObject skullParticlePrefab;

        public AnimalType Species;
        public AnimalGender Gender;
        public AnimalVitalityStatus IsAlive;

        public float Age;
        public float RemainingLifeExpectancy;
        public bool IsAdult;

        public float Size;
        public float Priority;
        public float Speed;

        public float Stamina;
        public float Water;
        public float Food;
        public float Mating;

        public bool IsPregnant;
        public float RemainingPregnancyTime;
        public EntityId Mother;
        public float MeatAsFoodResource;

        void OnEnable()
        {
            MaleIndicatorPrefab = Resources.Load<GameObject>("Indicators/MaleIndicator");
            ChildIndicatorPrefab = Resources.Load<GameObject>("Indicators/ChildIndicator");
            //starParticlePrefab = Resources.Load<GameObject>("Particles/StarParticle");
            //skullParticlePrefab = Resources.Load<GameObject>("Particles/SkullParticle");

            Species = AnimalInfoStateReader.Species;
            Gender = AnimalInfoStateReader.Gender;
            Priority = AnimalInfoStateReader.Priority;
            Mother = AnimalInfoStateReader.Mother;

            AnimalInfoStateReader.IsAliveUpdated += OnIsAliveUpdated;
            AnimalInfoStateReader.AgeUpdated += OnAgeUpdated;
            AnimalInfoStateReader.RemainingLifeExpectancyUpdated += OnRemainingLifeExpectancyUpdated;
            AnimalInfoStateReader.IsAdultUpdated += OnIsAdultUpdated;

            AnimalInfoStateReader.SizeUpdated += OnSizeUpdated;
            AnimalInfoStateReader.SpeedUpdated += OnSpeedUpdated;

            AnimalInfoStateReader.StaminaUpdated += OnStaminaUpdated;
            AnimalInfoStateReader.WaterUpdated += OnWaterReservesUpdated;
            AnimalInfoStateReader.FoodUpdated += OnFoodReservesUpdated;
            AnimalInfoStateReader.MatingUpdated += OnMatingUpdated;

            AnimalInfoStateReader.IsPregnantUpdated += OnIsPregnantUpdated;
            AnimalInfoStateReader.RemainingPregnancyTimeUpdated += OnRemainingPregnancyTimeUpdated;
            AnimalInfoStateReader.MeatAsFoodResourceUpdated += OnMeatAsFoodResourceUpdated;
        }

        void OnDisable()
        {
            AnimalInfoStateReader.IsAliveUpdated -= OnIsAliveUpdated;
            AnimalInfoStateReader.AgeUpdated -= OnAgeUpdated;
            AnimalInfoStateReader.RemainingLifeExpectancyUpdated -= OnRemainingLifeExpectancyUpdated;
            AnimalInfoStateReader.IsAdultUpdated -= OnIsAdultUpdated;

            AnimalInfoStateReader.SizeUpdated -= OnSizeUpdated;
            AnimalInfoStateReader.SpeedUpdated -= OnSpeedUpdated;

            AnimalInfoStateReader.StaminaUpdated -= OnStaminaUpdated;
            AnimalInfoStateReader.WaterUpdated -= OnWaterReservesUpdated;
            AnimalInfoStateReader.FoodUpdated -= OnFoodReservesUpdated;
            AnimalInfoStateReader.MatingUpdated -= OnMatingUpdated;

            AnimalInfoStateReader.IsPregnantUpdated -= OnIsPregnantUpdated;
            AnimalInfoStateReader.RemainingPregnancyTimeUpdated -= OnRemainingPregnancyTimeUpdated;
            AnimalInfoStateReader.MeatAsFoodResourceUpdated -= OnMeatAsFoodResourceUpdated;
        }

        void UpdateGenderIndicator()
        {
            if (IsAlive == AnimalVitalityStatus.Alive)
            {
                if (!IsAdult)
                {
                    if (GenderIndicator != null)
                    {
                        Destroy(GenderIndicator);
                    }
                    GenderIndicator = (GameObject) Instantiate(ChildIndicatorPrefab, transform.position, transform.rotation);
                    GenderIndicator.transform.localScale = new Vector3(GetComponent<BoxCollider>().size.z, 1.0f, GetComponent<BoxCollider>().size.z);
                    GenderIndicator.transform.parent = transform;

                }
                else if (Gender == AnimalGender.Male)
                {
                    if (GenderIndicator != null)
                    {
                        Destroy(GenderIndicator);
                    }
                    GenderIndicator = (GameObject) Instantiate(MaleIndicatorPrefab, transform.position, transform.rotation);
                    GenderIndicator.transform.localScale = new Vector3(GetComponent<BoxCollider>().size.z, 1.0f, GetComponent<BoxCollider>().size.z);
                    GenderIndicator.transform.parent = transform;
                }
                else
                {
                    if (GenderIndicator != null)
                    {
                        Destroy(GenderIndicator);
                    }
                }
            }
            else
            {
                if (GenderIndicator != null)
                {
                    Destroy(GenderIndicator);
                }
            }
        }

        void OnIsAliveUpdated(AnimalVitalityStatus i)
        {
            IsAlive = i;
            UpdateGenderIndicator();
            /*
            if (IsAlive == AnimalVitalityStatus.Alive)
            {
                Instantiate(starParticlePrefab, transform.position + Vector3.up * GetComponent<BoxCollider>().size.y * 1.2f, transform.rotation);
            }
            else
            {
                Instantiate(skullParticlePrefab, transform.position + Vector3.up * GetComponent<BoxCollider>().size.y * 1.2f, transform.rotation);
            }
            */
        }
        
        void OnAgeUpdated(float a)
        {
            Age = a;
        }

        void OnRemainingLifeExpectancyUpdated(float r)
        {
            RemainingLifeExpectancy = r;
        }

        void OnIsAdultUpdated(bool i)
        {
            IsAdult = i;
            UpdateGenderIndicator();
        }

        void OnSizeUpdated(float s)
        {
            Size = s;
            transform.localScale = Vector3.one * Size;
        }

        void OnSpeedUpdated(float s)
        {
            Speed = s;
        }

        void OnStaminaUpdated(float s)
        {
            Stamina = s;
        }

        void OnWaterReservesUpdated(float w)
        {
            Water = w;
        }

        void OnFoodReservesUpdated(float f)
        {
            Food = f;
        }

        void OnMatingUpdated(float m)
        {
            Mating = m;
        }

        void OnIsPregnantUpdated(bool i)
        {
            IsPregnant = i;
        }

        void OnRemainingPregnancyTimeUpdated(float r)
        {
            RemainingPregnancyTime = r;
        }

        void OnMeatAsFoodResourceUpdated(float m)
        {
            MeatAsFoodResource = m;
        }
    }
}
