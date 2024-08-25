using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;


public class DirectionalScalableCube : BaseCube, IInteractable
{
    public float scaleMultiplier = 1.2f; // Büyüme çarpaný
    public float scaleDuration = 1.0f; // Büyüme hýzý
 
    public int growthCountX = 2, growthCountY = 2, growthCountZ = 2;
    [SerializeField]bool scaleWithGrowthCounts;
    public int growthCountMaxX = 7, growthCountMaxY = 7, growthCountMaxZ = 7;
    public int growthCountMinX = -3, growthCountMinY = -3, growthCountMinZ = -3;

    public int selectedAxis = 0; // 0: X, 1: Y, 2: Z
    //private bool isScaling = false; // Coroutine'in birden fazla kez baþlamasýný engellemek için

    Coroutine scaleCoroutine=null;

    public string InteractionPrompt => throw new System.NotImplementedException();

    private void Start()
    {
        if (scaleWithGrowthCounts) 
        {
            transform.localScale = new Vector3(transform.localScale.x * Mathf.Pow(scaleMultiplier, growthCountX + growthCountMinX + 1),
            transform.localScale.y * Mathf.Pow(scaleMultiplier, growthCountY + growthCountMinY + 1),
            transform.localScale.z * Mathf.Pow(scaleMultiplier, growthCountZ + growthCountMinZ + 1));
        }
        

        /*growthCountX = (int)Mathf.Round(transform.localScale.x);
        growthCountY = (int)Mathf.Round(transform.localScale.y);
        growthCountZ = (int)Mathf.Round(transform.localScale.z);*/
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
        


        // Küpün büyümesini test et
        if (growthCount < growthCountMax)
        {


            if (scaleCoroutine==null) // Coroutine'in tekrar baþlamasýný önle
            {
                
                scaleCoroutine= StartCoroutine(ScaleOverTime(transform.localScale, scaleMultiplier, scaleDuration, true));
                growthCount++;
                switch (selectedAxis)
                {
                    case 0:

                        growthCountX = growthCount;

                        break;
                    case 1: // Y ekseni

                        growthCountY = growthCount;

                        break;
                    case 2: // Z ekseni


                        growthCountZ = growthCount;

                        break;
                }
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
            if (scaleCoroutine == null)  // Coroutine'in tekrar baþlamasýný önle
            {

                scaleCoroutine = StartCoroutine(ScaleOverTime(transform.localScale, scaleMultiplier, scaleDuration, false));
                growthCount--;
                switch (selectedAxis)
                {
                    case 0:

                        growthCountX = growthCount;

                        break;
                    case 1: // Y ekseni

                        growthCountY = growthCount;

                        break;
                    case 2: // Z ekseni


                        growthCountZ = growthCount;

                        break;
                }
            }
        }
    }
    public void ChangeAxis(bool increase)
    {
        selectedAxis= selectedAxis+(increase ? +1: -1);
        

        if (selectedAxis == 3) { selectedAxis = 0; }
        else if (selectedAxis == -1) { selectedAxis = 2; }
        
        
    }

    public string GetAxisString() 
    {
        string axis = null;
        switch (selectedAxis)
        {
            case 0: // X ekseni
                axis = "X";
                break;

            case 1: // Y ekseni
                axis = "Y";
                break;

            case 2: // Z ekseni
                axis = "Z";
                break;

        }
        return axis;
    }
    public Color ChangeTextColor()
    {
        Color color = Color.white;
        switch (selectedAxis)
        {
            case 0: // X ekseni
                color = Color.red;
                break;

            case 1: // Y ekseni
                color = Color.green;
                break;

            case 2: // Z ekseni
                color = Color.blue;
                break;

        }
        return color;
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





        //transform.localScale = endScale; // Son ölçek deðerine ayarla
        //isScaling = false;
        scaleCoroutine = null;
        yield return null;
    }

    public bool Interact(Interaction interaction)
    {
        throw new System.NotImplementedException();
    }
}
