using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class MeshModTextGlow : BaseMeshEffect
{
    [Range(0, 10)]
    public float m_size = 3.0f;
    // defines the glow color + opacity
    public Color m_glowColor;
    // use temporary list to prevent allocations
    private static readonly List<UIVertex> s_tempVertices = new List<UIVertex>();

    void Update()
    {
        if(transform.hasChanged)
        {
            graphic.SetVerticesDirty();
        }
    }

    public override void ModifyMesh(VertexHelper _vh)
    {
        _vh.GetUIVertexStream(s_tempVertices);
        // for every triangle...
        for (var i = 0; i <= s_tempVertices.Count - 3; i += 3)
        {
            UIVertex v0 = s_tempVertices[i + 0];
            UIVertex v1 = s_tempVertices[i + 1];
            UIVertex v2 = s_tempVertices[i + 2];
            // 2D points please
            var xy0 = new Vector2(v0.position.x, v0.position.y);
            var xy1 = new Vector2(v1.position.x, v1.position.y);
            var xy2 = new Vector2(v2.position.x, v2.position.y);
            // build two vectors
            Vector2 deltaA = (xy1 - xy0).normalized;
            Vector2 deltaB = (xy2 - xy1).normalized;
            Vector2 vecUvX;
            Vector2 vecUvY;
            Vector2 vecX;
            Vector2 vecY;
            // calculate UV vectors for the X and Y axes
            if (Mathf.Abs(Vector2.Dot(deltaA, Vector2.right)) > Mathf.Abs(Vector2.Dot(deltaB, Vector2.right)))
            {
                vecX = xy1 - xy0;
                vecY = xy2 - xy1;
                vecUvX = v1.uv0 - v0.uv0;
                vecUvY = v2.uv0 - v1.uv0;
            }
            else
            {
                vecX = xy2 - xy1;
                vecY = xy1 - xy0;
                vecUvX = v2.uv0 - v1.uv0;
                vecUvY = v1.uv0 - v0.uv0;
            }
            // retrieve UV minimum and maximum
            Vector2 uvMin = Min(v0.uv0, v1.uv0, v2.uv0);
            Vector2 uvMax = Max(v0.uv0, v1.uv0, v2.uv0);
            // also retrieve the XY mininum and maximum
            float xMin = Min(v0.position.x, v1.position.x, v2.position.x);
            float yMin = Min(v0.position.y, v1.position.y, v2.position.y);
            float xMax = Max(v0.position.x, v1.position.x, v2.position.x);
            float yMax = Max(v0.position.y, v1.position.y, v2.position.y);
            var xyMin = new Vector2(xMin, yMin);
            var xyMax = new Vector2(xMax, yMax);
            // store UV min. and max. in the tangent of each vertex
            var tangent = new Vector4(uvMin.x, uvMin.y, uvMax.x, uvMax.y);

            //tangent.y = tangent.y / transform.lossyScale.y * transform.lossyScale.x;
           // tangent.z = tangent.z / transform.lossyScale.z * transform.lossyScale.x;

            // calculate center of UV and pos
            Vector2 xyCenter = (xyMin + xyMax) * 0.5f;
            // we need the vector lengths inside our loop, precalculate them here
            float vecXLen = vecX.magnitude;
            float vecYLen = vecY.magnitude;

            // now manipulate each vertex
            for (var v = 0; v < 3; ++v)
            {
                UIVertex vertex = s_tempVertices[i + v];
                // extrude each vertex to the outside 'm_size' pixels wide.
                // we need the extrude to create more space for the glow,
                var posOld = new Vector2(vertex.position.x, vertex.position.y);
                Vector2 posNew = posOld;
                float addX = (vertex.position.x > xyCenter.x) ? m_size : -m_size;
                float addY = (vertex.position.y > xyCenter.y) ? m_size : -m_size;
                float signX = Vector2.Dot(vecX, Vector2.right) > 0 ? 1 : -1;
                float signY = Vector2.Dot(vecY, Vector2.up) > 0 ? 1 : -1;
                posNew.x += addX;
                posNew.y += addY;
                vertex.position = new Vector3(posNew.x, posNew.y, m_glowColor.a);
                // re-calculate UVs accordingly to prevent scaled texts
                Vector2 uvOld = vertex.uv0;
                vertex.uv0 += vecUvX / vecXLen * addX * signX;
                vertex.uv0 += vecUvY / vecYLen * addY * signY;
                // set the tangent so we know the UV boundaries. We use this to
                // prevent smearing into other characters in the texture atlas
                vertex.tangent = tangent;
                // normal is used as glow color
                vertex.normal.x = m_glowColor.r;
                vertex.normal.y = m_glowColor.g /*/ transform.lossyScale.y * transform.lossyScale.x*/;
                vertex.normal.z = m_glowColor.b /*/ transform.lossyScale.z * transform.lossyScale.x*/;
                //// uv1 is glow size
                vertex.uv1 = vertex.uv0 - uvOld;
                //// needs to be positive
                vertex.uv1.x = Mathf.Abs(vertex.uv1.x);
                vertex.uv1.y = Mathf.Abs(vertex.uv1.y);
                s_tempVertices[i + v] = vertex;
            }
        }
        _vh.Clear();
        _vh.AddUIVertexTriangleStream(s_tempVertices);
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        graphic.SetVerticesDirty();
    }
#endif
    private static float Min(float _a, float _b, float _c)
    {
        return Mathf.Min(_a, Mathf.Min(_b, _c));
    }
    private static float Max(float _a, float _b, float _c)
    {
        return Mathf.Max(_a, Mathf.Max(_b, _c));
    }
    private static Vector2 Min(Vector2 _a, Vector2 _b, Vector2 _c)
    {
        return new Vector2(Min(_a.x, _b.x, _c.x), Min(_a.y, _b.y, _c.y));
    }
    private static Vector2 Max(Vector2 _a, Vector2 _b, Vector2 _c)
    {
        return new Vector2(Max(_a.x, _b.x, _c.x), Max(_a.y, _b.y, _c.y));
    }
}