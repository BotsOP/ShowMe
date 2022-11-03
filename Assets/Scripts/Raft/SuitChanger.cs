using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitChanger : MonoBehaviour, IInteractable
{
    private bool suit = false;
    [SerializeField] private string _displayText;
    [SerializeField] private string _displayText2;
    public string displayText()
    {
        return suit ? _displayText2 : _displayText;
    }

    public void Interact()
    {
        suit = !suit;
        EventSystem<bool>.RaiseEvent(EventType.CHANGED_SUIT, suit);
    }
}
