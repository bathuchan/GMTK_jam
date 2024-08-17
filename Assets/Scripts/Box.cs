using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Box : MonoBehaviour
{
    DragAndDrop dragAndDrop;

    private void Start()
    {

        dragAndDrop = GameObject.FindAnyObjectByType<DragAndDrop>(); 
    }

    IEnumerator Check(Collision collision) 
    {

        

        while (dragAndDrop != null && dragAndDrop.isDragging) 
        {
            Debug.Log("checking collision");
            if (Physics.Raycast(this.transform.position, collision.transform.position,  this.transform.localScale.y / 2 , dragAndDrop.stopDragLayer))
            {
                dragAndDrop.StopDrag();
            }
            yield return new WaitForSeconds(0.05f);
            yield return null;
        }
        
            
        
        chechCoroutine=null;
        yield return null;
    }

    Coroutine chechCoroutine;
    private void OnCollisionStay(Collision collision)
    {
        
        if (collision.gameObject.tag=="Player"&&dragAndDrop.isDragging) 
        {
            GameObject player = collision.gameObject;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity=Vector3.zero;
        }

        
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && dragAndDrop.isDragging)
        {
            GameObject player = collision.gameObject;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
        }

    }


}
