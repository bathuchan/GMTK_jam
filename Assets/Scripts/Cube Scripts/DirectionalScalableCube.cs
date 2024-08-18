using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class DirectionalScalableCube : BaseCube
{
    public float scaleMultiplier = 1.5f; // B�y�me �arpan�
    public float scaleSpeed = 1.0f; // B�y�me h�z�
    public float maxScale = 5f;
    public float minScale = 0.1f;// Maksimum �l�ekleme fakt�r�
    public int growthCountX = 0, growthCountY = 0, growthCountZ = 0;
    public int growthCountMaxX = 5, growthCountMaxY = 5, growthCountMaxZ = 5;
    public int growthCountMinX = -5, growthCountMinY = -5, growthCountMinZ = -5;

    public int selectedAxis = 0; // 0: X, 1: Y, 2: Z
    //private bool isScaling = false; // Coroutine'in birden fazla kez ba�lamas�n� engellemek i�in

    Coroutine scaleCoroutine=null;
    private void Start()
    {
        growthCountX = (int)Mathf.Round(transform.localScale.x);
        growthCountY = (int)Mathf.Round(transform.localScale.y);
        growthCountZ = (int)Mathf.Round(transform.localScale.z);
    }
    public override void Interact()
    {
        int growthCountMax=0, growthCount=0;
        switch (selectedAxis)
        {
            case 0:
                growthCountMax = growthCountMaxX;
                    growthCount = growthCountX;
                
                break;
            case 1: // Y ekseni
                growthCountMax = growthCountMaxY;
                    growthCount = growthCountY;

                break;
            case 2: // Z ekseni
                growthCountMax = growthCountMaxX;
                    growthCount = growthCountZ;

                break;
        }
        


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
        int growthCountMin = 0, growthCount = 0;
        switch (selectedAxis)
        {
            case 0:
                growthCountMin = growthCountMinX;
                growthCount = growthCountX;

                break;
            case 1: // Y ekseni
                growthCountMin = growthCountMinY;
                growthCount = growthCountY;

                break;
            case 2: // Z ekseni
                growthCountMin = growthCountMinX;
                growthCount = growthCountZ;

                break;
        }

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
