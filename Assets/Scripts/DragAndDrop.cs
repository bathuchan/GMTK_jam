using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.Timeline.AnimationPlayableAsset;

public class DragAndDrop : MonoBehaviour
{
    private Camera cam;
    private Rigidbody rb;
    [HideInInspector]public bool isDragging = false;
    public float dragDistance;
    public LayerMask draggableLayer;
    public LayerMask stopDragLayer;
    public PlayerInputController playerInputController;
    private CapsuleCollider playerCollider;
    public TextMeshProUGUI UiText;
    private ChooseBox chooseBox;

    void Start()
    {
        cam = Camera.main;
        playerInputController = GameObject.FindAnyObjectByType<PlayerInputController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCollider = playerInputController.playerCollider;
        chooseBox=GetComponent<ChooseBox>();

    }
    public float dragSpeed;
    Coroutine dragCoroutine;
    GameObject draggedObject;
    public bool TryStartDrag(RaycastHit hita)
    {
        //if (chooseBox.lookingAt != null)
        //{
        //    hitt = new RaycastHit();
        //    if (chooseBox.lookingAt.layer == LayerMask.NameToLayer("Box") && chooseBox.lookingAt.TryGetComponent<Rigidbody>(out rb))
        //    {
        //        draggedObject = chooseBox.lookingAt;
        //        isDragging = true;
        //        rb.useGravity = false;
        //        //rb.constraints = RigidbodyConstraints.FreezeRotation;

        //        dragCoroutine = StartCoroutine(DragObject());

        //        return true;
        //    }
        //    return false;
        //}


        //else
        //{
        hitt = hita;
       

        if (hitt.transform.tag == "Box" && hitt.transform.gameObject.TryGetComponent<Rigidbody>(out rb))
        {

            draggedObject = hitt.transform.gameObject; ;
            isDragging = true;
            rb.useGravity = false;
            //rb.constraints = RigidbodyConstraints.FreezeRotation;

            dragCoroutine = StartCoroutine(DragObject());
            draggedObject.transform.position = hitt.point;

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

            //Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, cam.WorldToScreenPoint(rb.transform.position).z);
            //Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);


            //draggedObject.transform.position = Vector3.MoveTowards(draggedObject.transform.position, hitt.point, dragSpeed * Time.deltaTime);

            //rb.MovePosition(worldPos);
            draggedObject.transform.position = Vector3.MoveTowards(draggedObject.transform.position, cam.transform.position + cam.transform.forward * distanceToDraggedObject, dragSpeed*Time.deltaTime);
            //distanceToDraggedObject = Vector3.Distance(cam.transform.position, draggedObject.transform.position);
            distanceToDraggedObject = Mathf.Lerp(distanceToDraggedObject, dragDistance-1f, dragSpeed * Time.deltaTime);
            //hitt.point= cam.transform.position+cam.transform.forward* distanceToDraggedObject;
            //rayy =new Ray(cam.transform.position,rayy.direction * distanceToDraggedObject);
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

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, Vector3.Distance(cam.transform.position, draggedObject.transform.position) , stopDragLayer))
            {
                // If an obstacle is detected, stop dragging
                Debug.Log("BurayaGiriyor.");
                StopDrag();
            }
            //else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dragDistance, stopDragLayer))
            //{
            //    // If there's something in front of the player but not blocking the drag object, continue dragging
            //    draggedObject = hit.collider.gameObject;
            //    if (hitObject.layer == LayerMask.NameToLayer("Box"))
            //    {
            //        Debug.Log("BurayaGiriyor.!!!!");
            //        StopDrag();
            //    }
            //}
            else if (dragDistance<= distanceToDraggedObject) 
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
        if (dragCoroutine != null)
            {
                StopCoroutine(dragCoroutine);
                dragCoroutine = null;
                // Re-enable collision with player
                //rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
                rb = null;
                isDragging = false;
                playerInputController.isDragging=false;
            }

            
        
    }



}
