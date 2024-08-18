using UnityEngine;

public class BaseCube : MonoBehaviour
{
    public float baseScale;   // Ortak boyut özelliði
    //public float baseWeight = 50f; // Ortak aðýrlýk özelliði

    protected Rigidbody rb;

    protected BoxCollider boxCollider;

    protected void Awake()
    {
        baseScale = transform.localScale.x;
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        
    }

    public void SetWeight(float weight)
    {
        if (rb != null)
        {
            rb.mass = weight;
            Debug.Log($"Cube Weight set to {weight}");
        }
    }
    public string GetWeight()
    {
        if (rb != null)
        {
            return Mathf.Round(rb.mass).ToString();
            
        }
        else { Debug.Log("Cube dont have rigibody");
            return "Cube dont have rigibody";
        }
    }
    // E tuþuna basýldýðýnda çaðrýlacak etkileþim fonksiyonu
    public virtual void Interact()
    {
        Debug.Log("Interacting with Base Cube");
    }

    public virtual void InteractAlt()
    {
        Debug.Log("Interacting with f");
    }

   
}
