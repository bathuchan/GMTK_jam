using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{

    //[SerializeField] private Transform _interactionPoint;
    
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private float detectRange,detectRadius;

    [SerializeField] public  Collider[] _colliders = new Collider[1];
    [SerializeField] private int _numFound;
    
    public TextMeshProUGUI[] UiTexts;
    public TextMeshProUGUI rotateText,pickupText,boxPosition;



    void Update()
    {
        _numFound = Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + (transform.forward * detectRange), detectRadius, _colliders, _interactableMask);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        
    }
    public void TextUpdate(bool isDragging) 
    {
        if (isDragging && !pickupText.IsActive() && !rotateText.IsActive())
        {
            pickupText.gameObject.SetActive(true);
            rotateText.gameObject.SetActive(true);
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);

            }
        } else if (!isDragging && pickupText.IsActive() && rotateText.IsActive()) 
        {
            pickupText.gameObject.SetActive(false);
            rotateText.gameObject.SetActive(false);
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);

            }
        }
        
        if (_numFound != 0 && !isDragging)
        {

            if (!pickupText.IsActive()) pickupText.gameObject.SetActive(true);
            if (rotateText.IsActive()) rotateText.gameObject.SetActive(false);


            pickupText.text = "PICKUP(F)";
                
            
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(true);
                if (_colliders[0].transform.gameObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
                {
                    UiTexts[1].text = "CHANGE SCALE";
                    UiTexts[1].color = Color.yellow;
                }
                else if (_colliders[0].transform.gameObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
                {
                    UiTexts[1].text = "CHANGE WEIGHT";
                    UiTexts[1].color = Color.green;
                }
                else if (_colliders[0].transform.gameObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsCube))
                {
                    UiTexts[1].text = "CHANGE SCALE IN:" + dsCube.GetAxisString() + System.Environment.NewLine + " (M.SCROLL/R)";
                    UiTexts[1].color = dsCube.ChangeTextColor();
                }
            }
        }
        else if (isDragging)
        {

            if (!pickupText.IsActive()) pickupText.gameObject.SetActive(true);
            if (!rotateText.IsActive()) rotateText.gameObject.SetActive(true);
            pickupText.text = "DROP (F)";
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);

            }








        }
        else if (_numFound == 0) 
        {
            if (pickupText.IsActive()) pickupText.gameObject.SetActive(false);
            if (rotateText.IsActive()) rotateText.gameObject.SetActive(false);
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);
                
            }
            

        }
        
        
    }
}
