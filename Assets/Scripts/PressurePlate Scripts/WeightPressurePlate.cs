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
        if (heavyCube != null && heavyCube.currentWeight >= targetWeight)
        {
            animatorMesh.material = materials[1];
            foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
            {
                mr.material = materials[1];
            }
            animator.SetBool("match", true);
            match = true;
        }
        else
        {
            animatorMesh.material = materials[0];
            foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
            {
                mr.material = materials[0];
            }
            animator.SetBool("match", false);
            match = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        heavyCube = other.GetComponent<HeavyCube>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (heavyCube != null && heavyCube.currentWeight >= targetWeight)
        {
            animatorMesh.material = materials[1];
            foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
            {
                mr.material = materials[1];
            }
            animator.SetBool("match", true);
            match = true;
        }
        else
        {
            animatorMesh.material = materials[0];
            foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
            {
                mr.material = materials[0];
            }
            animator.SetBool("match", false);
            match = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        heavyCube = null;
        animatorMesh.material = materials[0];
        foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
        {
            mr.material = materials[0];
        }
        animator.SetBool("match", false);
        match = false;
    }
}
