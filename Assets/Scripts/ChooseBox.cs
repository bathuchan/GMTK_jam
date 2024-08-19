using Cinemachine;
using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseBox : MonoBehaviour
{
    public float interactRange;
    public LayerMask hitWhat;
    public CinemachineVirtualCamera mainCamera;
    public GameObject lookingAt;
    public TextMeshProUGUI[] UiTexts;
    private DragAndDrop dragAndDrop;
    private Coroutine raycastCoroutine;

    private void Awake()
    {
        dragAndDrop = gameObject.GetComponent<DragAndDrop>();
    }

    private void Update()
    {
        if (raycastCoroutine == null && !dragAndDrop.isDragging)
        {
            raycastCoroutine = StartCoroutine(RaycastToBox());
        }

        if (lookingAt == null)
        {
            foreach (TextMeshProUGUI text in UiTexts)
            {
                if (text.IsActive())
                {
                    text.gameObject.SetActive(false);
                }
            }
        }
        else 
        {
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(true);
                if (lookingAt.transform.gameObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
                {
                    UiTexts[1].text = "CHANGE SCALE";
                    UiTexts[1].color = Color.yellow;
                }
                else if (lookingAt.transform.gameObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
                {
                    UiTexts[1].text = "CHANGE WEIGHT";
                    UiTexts[1].color = Color.green;
                }
                else if (lookingAt.transform.gameObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsCube))
                {
                    UiTexts[1].text = "CHANGE SCALE IN:" + dsCube.GetAxisString() + System.Environment.NewLine + " (M.SCROLL/R)";
                    UiTexts[1].color = dsCube.ChangeTextColor();
                }
            }
        }

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit1, dragAndDrop.dragDistance, dragAndDrop.draggableLayer) && !dragAndDrop.isDragging)
        {
            GameObject hitObject = hit1.collider.gameObject;
            if (hitObject.layer == LayerMask.NameToLayer("Box"))
            {
                dragAndDrop.UiText.gameObject.SetActive(true);
            }
            else
            {
                if (dragAndDrop.UiText.IsActive())
                    dragAndDrop.UiText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (dragAndDrop.UiText.IsActive())
                dragAndDrop.UiText.gameObject.SetActive(false);
        }
    }

    private IEnumerator RaycastToBox()
    {
        while (true)
        {
            if (dragAndDrop.isDragging)
            {
                foreach (TextMeshProUGUI text in UiTexts)
                {
                    if (text.IsActive())
                    {
                        text.gameObject.SetActive(false);
                    }
                }
                yield return null;
            }
            else if (lookingAt == null)
            {
                foreach (TextMeshProUGUI text in UiTexts)
                {
                    if (text.IsActive())
                    {
                        text.gameObject.SetActive(false);
                    }
                }
            }


            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, interactRange, hitWhat))
            {
                if (hit.transform.CompareTag("Box") && hit.transform.gameObject != lookingAt)
                {
                    if (hit.transform.TryGetComponent<Animator>(out Animator anim))
                    {
                        anim.SetTrigger("LookingAt");
                        foreach (TextMeshProUGUI text in UiTexts)
                        {
                            text.gameObject.SetActive(true);
                            if (hit.transform.gameObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
                            {
                                UiTexts[1].text = "CHANGE SCALE";
                                UiTexts[1].color = Color.yellow;
                            }
                            else if (hit.transform.gameObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
                            {
                                UiTexts[1].text = "CHANGE WEIGHT";
                                UiTexts[1].color = Color.green;
                            }
                            else if (hit.transform.gameObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsCube))
                            {
                                UiTexts[1].text = "CHANGE SCALE IN:" + dsCube.GetAxisString() + System.Environment.NewLine + " (M.SCROLL/R)";
                                UiTexts[1].color = dsCube.ChangeTextColor();
                            }
                        }
                    }

                    lookingAt = hit.transform.gameObject;
                    yield return lookingAt;

                    // Restart the coroutine
                    StopCoroutine(raycastCoroutine);
                    raycastCoroutine = null;
                }
                else 
                {
                    
                    //lookingAt=null;
                    yield return lookingAt;
                }

            }

            
            lookingAt = null;
            yield return lookingAt;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(mainCamera.transform.position, mainCamera.transform.forward * interactRange);
    }
}
