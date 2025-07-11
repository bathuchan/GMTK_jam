
using System.Collections.Generic;
using UnityEngine;



public class DoorsScript : MonoBehaviour
{
    [SerializeField] private Animator animatorDoorL, animatorDoorR;
    [SerializeField] private List<GameObject> conditions;
    [SerializeField] private bool[] conditionsBool;
    [SerializeField] private bool doorIsOpen = false, conditionsMet;


    private void Start()
    {
        conditionsBool = new bool[conditions.Count];
    }

    private void Update()
    {
        if (conditions != null) {
            conditionsMet = true;
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].TryGetComponent<GeneralPlateScript>(out GeneralPlateScript generalPlateScript))
                {
                    conditionsBool[i] = generalPlateScript.match;
                    if (!conditionsBool[i])
                    {
                        conditionsMet = false;
                        break;
                    }
                }
            }
        }
        

        if (!doorIsOpen && conditionsMet)
        {
            doorIsOpen = true;
            animatorDoorR.SetTrigger("Open");
            animatorDoorL.SetTrigger("Open");
            
        }
        else if (doorIsOpen && !conditionsMet)
        {
            doorIsOpen = false;
            animatorDoorR.SetTrigger("Close");
            animatorDoorL.SetTrigger("Close");
        }
    }
}
