using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

public class ChooseBox : MonoBehaviour
{

    public float interactRange;
    public LayerMask hitWhat;
    public CinemachineVirtualCamera mainCamera;
    [HideInInspector]public GameObject lookingAt;

    private void Update()
    {
        lookingAt=RaycastToBox();
    }

    public GameObject RaycastToBox() 
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward*interactRange, out RaycastHit hit, interactRange,hitWhat)) 
        {
            if (hit.transform.tag == "Box") 
            {
                if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                {
                    anim.SetTrigger("LookingAt");
                }
                return hit.transform.gameObject;
            }
            
        }
        return null;
    }
    void OnDrawGizmos()
    {
        
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.forward* interactRange);
        
    }

}
