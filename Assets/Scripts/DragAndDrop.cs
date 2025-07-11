using System.Collections;
using TMPro;

using UnityEngine;


public class DragAndDrop : MonoBehaviour
{
    private Camera cam;
    public Rigidbody rb;
    [HideInInspector]public bool isDragging = false;
    public float dragDistance, rotationBreakMultiplier=1f;
    public LayerMask draggableLayer;
    public LayerMask stopDragLayer;
    public PlayerInputController playerInputController;
    private CapsuleCollider playerCollider;
    public TextMeshProUGUI UiText;
    public TextMeshProUGUI rotateText;
    private ChooseBox chooseBox;

    void Start()
    {
        cam = Camera.main;
        playerInputController = GameObject.FindAnyObjectByType<PlayerInputController>();
        
        //playerCollider = playerInputController.playerCollider;
        chooseBox=GetComponent<ChooseBox>();

    }
    public float dragSpeed;
    Coroutine dragCoroutine;
    public GameObject draggedObject;
    public bool TryStartDrag(RaycastHit hita)
    {
        //if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitt, dragAndDrop.dragDistance, dragAndDrop.draggableLayer)) { }
        hitt = hita;
       

        if ((hitt.transform.tag == "Box"|| hitt.transform.gameObject.layer == LayerMask.NameToLayer("BlueBox"))|| hitt.transform.gameObject.layer == LayerMask.NameToLayer("GreenBox")|| hitt.transform.gameObject.layer == LayerMask.NameToLayer("YellowBox"))
        {


            if (hitt.transform.gameObject.TryGetComponent<Rigidbody>(out rb)) 
            {
                draggedObject = hitt.transform.gameObject; ;
                isDragging = true;
                
                rb.useGravity = false;
                //rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.angularDrag = 5f;
                dragCoroutine = StartCoroutine(DragObject());
                draggedObject.transform.position = hitt.point;

                
            }
            return true;
        }
        else { return false; }


        //}




        //}
    }

    RaycastHit hitt;
    
    



    private IEnumerator DragObject()
    {
        
        float distanceToDraggedObject = dragDistance - 1f;
        draggedObject.transform.position = cam.transform.forward * distanceToDraggedObject;


        while (isDragging)
        {
            
            if(!UiText.IsActive()) UiText.gameObject.SetActive(true);
            
            UiText.text = "DROP (F)";

            if(!rotateText.IsActive()) { rotateText.gameObject.SetActive(true); }





            draggedObject.transform.position = Vector3.MoveTowards(draggedObject.transform.position, cam.transform.position + cam.transform.forward * distanceToDraggedObject, dragSpeed*Time.deltaTime);
            
            distanceToDraggedObject = Mathf.Lerp(distanceToDraggedObject, dragDistance-1f, dragSpeed * Time.deltaTime);
            
            if (playerInputController.onSlope)
            {
                rb.velocity = new Vector3(playerInputController._rb.velocity.x, playerInputController._rb.velocity.y, playerInputController._rb.velocity.z);

            }
            else if (playerInputController.inAir)
            {
                rb.velocity = new Vector3(playerInputController._rb.velocity.x, -rb.velocity.y, playerInputController._rb.velocity.z);
            }
            else 
            {
                rb.velocity = new Vector3(playerInputController._rb.velocity.x, -rb.velocity.y, playerInputController._rb.velocity.z);
            }

            // Check if there is something between the player and the dragged object
            Vector3 directionToDraggedObject = (rb.position - cam.transform.position).normalized;
            //distanceToDraggedObject = ;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, Vector3.Distance(cam.transform.position, draggedObject.transform.position), stopDragLayer))
            {
                // If an obstacle is detected, stop dragging
                
                StopDrag();
            }
            
            else if (dragDistance <= distanceToDraggedObject)
            {
                StopDrag();
            }
            else if (Vector3.Distance((cam.transform.position + cam.transform.forward * distanceToDraggedObject), draggedObject.transform.position)
                >= dragDistance * 0.75f * rotationBreakMultiplier) 
            {
                StopDrag();
            }

            yield return null;
        }
    }
    Coroutine moveCoroutine;
    //private IEnumerator MovePosition()
    //{
    //    Debug.Log("rb" + rb);
    //    Debug.Log("v3" + rb);
    //    while (rb.transform.position != worldPos&& isDragging)
    //    {
    //        Vector3.MoveTowards(rb.transform.position, worldPos , dragFollowSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    moveCoroutine = null;
    //    yield return null;
    //}

    public void StopDrag()
    {
        UiText.gameObject.SetActive(false);
        UiText.text = "PICK UP (F)";
        if (rotateText.IsActive()) { rotateText.gameObject.SetActive(false); }
        if (dragCoroutine != null)
            {
                StopCoroutine(dragCoroutine);
                dragCoroutine = null;
                // Re-enable collision with player
                //rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
            playerInputController.isRightClick = false;
            playerInputController.isLeftClick = false;
            rb.angularDrag = 0.05f;
            rb = null;
                isDragging = false;
                //playerInputController.isDragging=false;
            draggedObject=null;
            
            }

            
        
    }



}
