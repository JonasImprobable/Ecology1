using Assets.Gamelogic.Visualizers.Environment;
using Improbable.Corelib.PrefabExporting.PreProcessors;
using Improbable.CoreLibrary.Transforms;
using Improbable.Unity;
using System;
using System.Collections.Generic;

namespace Assets.Gamelogic.Visualizers.Util
{
    public class AttachEnvironmentScripts : PreProcessorBase
    {
        public override void ExportProcess(EnginePlatform enginePlatform)
        {
            var transformNature = gameObject.AddComponent<TransformNature>();
            transformNature.ClientNonAuthoritativeMode = TransformNature.NonAuthoritativeMode.Static;
            transformNature.ClientCanBeAuthoritative = false;
            transformNature.ClientEnableGlobalFallback = false;
            transformNature.FSimNonAuthoritativeMode = TransformNature.NonAuthoritativeMode.Static;
            transformNature.FSimCanBeAuthoritative = false;
            transformNature.FSimEnableGlobalFallback = false;
            transformNature.GameObjectCanBeParent = false;
            transformNature.GameObjectCanBeParented = false;

            transformNature.ExportProcess(enginePlatform);

            base.ExportProcess(enginePlatform);
        }

        protected override Dictionary<Type, VisualizerPreProcessorConfig> GetCommonVisualizersToAdd()
        {
            return new Dictionary<Type, VisualizerPreProcessorConfig>
            {
                {typeof(EnvironmentInfoReader), VisualizerPreProcessorConfig.DefaultInstance}
            };
        }
    }
}
