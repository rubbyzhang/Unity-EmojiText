using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CircleOutline : ModifiedShadow
{
    [SerializeField]
    int CircleCount = 4;
    [SerializeField]
    int mFirstCircleSampleCount = 1;
    [SerializeField]
    int mSampleCountIncrement = 2;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        TotalCircleCount = CircleCount;
        FirstCircleSampleCount = mFirstCircleSampleCount;
        SampleCountIncrement = mSampleCountIncrement;
    }
#endif

    public int TotalCircleCount
    {
        get
        {
            return CircleCount;
        }

        set
        {
            CircleCount = Mathf.Max(value, 1);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int FirstCircleSampleCount
    {
        get
        {
            return mFirstCircleSampleCount;
        }

        set
        {
            mFirstCircleSampleCount = Mathf.Max(value, 2);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public int SampleCountIncrement
    {
        get
        {
            return mSampleCountIncrement;
        }

        set
        {
            mSampleCountIncrement = Mathf.Max(value, 1);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive())
            return;

        var total = (mFirstCircleSampleCount * 2 + mSampleCountIncrement * (CircleCount - 1)) * CircleCount / 2;
        var neededCapacity = verts.Count * (total + 1);
        if (verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;
        var original = verts.Count;
        var count = 0;
        var sampleCount = mFirstCircleSampleCount;
        var dx = effectDistance.x / CircleCount;
        var dy = effectDistance.y / CircleCount;
        for (int i = 1; i <= CircleCount; i++)
        {
            var rx = dx * i;
            var ry = dy * i;
            var radStep = 2 * Mathf.PI / sampleCount;
            var rad = (i % 2) * radStep * 0.5f;
            for (int j = 0; j < sampleCount; j++)
            {
                var next = count + original;
                ApplyShadow(verts, effectColor, count, next, rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
                count = next;
                rad += radStep;
            }
            sampleCount += mSampleCountIncrement;
        }
    }
}
