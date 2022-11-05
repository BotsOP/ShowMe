using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private int amountPoints;
    private void OnEnable()
    {
        EventSystem<int>.Subscribe(EventType.SCORED_POINTS, ScoredPoints);
    }
    private void OnDestroy()
    {
        EventSystem<int>.Unsubscribe(EventType.SCORED_POINTS, ScoredPoints); 
    }

    private void ScoredPoints(int points)
    {
        amountPoints += points;
        text.text = "Score: " + amountPoints;
    }
}
