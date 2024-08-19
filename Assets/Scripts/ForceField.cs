using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private DragAndDrop dragAndDrop;
    private void Awake()
    {
        dragAndDrop = GameObject.FindAnyObjectByType<DragAndDrop>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COllison detected with:" + collision.gameObject.name);
        if (dragAndDrop != null && dragAndDrop.isDragging) 
        {
            dragAndDrop.StopDrag();
        }
    }
}
