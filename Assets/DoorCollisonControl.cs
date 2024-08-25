using UnityEngine;


public class DoorCollisonControl : MonoBehaviour
{
//    public float speed = 2.0f; // Platformun hareket h�z�
//    public float height = 5.0f; // Platformun ula�aca�� maksimum y�kseklik
//    public Vector3 startPos; // Platformun ba�lang�� pozisyonu

    [SerializeField] LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.FindAnyObjectByType<LevelManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
       
            // E�er oyuncu platforma binerse sahne ge�i�i yap�yoruz

            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("PlayerGirdi");
                levelManager.OnLevelSuccess();

            }
        
    }
}
