using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    
    public GameObject rangeText,boxTypeText,rotateText, pickupText, boxPosition;
    //[SerializeField]GameObject rangeTextKB, boxTypeTextKB, rotateTextKB, pickupTextKB, boxPositionKB;
    //[SerializeField] GameObject rangeTextGP, boxTypeTextGP, rotateTextGP, pickupTextGP, boxPositionGP;
   
    public List<GameObject> keyboardUIElements, gamepadUIElements;
    // List<TextMeshProUGUI> accordingUIElements;


    

    public void TextUpdate(bool isDragging, int _numFound, Collider[] _colliders,bool isKeyboard, bool isGamepad)
    {
        if (isGamepad) 
        {
            foreach (GameObject go in gamepadUIElements) 
            {
                go.SetActive(true);
            }
            foreach (GameObject go in keyboardUIElements)
            {
                go.SetActive(false);
            }

        }
        if (isKeyboard)
        {
            foreach (GameObject go in gamepadUIElements)
            {
                go.SetActive(false);
            }
            foreach (GameObject go in keyboardUIElements)
            {
                go.SetActive(true);
            }

        }
        //if (isKeyboard) { accordingUIElements = gamepadUIElements; }

        if (isDragging && !pickupText.activeSelf && !rotateText.activeSelf)
        {
            pickupText.SetActive(true);
            rotateText.SetActive(true);
            boxPosition.SetActive(true);
            
            rangeText.SetActive(false);
            boxTypeText.SetActive(false);
            
        }
        else if (!isDragging && pickupText.activeSelf && rotateText.activeSelf)
        {
            pickupText.gameObject.SetActive(false);
            rotateText.gameObject.SetActive(false);
            boxPosition.gameObject.SetActive(false);
            
            rangeText.SetActive(false);
            boxTypeText.SetActive(false);
        }

        if (_numFound != 0 && !isDragging)
        {

            if (!pickupText.activeSelf) pickupText.SetActive(true);
            if (rotateText.activeSelf) rotateText.SetActive(false);
            boxPosition.SetActive(false);


            pickupText.GetComponent<TextMeshProUGUI>().text = "PICK UP (  )";

            TextMeshProUGUI t = boxTypeText.GetComponent<TextMeshProUGUI>();
            rangeText.SetActive (true);
               boxTypeText.gameObject.SetActive(true);
                if (_colliders[0].transform.gameObject.TryGetComponent<ScalableCube>(out ScalableCube scalableCube))
                {
                    t.text = "CHANGE SCALE";
                    t.color = Color.yellow;
                }
                else if (_colliders[0].transform.gameObject.TryGetComponent<HeavyCube>(out HeavyCube heavyCube))
                {
                    t.text = "CHANGE WEIGHT";
                    t.color = Color.green;
                }
                else if (_colliders[0].transform.gameObject.TryGetComponent<DirectionalScalableCube>(out DirectionalScalableCube dsCube))
                {
                    t.text = "CHANGE SCALE IN:" + dsCube.GetAxisString() + System.Environment.NewLine + " (M.SCROLL/R)";
                    t.color = dsCube.ChangeTextColor();
                }
            
        }
        else if (isDragging)
        {

            if (!pickupText.activeSelf) pickupText.gameObject.SetActive(true);
            if (!rotateText.activeSelf) rotateText.gameObject.SetActive(true);
            if (!boxPosition.activeSelf) boxPosition.gameObject.SetActive(true); ;
            pickupText.GetComponent<TextMeshProUGUI>().text = "   DROP   (  )";

            rangeText.SetActive(false);
            boxTypeText.SetActive(false);








        }
        else if (_numFound == 0)
        {
            if (pickupText.activeSelf) pickupText.gameObject.SetActive(false);
            if (rotateText.activeSelf) rotateText.gameObject.SetActive(false);
            
            rangeText.SetActive(false);
            boxTypeText.SetActive(false);


        }


    }
}
