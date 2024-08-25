using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Interaction : MonoBehaviour
{
    [SerializeField]private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius=0.5f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders=new Collider[3];
    [SerializeField] private int _numFound;
    [SerializeField] public GameObject interactionTextBox;
    private IInteractable _interactable;
    //Image asd;
    [SerializeField] public Text dsa;
    private void Start()
    {

        //interactionTextBox = GameObject.FindGameObjectWithTag("HUD").gameObject.transform.GetChild(3).gameObject;

         //asd=interactionTextBox.GetComponent<Image>();
         //dsa= interactionTextBox.transform.GetChild(0).GetComponent<Text>();

    }




    private void Update()
    {
        

        _interactionPoint = GameObject.FindGameObjectWithTag("Player").transform;

        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position,_interactionPointRadius
            ,_colliders,_interactableMask);

        if (_numFound > 0)
        {
            
            
            _interactable = _colliders[0].GetComponent<IInteractable>();

            if (_interactable != null && Input.GetKeyDown(KeyCode.F))
            {
                _interactable.Interact(this);
                
            }
            
            interactionTextBox.SetActive(true);
            dsa.text = _interactable.InteractionPrompt;
            /*asd.enabled = true;
            dsa.enabled = true;*/
        }
        else if(_numFound==0)
        {
            interactionTextBox.SetActive(false);
            /*asd.enabled = false;
            dsa.enabled = false;*/
        }


        //SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position,_interactionPointRadius);
    }

}
