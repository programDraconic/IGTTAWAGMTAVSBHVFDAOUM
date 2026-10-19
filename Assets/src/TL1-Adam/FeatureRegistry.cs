using System.Collections.Generic;
using UnityEngine;

// Owns the registered features (composition in the diagram).
public class FeatureRegistry
{
    private readonly List<IGameplayFeature> features = new List<IGameplayFeature>();
    private readonly object context;

    public FeatureRegistry(object context = null)
    {
        this.context = context;
    }

    public int Count { get { return features.Count; } }

    public void RegisterFeature(IGameplayFeature feature)
    {
        if (feature == null || features.Contains(feature)) return;

        if (feature.Initialize(context))
            features.Add(feature);
        else
            Debug.LogWarning("Feature failed to initialize and was not registered: " + feature.GetType().Name);
    }

    public void DispatchTick(FrameContext frame)
    {
        for (int i = 0; i < features.Count; i++)
            features[i].Tick(frame);
    }

    public void ShutdownAll()
    {
        for (int i = 0; i < features.Count; i++)
            features[i].Shutdown();
        features.Clear();
    }
}
