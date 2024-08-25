using UnityEngine;
using System.Collections;

public class ScalableCube : BaseCube, IInteractable
{
    public float scaleMultiplier = 1.2f;
    public float duration = 1.0f; // Büyüme hýzýný kontrol eder
    
    public int growthCount = 2;
    [SerializeField] bool scaleWithGrowthCounts;
    public int growthCountMax = 7;
    public int growthCountMin = -3;

    Coroutine scaleCoroutine = null;

    public string InteractionPrompt => throw new System.NotImplementedException();

    private void Start()
    {
        if(scaleWithGrowthCounts) transform.localScale = transform.localScale * Mathf.Pow(scaleMultiplier, growthCount + growthCountMin + 1);


    }
    public override void Interact()
    {
        //base.Interact();

        // Küpün büyümesini test et
        if(growthCount<growthCountMax && scaleCoroutine==null) {

            scaleCoroutine =StartCoroutine(ScaleOverTime(transform.localScale, transform.localScale * scaleMultiplier, duration));
            growthCount++;
        }
        
    }

    public override void InteractAlt()
    {
        //base.Interact();

        // Küpün büyümesini test et
        if ( growthCount > growthCountMin && scaleCoroutine == null)
        {

            scaleCoroutine = StartCoroutine(ScaleOverTime(transform.localScale, transform.localScale / scaleMultiplier, duration));
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
        transform.localScale = endScale; // Son ölçek deðerine ayarla
    }

    public bool Interact(Interaction interaction)
    {
        throw new System.NotImplementedException();
    }
}
