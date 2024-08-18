using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class DirectionalScalableCube : BaseCube
{
    public float scaleMultiplier = 1.5f; // B�y�me �arpan�
    public float scaleSpeed = 1.0f; // B�y�me h�z�
    public float maxScale = 5f;
    public float minScale = 0.1f;// Maksimum �l�ekleme fakt�r�
    public int growthCount = 0;
    public int growthCountMax = 5;
    public int growthCountMin = -5;

    public int selectedAxis = 0; // 0: X, 1: Y, 2: Z
    //private bool isScaling = false; // Coroutine'in birden fazla kez ba�lamas�n� engellemek i�in

    Coroutine scaleCoroutine=null;
    public override void Interact()
    {
        

        // K�p�n b�y�mesini test et
        if (growthCount < growthCountMax)
        {
            if (scaleCoroutine==null) // Coroutine'in tekrar ba�lamas�n� �nle
            {
                
                scaleCoroutine= StartCoroutine(ScaleOverTime(transform.localScale, scaleMultiplier, scaleSpeed, true));
                growthCount++;
            }
        }
    }

    public override void InteractAlt()
    {
        if (growthCount >growthCountMin)
        {
            if (scaleCoroutine == null)  // Coroutine'in tekrar ba�lamas�n� �nle
            {
                
                StartCoroutine(ScaleOverTime(transform.localScale, scaleMultiplier, scaleSpeed,false));
                growthCount--;
            }
        }
    }
    public void ChangeAxis()
    {
        base.InteractAlt();

        // Ekseni de�i�tir
        selectedAxis = (selectedAxis + 1) % 3;
        Debug.Log($"B�y�me ekseni de�i�tirildi: {selectedAxis}");
    }

    //private Vector3 GetNewScale()
    //{
        
    //    Vector3 newScale = transform.localScale;
    //    switch (selectedAxis)
    //    {
    //        case 0: // X ekseni
    //            newScale.x *= scaleMultiplier;
    //            //if (newScale.x > maxScale) newScale.x = maxScale;
    //            break;
    //        case 1: // Y ekseni
    //            newScale.y *= scaleMultiplier;
    //            //if (newScale.y > maxScale) newScale.y = maxScale;
    //            break;
    //        case 2: // Z ekseni
    //            newScale.z *= scaleMultiplier;
    //            //if (newScale.z > maxScale) newScale.z = maxScale;
    //            break;
    //    }
    //    return newScale;
    //}
    //private Vector3 GetNewDownScale()
    //{
    //    Vector3 newScale = transform.localScale;
    //    ;
    //    switch (selectedAxis)
    //    {
    //        case 0: // X ekseni
    //            newScale.x /= scaleMultiplier;
    //            //if (newScale.x > maxScale) newScale.x = minScale;
    //            break;
    //        case 1: // Y ekseni
    //            newScale.y /= scaleMultiplier;
    //            //if (newScale.y > maxScale) newScale.y = minScale;
    //            break;
    //        case 2: // Z ekseni
    //            newScale.z /= scaleMultiplier;
    //            //if (newScale.z > maxScale) newScale.z = minScale;
    //            break;
    //    }
    //    return newScale;
    //}

    private IEnumerator ScaleOverTime(Vector3 startScale, float scaleMultipler, float duration,bool grow)
    {
        
        int currentAxis = selectedAxis;
        //isScaling = true;
        float elapsedTime = 0;
        switch (currentAxis)
        {
            case 0: // X ekseni
                while (elapsedTime < duration)
                {
                    transform.localScale = Vector3.Lerp(startScale, new Vector3(grow? startScale.x * scaleMultipler : startScale.x / scaleMultipler, startScale.y, startScale.z), elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                break;
            case 1: // Y ekseni
                while(elapsedTime < duration)
                {
                    transform.localScale = Vector3.Lerp(startScale, new Vector3(startScale.x , grow ? startScale.y * scaleMultipler : startScale.y / scaleMultipler, startScale.z), elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                break;
            case 2: // Z ekseni
                while(elapsedTime < duration)
                {
                    transform.localScale = Vector3.Lerp(startScale, new Vector3(startScale.x, startScale.y , grow? startScale.z * scaleMultipler: startScale.z / scaleMultipler), elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                break;
        }





        //transform.localScale = endScale; // Son �l�ek de�erine ayarla
        //isScaling = false;
        scaleCoroutine = null;
        yield return null;
    }
}
