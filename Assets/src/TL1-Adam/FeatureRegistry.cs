using System.Collections.Generic;
using UnityEngine;

// Owns the registered features (composition in the diagram).
// Added beyond the diagram: UnregisterFeature, so destroyed GameObjects
// don't leave dead references behind.
public class FeatureRegistry
{
    private readonly List<IGameplayFeature> features = new List<IGameplayFeature>();
    private readonly HashSet<IGameplayFeature> lookup = new HashSet<IGameplayFeature>();
    private readonly object context;

    public FeatureRegistry(object context = null)
    {
        this.context = context;
    }

    public int Count { get { return features.Count; } }

    public void RegisterFeature(IGameplayFeature feature)
    {
        if (feature == null || lookup.Contains(feature)) return;

        if (feature.Initialize(context))
        {
            features.Add(feature);
            lookup.Add(feature);
        }
        else
        {
            Debug.LogWarning("Feature failed to initialize and was not registered: " + feature.GetType().Name);
        }
    }

    public void UnregisterFeature(IGameplayFeature feature)
    {
        if (feature == null || !lookup.Remove(feature)) return;

        features.Remove(feature);
        feature.Shutdown();
    }

    public void DispatchTick(FrameContext frame)
    {
        // Backwards so a feature that unregisters itself mid-tick is safe.
        for (int i = features.Count - 1; i >= 0; i--)
        {
            if (i < features.Count)
                features[i].Tick(frame);
        }
    }

    public void ShutdownAll()
    {
        for (int i = 0; i < features.Count; i++)
            features[i].Shutdown();
        features.Clear();
        lookup.Clear();
    }
}
