using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightPressurePlate : GeneralPlateScript
{
    public int targetWeight;
    private HeavyCube heavyCube;
    [SerializeField] private TextMeshPro text;

    private void Start()
    {
        text.text = targetWeight.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {        
        heavyCube = other.GetComponent<HeavyCube>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (heavyCube != null && heavyCube.currentWeight >= targetWeight)
        {
            animatorMesh.material = materials[1];lamp_INSIDE_Mesh.material = materials[1];
            animator.SetBool("match", true);
            match = true;
        }
        else
        {
            animatorMesh.material = materials[0];lamp_INSIDE_Mesh.material = materials[0];
            animator.SetBool("match", false);
            match = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        heavyCube = null;
        animatorMesh.material = materials[0];lamp_INSIDE_Mesh.material = materials[0];
        animator.SetBool("match", false);
        match = false;
    }
}
