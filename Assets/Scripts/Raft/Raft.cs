using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raft : MonoBehaviour
{
    [SerializeField] private PathCreator path;
    [SerializeField] private int currentSegment;
    [SerializeField] private float raftSpeed = 5;

    private void FixedUpdate()
    {
        float t = Time.timeSinceLevelLoad / raftSpeed;
        OrientedPoint orientedPoint = GetOrientedPointInCurve(t % 1, (int)t % path.segmentCount);
        transform.position = orientedPoint.pos;
        transform.rotation = orientedPoint.rot;
    }

    private OrientedPoint GetOrientedPointInCurve(float t, int segment)
    {
        int index = segment * 3;
        Vector3 a = path.transformList[index].position;
        Vector3 b = path.transformList[index + 1].position;
        Vector3 c = path.transformList[index + 2].position;
        Vector3 d = path.transformList[index + 3].position;

        Vector3 e = Vector3.Lerp(a, b, t);
        Vector3 f = Vector3.Lerp(b, c, t);
        Vector3 g = Vector3.Lerp(c, d, t);

        Vector3 h = Vector3.Lerp(e, f, t);
        Vector3 i = Vector3.Lerp(f, g, t);

        Vector3 pos = Vector3.Lerp(h, i, t);
        Vector3 tangent = (h - i).normalized;

        return new OrientedPoint(pos, tangent);;
    }
}
