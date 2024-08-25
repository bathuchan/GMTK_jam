using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;

    [HideInInspector] public bool isFixed=false;
    public List<GameObject> stopedCars;

    public bool Interact(Interaction interaction)
    {
       
        return true;
    }
}
