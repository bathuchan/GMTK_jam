using UnityEngine;
using System.Collections;

public class DirectionalScalableCube : BaseCube
{
    public float scaleMultiplier = 1.5f; // Büyüme çarpaný
    public float scaleSpeed = 1.0f; // Büyüme hýzý
    public float maxScale = 5f; // Maksimum ölçekleme faktörü
    public int growthCount = 0;
    public int growthCountMax = 5;

    private int currentAxis = 0; // 0: X, 1: Y, 2: Z
    private bool isScaling = false; // Coroutine'in birden fazla kez baþlamasýný engellemek için

    public override void Interact()
    {
        base.Interact();

        // Küpün büyümesini test et
        if (growthCount < growthCountMax)
        {
            if (!isScaling) // Coroutine'in tekrar baþlamasýný önle
            {
                Vector3 endScale = GetNewScale();
                StartCoroutine(ScaleOverTime(transform.localScale, endScale, scaleSpeed));
                growthCount++;
            }
        }
    }

    public override void InteractAlt()
    {
        base.InteractAlt();

        // Ekseni deðiþtir
        currentAxis = (currentAxis + 1) % 3;
        Debug.Log($"Büyüme ekseni deðiþtirildi: {currentAxis}");
    }

    private Vector3 GetNewScale()
    {
        Vector3 newScale = transform.localScale;
        switch (currentAxis)
        {
            case 0: // X ekseni
                newScale.x *= scaleMultiplier;
                if (newScale.x > maxScale) newScale.x = maxScale;
                break;
            case 1: // Y ekseni
                newScale.y *= scaleMultiplier;
                if (newScale.y > maxScale) newScale.y = maxScale;
                break;
            case 2: // Z ekseni
                newScale.z *= scaleMultiplier;
                if (newScale.z > maxScale) newScale.z = maxScale;
                break;
        }
        return newScale;
    }

    private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        isScaling = true;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale; // Son ölçek deðerine ayarla
        isScaling = false;
    }
}
