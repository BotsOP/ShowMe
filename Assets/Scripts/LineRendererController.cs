using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class LineRendererController : MonoBehaviour
    {
        public LineRenderer lr;
        public Transform[] points;

        private void Awake()
        {
            
        }

        public void SetUpLine(Transform[] points)
        {
            lr.positionCount = points.Length;
            this.points = points;
        }

        private void Update()
        {
            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
    }
}