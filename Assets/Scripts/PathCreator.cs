using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    public List<Transform> transformList;
    [SerializeField] private GameObject original;
    [SerializeField]private float sphereSize = 0.1f;
    private Texture2D icon;
    public int segmentCount => transformList.Count / 3;
    private void OnDrawGizmos()
    {
        if (icon == null)
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Textures/emptyImage.png");
        }

        StartPoints();
        DrawBezierCurvesGizmos();
        MirrorHandles();
    }

    private void StartPoints()
    {
        if (transformList.Count <= 0)
        {
            Debug.Log($"test");
            Transform point1 = Instantiate(original, Vector3.left, quaternion.identity, transform).transform;
            Transform point2 = Instantiate(original, (Vector3.left + Vector3.forward) * 0.5f, quaternion.identity, point1).transform;
            Transform point4 = Instantiate(original, Vector3.right, quaternion.identity, transform).transform;
            Transform point3 = Instantiate(original, (Vector3.right + Vector3.forward) * 0.5f, quaternion.identity, point4).transform;

            transformList.Add(point1);
            transformList.Add(point2);
            transformList.Add(point3);
            transformList.Add(point4);
        }
    }
    private void DrawBezierCurvesGizmos()
    {
        for (int i = 0; i < transformList.Count; i++)
        {
            if (i % 3 == 0)
            {
                Gizmos.color = Color.red;
                if (i != 0)
                {
                    Vector3 startPoint = transformList[i - 3].position;
                    Vector3 endPoint = transformList[i].position;
                    Vector3 startTangent = transformList[i - 2].position;
                    Vector3 endTangent = transformList[i - 1].position;
                    Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, Color.white, EditorGUIUtility.whiteTexture, 1f);
                }
            }
            else
            {
                Gizmos.color = Color.white;
                int currengtSegmentCount = i / 3;
                
                if (currengtSegmentCount % 2 == 0)
                {
                    if (i % 2 == 0)
                    {
                        Gizmos.DrawLine(transformList[i + 1].position, transformList[i].position);
                    }
                    else
                    {
                        Gizmos.DrawLine(transformList[i - 1].position, transformList[i].position);
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        Gizmos.DrawLine(transformList[i - 1].position, transformList[i].position);

                    }
                    else
                    {
                        Gizmos.DrawLine(transformList[i + 1].position, transformList[i].position);
                    }
                }
                
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawSphere(transformList[i].position, sphereSize);
        }
    }
    private void MirrorHandles()
    {
        bool selectedPoint = false;
        int index = 0;
        //check if selected object is in transformList
        foreach (var t in transformList)
        {
            if (!selectedPoint)
            {
                selectedPoint = Selection.Contains(t.gameObject);
            }
            else  //Found selected point inside list
            {
                var selected = Selection.activeObject;
                Transform selectedTransform = selected.GameObject().transform;
                
                //check at what index the selected object is
                for (int i = 0; i < transformList.Count; i++)
                {
                    if (selectedTransform == transformList[i])
                    {
                        index = i;
                    }
                }
                
                //Mirror handle on opposite side for smooth curves
                //Check if point is handle and not the first handle or last handle
                if (index % 3 != 0)
                {
                    if (index > 1 && index < transformList.Count - 2)
                    {
                        int currengtSegmentCount = index / 3;
                
                        if (currengtSegmentCount % 2 == 0)
                        {
                            if (index % 2 == 0)
                            {
                                Vector3 dir = transformList[index].position - transformList[index + 1].position;
                                transformList[index + 2].position = -dir + transformList[index + 1].position;
                            }
                            else
                            {
                                Vector3 dir = transformList[index].position - transformList[index - 1].position;
                                transformList[index - 2].position = -dir + transformList[index - 1].position;
                            }
                        }
                        else
                        {
                            if (index % 2 == 0)
                            {
                                Vector3 dir = transformList[index].position - transformList[index - 1].position;
                                transformList[index - 2].position = -dir + transformList[index - 1].position;
                            }
                            else
                            {
                                Vector3 dir = transformList[index].position - transformList[index + 1].position;
                                transformList[index + 2].position = -dir + transformList[index + 1].position;
                            }
                        }
                    }
                }
            }
        }
    }

    public void AddSegment()
    {
        int maxCount = transformList.Count - 1;
        Vector3 lastPos = transformList[maxCount].position;

        Vector3 localScale = transform.localScale;
        Transform point1 = Instantiate(original, lastPos + (Vector3.left + Vector3.forward) * (0.5f * 2 * localScale.x), quaternion.identity, transformList[^1]).transform;
        Transform point3 = Instantiate(original, lastPos + Vector3.right * (2 * localScale.x), quaternion.identity, transform).transform;
        Transform point2 = Instantiate(original, lastPos + (Vector3.right + Vector3.forward) * (0.5f * 2 * localScale.x), quaternion.identity, point3).transform;

        transformList.Add(point1);
        transformList.Add(point2);
        transformList.Add(point3);
    }

    public void RemoveSegment()
    {
        int maxCount = transformList.Count - 1;
        GameObject point1 = transformList[^1].gameObject;
        GameObject point2 = transformList[^2].gameObject;
        GameObject point3 = transformList[^3].gameObject;
        
        transformList.RemoveRange(maxCount - 2, 3);
        
        DestroyImmediate(point1);
        DestroyImmediate(point2);
        DestroyImmediate(point3);
    }

    public void ResetPoints()
    {
        for (int i = 0; i < transformList.Count; i++)
        {
            DestroyImmediate(transformList[i].gameObject);
        }

        transformList.Clear();
    }

}
