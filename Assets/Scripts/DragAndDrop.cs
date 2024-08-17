using System.Collections;
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
    void Start()
    {
        cam = Camera.main;
        playerInputController = GameObject.FindAnyObjectByType<PlayerInputController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCollider = playerInputController.playerCollider;

    }

    Coroutine dragCoroutine;

    public void TryStartDrag()
    {
        
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dragDistance, draggableLayer)&&!isDragging)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.layer == LayerMask.NameToLayer("Box"))
            {
                rb = hitObject.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    
                    isDragging = true;
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                   
                    dragCoroutine =StartCoroutine(DragObject());
                }
                
            }
        }
    }
    
    GameObject hitObject;


    private IEnumerator DragObject()
    {
        while (isDragging)
        {
            
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, cam.WorldToScreenPoint(rb.transform.position).z);
            Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);

            rb.MovePosition(worldPos);
            if (!playerInputController.inAir)
            {
                rb.velocity = new Vector3(playerInputController._rb.velocity.x, rb.velocity.y, playerInputController._rb.velocity.z);
            }

            // Check if there is something between the player and the dragged object
            Vector3 directionToDraggedObject = (rb.position - cam.transform.position).normalized;
            float distanceToDraggedObject = Vector3.Distance(cam.transform.position, rb.position);

            if (Physics.Raycast(cam.transform.position, directionToDraggedObject, out RaycastHit hit, distanceToDraggedObject, stopDragLayer))
            {
                // If an obstacle is detected, stop dragging
                
                StopDrag();
            }
            else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, dragDistance, stopDragLayer))
            {
                // If there's something in front of the player but not blocking the drag object, continue dragging
                hitObject = hit.collider.gameObject;
                if (hitObject.layer != LayerMask.NameToLayer("Box"))
                {
                   
                    StopDrag();
                }
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
        if (isDragging)
        {
            if (dragCoroutine != null)
            {
                StopCoroutine(dragCoroutine);
                dragCoroutine = null;
            }

             // Re-enable collision with player
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb = null;
            isDragging = false;
        }
    }



}
