using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour,IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    public bool Interact(Interaction interaction)
    {
        Debug.Log(message: "Opening Chest!");
        return true;
    }
}
