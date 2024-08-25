using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeavyCube : BaseCube, IInteractable
{
    public float weightIncrease = 5f;
    public float weightDecrease = -5f;
    public float maxWeight = 500f, minWeight = 10;
    public float currentWeight;



    List<TextMeshPro> texts = new List<TextMeshPro>();

    public string InteractionPrompt => throw new System.NotImplementedException();

    private void Start()
    {
        
        SetWeight(currentWeight);
        for (int i = 0; i < transform.childCount; i++)
        {
            TextMeshPro tmp = transform.GetChild(i).GetComponent<TextMeshPro>();
            texts.Add(tmp);
            tmp.text = GetWeight();
        }

    }
    public override void Interact()
    {
        if (float.Parse(GetWeight()) < maxWeight)
        {
            SetWeight(rb.mass + weightIncrease); // Aðýrlýðý artýr
            currentWeight = Mathf.Round(rb.mass);
            foreach (TextMeshPro text in texts)
            {

                text.text = GetWeight();
            }
        }

    }

    public override void InteractAlt()
    {
        if (float.Parse(GetWeight()) > minWeight)
        {
            SetWeight(rb.mass + weightDecrease); // Aðýrlýðý artýr
            currentWeight = Mathf.Round(rb.mass);
            foreach (TextMeshPro text in texts)
            {

                text.text = GetWeight();
            }
        }
    }

    public bool Interact(Interaction interaction)
    {
        throw new System.NotImplementedException();
    }
}