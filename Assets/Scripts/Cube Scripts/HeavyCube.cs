using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeavyCube : BaseCube
{
    public float weightIncrease = 5f;
    public float weightDecrease = -5f;
    public float maxWeight = 500f, minWeight = 10;



    List<TextMeshPro> texts = new List<TextMeshPro>();

    private void Start()
    {

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
            foreach (TextMeshPro text in texts)
            {

                text.text = GetWeight();
            }
        }

    }

    public override void InteractAlt()
    {
        if (  float.Parse(GetWeight())> minWeight )
        {
            SetWeight(rb.mass + weightDecrease); // Aðýrlýðý artýr
            foreach (TextMeshPro text in texts)
            {

                text.text = GetWeight();
            }
        }
    }

}