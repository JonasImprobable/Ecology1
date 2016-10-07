using Assets.Gamelogic.Visualizers.AnimalClient;
using Assets.Gamelogic.Visualizers.AnimalFsim;
using Improbable.Corelib.PrefabExporting.PreProcessors;
using Improbable.CoreLibrary.Transforms;
using Improbable.Unity;
using System;
using System.Collections.Generic;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class AttachAnimalScripts : PreProcessorBase
    {
        public override void ExportProcess(EnginePlatform enginePlatform)
        {
            var transformNature = gameObject.AddComponent<TransformNature>();
            transformNature.NetworkUpdatePeriodThreshold = 0.1f;
            transformNature.ExportProcess(enginePlatform);

            base.ExportProcess(enginePlatform);
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetCommonVisualizersToAdd()
        { 
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                {typeof(RotateTowardsMovementDirection), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalDeathAnimation), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalInteractionMovement), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalGameInfoManager), VisualizerPreProcessorConfig.DefaultInstance},

                {typeof(AnimalGameInfoReader), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalInfoReader), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalPerceptionManager), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalPerceptionReader), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalMovementManager), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalMovementReader), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalStateMachineReader), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalStateMachineManager), VisualizerPreProcessorConfig.DefaultInstance},
                {typeof(AnimalMemoryReader), VisualizerPreProcessorConfig.DefaultInstance}
            };
        }
    }
}
