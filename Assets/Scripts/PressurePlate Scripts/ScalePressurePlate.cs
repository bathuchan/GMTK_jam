using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePressurePlate : GeneralPlateScript
{
    public int targetGrowthCount;
    private ScalableCube scalableCube;
    [SerializeField] private bool changeSizeAutomatically = false;

    private void Start()
    {
        if (changeSizeAutomatically)
        {
            switch (targetGrowthCount)
            {
                case -3:
                    transform.localScale = new Vector3(0.5f, 1, 0.5f);
                    break;
                case -2:
                    transform.localScale = new Vector3(0.6f, 1, 0.6f);
                    break;
                case -1:
                    transform.localScale = new Vector3(0.7f, 1, 0.7f);
                    break;
                case 0:
                    transform.localScale = new Vector3(0.85f, 1, 0.85f);
                    break;
                case 1:
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
                case 2:
                    transform.localScale = new Vector3(1.2f, 1, 1.2f);
                    break;
                case 3:
                    transform.localScale = new Vector3(1.5f, 1, 1.5f);
                    break;
                case 4:
                    transform.localScale = new Vector3(1.7f, 1, 1.7f);
                    break;
                case 5:
                    transform.localScale = new Vector3(2.1f, 1, 2.1f);
                    break;
                case 6:
                    transform.localScale = new Vector3(2.5f, 1, 2.5f);
                    break;
                case 7:
                    transform.localScale = new Vector3(3f, 1, 3f);
                    break;
            } 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        scalableCube = other.GetComponent<ScalableCube>();
    }
    private void OnTriggerStay(Collider other)
    {
        if(scalableCube != null && scalableCube.growthCount == targetGrowthCount)
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
        scalableCube = null;
        animatorMesh.material = materials[0];
        foreach (MeshRenderer mr in lamp_INSIDE_Mesh)
        {
            mr.material = materials[0];
        }
        animator.SetBool("match", false);
        match = false;
    }
}
