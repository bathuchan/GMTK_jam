using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePressurePlate : GeneralPlateScript
{
    public int targetGrowthCount;
    private ScalableCube scalableCube;

    private void OnTriggerEnter(Collider other)
    {
        scalableCube = other.GetComponent<ScalableCube>();
    }
    private void OnTriggerStay(Collider other)
    {
        if(scalableCube != null && scalableCube.growthCount == targetGrowthCount)
        {
            animatorMesh.material = materials[1]; lampMesh.material = materials[1];
            animator.SetBool("match", true);
            match = true;
        }
        else
        {
            animatorMesh.material = materials[0]; lampMesh.material = materials[0];
            animator.SetBool("match", false);
            match = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        scalableCube = null;
        animatorMesh.material = materials[0]; lampMesh.material = materials[0];
        animator.SetBool("match", false);
        match = false;
    }
}
