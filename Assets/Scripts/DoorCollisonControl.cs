using UnityEngine;


public class DoorCollisonControl : MonoBehaviour
{
//    public float speed = 2.0f; // Platformun hareket hýzý
//    public float height = 5.0f; // Platformun ulaþacaðý maksimum yükseklik
//    public Vector3 startPos; // Platformun baþlangýç pozisyonu

    [SerializeField] LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.FindAnyObjectByType<LevelManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       
            // Eðer oyuncu platforma binerse sahne geçiþi yapýyoruz

            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("PlayerGirdi");
                levelManager.OnLevelSuccess();

            }
        
    }
}
