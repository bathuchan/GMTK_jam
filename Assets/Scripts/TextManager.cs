using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public TextMeshProUGUI[] UiTexts;
    public TextMeshProUGUI rotateText, pickupText, boxPosition;
    private NewDragAndDrop _dragAndDrop;

    private void Start()
    {
        _dragAndDrop = GameObject.FindAnyObjectByType<NewDragAndDrop>();
    }

    public void TextUpdate(bool isDragging, int _numFound, Collider[] _colliders)
    {
        if (isDragging && !pickupText.IsActive() && !rotateText.IsActive())
        {
            pickupText.gameObject.SetActive(true);
            rotateText.gameObject.SetActive(true);
            boxPosition.gameObject.SetActive(true);
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);

            }
        }
        else if (!isDragging && pickupText.IsActive() && rotateText.IsActive())
        {
            pickupText.gameObject.SetActive(false);
            rotateText.gameObject.SetActive(false);
            boxPosition.gameObject.SetActive(false);
            foreach (TextMeshProUGUI text in UiTexts)
            {
                text.gameObject.SetActive(false);

            }
        }

        if (_numFound != 0 && !isDragging)
        {

            if (!pickupText.IsActive()) pickupText.gameObject.SetActive(true);
            if (rotateText.IsActive()) rotateText.gameObject.SetActive(false);
            boxPosition.gameObject.SetActive(false);


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
