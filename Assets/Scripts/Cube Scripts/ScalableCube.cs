using UnityEngine;
using System.Collections;

public class ScalableCube : BaseCube
{
    public float scaleMultiplier = 1.2f;
    public float scaleSpeed = 1.0f; // B�y�me h�z�n� kontrol eder
    public float raycastDistance = 10f; // Raycast uzunlu�u
    public int growthCount = 2;
    public int growthCountMax = 7;
    public int growthCountMin = -3;

    Coroutine scaleCoroutine = null;

    private void Start()
    {
        transform.localScale = transform.localScale * Mathf.Pow(scaleMultiplier, growthCount + growthCountMin + 1);
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
