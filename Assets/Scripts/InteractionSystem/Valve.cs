using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;

    public string InteractionPrompt => _prompt;
    public GameObject effectedObject;
    public bool valveIsOpen;
    private GameObject valvePref;

    private void Start()
    {
        valvePref = gameObject.transform.GetChild(0).gameObject;
        if (valveIsOpen) 
        {
            effectedObject.SetActive(valveIsOpen);
            valveIsOpen=true;
        }else
        {
            effectedObject.SetActive(valveIsOpen);
            valveIsOpen = false;
        }
    }
    public bool Interact(Interaction interaction)
    {
        if (valveIsOpen) 
        {
            StartCoroutine(RotateR(360f));
            FindObjectOfType<AudioManager>().Play("ValveSound");

        }
        else 
        {
            StartCoroutine(RotateL(360f));
            FindObjectOfType<AudioManager>().Play("ValveSound");

        }
        valveIsOpen=!valveIsOpen;
        effectedObject.SetActive(valveIsOpen);

        Debug.Log(message: "Interacted with "+this.gameObject.name+" ->"+effectedObject.name);
        return true;
    }

    IEnumerator RotateR(float degree)
    {
        float startRotation = 0;
        float endRotation = startRotation + degree;
        

        while (startRotation <= endRotation)
        {
            

            valvePref.transform.Rotate(Vector3.up,5f);
            startRotation = startRotation + 5f;
            

            yield return null;
        }
    }
    IEnumerator RotateL(float degree)
    {
        float startRotation = 0;
        float endRotation = startRotation + degree;


        while (startRotation <= endRotation)
        {


            //float yRotation = Mathf.Lerp(startRotation, endRotation, Time.deltaTime);
            ///valvePref.transform.RotateAround(valvePref.transform.position, valvePref.transform.up, Time.deltaTime*degree);
            valvePref.transform.Rotate(Vector3.up, -5f);
            startRotation = startRotation + 5f;
            /*valvePref.transform.eulerAngles = new Vector3(valvePref.transform.eulerAngles.x, yRotation,
            valvePref.transform.eulerAngles.z);*/

            yield return null;
        }
    }
}
