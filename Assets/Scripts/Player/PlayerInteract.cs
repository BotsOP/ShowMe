using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    [SerializeField] private TMP_Text interactText;
    bool displayingText;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 5f, mask)) {
            if(!displayingText)
            {
                interactText.gameObject.SetActive(true);
                interactText.text = hit.collider.gameObject.GetComponent<IInteractable>().displayText();
                displayingText = true;
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.collider.gameObject.GetComponent<IInteractable>().Interact();
            }
        }
        else
        {
            if(displayingText)
            {
                interactText.gameObject.SetActive(false);
                displayingText = false;
            }
        }
    }
}
