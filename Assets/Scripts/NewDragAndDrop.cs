using Cinemachine;

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NewDragAndDrop : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public Rigidbody playerRb;
    public Rigidbody draggedObjectRb;
    [HideInInspector] public bool isDragging = false;
    public float dragDistance;
    public LayerMask draggableLayer;
    public LayerMask playerLayer,nothingLayer;

    public PlayerInputController playerInputController;
    public GameObject draggedHolder;
    public GameObject draggedObject;
    float draggedObjMass;


    Vector3 draggedHolderStartPosition;
    SpringJoint joint;
    private void Start()
    {
        draggedHolderStartPosition=draggedHolder.transform.localPosition;
        mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        //playerRb= GetComponent<Rigidbody>();
    }
    public void TryStartDrag()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, dragDistance, draggableLayer))
        {
            if (hit.transform.gameObject != null && hit.transform.CompareTag("Draggable")&& hit.transform.TryGetComponent<Rigidbody>(out draggedObjectRb)) 
            {
                draggedObject = hit.transform.gameObject;
                joint=hit.transform.gameObject.AddComponent<SpringJoint>();
                if(joint != null&& draggedHolder.TryGetComponent<Rigidbody>(out playerRb))
                {
                    draggedObjMass=draggedObjectRb.mass;
                    draggedObjectRb.mass = 1;
                    draggedObjectRb.excludeLayers = playerLayer;
                    //draggedHolder.transform.position=hit.transform.gameObject.transform.position;
                    draggedObjectRb.useGravity=false;
                    draggedObjectRb.drag = 5;
                    draggedObjectRb.angularDrag = 3;
                    joint.autoConfigureConnectedAnchor = false;
                    joint.anchor = Vector3.zero;
                    joint.connectedBody = playerRb;
                    joint.spring = 50f;
                    joint.damper= 0.0f;
                    joint.maxDistance = 0.05f;
                    joint.breakForce = 200f;
                    isDragging = true;
                    return;
                }
            }
            
            
        }
        if (isDragging == true)
        {

            StopDrag();
        }
        else
        {
            isDragging = false;
        }
        


    }

    public void StopDrag() 
    {
        if (draggedObjectRb != null) 
        {
            ResetPosition();
            Destroy(joint);
            draggedObjectRb.excludeLayers = nothingLayer;
            draggedObjectRb.mass = draggedObjMass;
            isDragging = false;
            draggedObject =null;
            draggedObjectRb.useGravity = true;
            draggedObjectRb.drag = 0; 
            draggedObjectRb.angularDrag = 0.05f;
            draggedObjectRb = null;

        }
    }

    public void ResetPosition() 
    {
        draggedHolder.transform.localPosition = draggedHolderStartPosition;
    }

    
}
