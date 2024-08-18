using UnityEngine;
using System.Collections;

public class ScalableCube : BaseCube
{
    public float scaleMultiplier = 1.5f;
    public float scaleSpeed = 1.0f; // B�y�me h�z�n� kontrol eder
    public float raycastDistance = 10f; // Raycast uzunlu�u
    public int growthCount = 0;
    public int growthCountMax = 5;
    public int growthCountMin = -5;

    Coroutine scaleCoroutine = null;

    private void Start()
    {
        growthCount = (int)Mathf.Round( transform.localScale.x);
    }
    public override void Interact()
    {
        base.Interact();

        // K�p�n b�y�mesini test et
        if(growthCount<growthCountMax && scaleCoroutine==null) {

            scaleCoroutine =StartCoroutine(ScaleOverTime(transform.localScale, transform.localScale * scaleMultiplier, scaleSpeed));
            growthCount++;
        }
        
    }

    public override void InteractAlt()
    {
        //base.Interact();

        // K�p�n b�y�mesini test et
        if ( growthCount > growthCountMin && scaleCoroutine == null)
        {

            scaleCoroutine = StartCoroutine(ScaleOverTime(transform.localScale, transform.localScale / scaleMultiplier, scaleSpeed));
            growthCount--;
        }

    }

    //private bool CanGrow()
    //{
    //    // K�p�n mevcut ve b�y�t�lm�� �l�e�i
    //    Vector3 currentScale = transform.localScale;
    //    Vector3 newScale = currentScale * scaleMultiplier;

    //    // Raycast'leri kullanarak etraf�ndaki alan� kontrol et
    //    return CheckRaycasts(newScale);
    //}

    //private bool CheckRaycasts(Vector3 newScale)
    //{
    //    // Yeni �l�ek i�in b�y�me �ncesi merkez
    //    Vector3 growthCenter = transform.position;

    //    // K�p�n etraf�nda d�rt y�nde ve yukar� y�nde raycast yap
    //    Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.up };
    //    foreach (Vector3 direction in directions)
    //    {
    //        // Raycast'leri b�y�t�lm�� k�p�n etraf�nda kontrol et
    //        Ray ray = new Ray(growthCenter, direction);
    //        RaycastHit hit;


    //    }

    //    return true;
    //}

    private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        scaleCoroutine = null;
        transform.localScale = endScale; // Son �l�ek de�erine ayarla
    }
}
