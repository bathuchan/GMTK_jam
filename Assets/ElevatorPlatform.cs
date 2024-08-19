using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorPlatform : MonoBehaviour
{
    public float speed = 2.0f; // Platformun hareket hýzý
    public float height = 5.0f; // Platformun ulaþacaðý maksimum yükseklik
    public Vector3 startPos; // Platformun baþlangýç pozisyonu

    public LevelManager levelManager;

    void Start()
    {
        // Platformun baþlangýç pozisyonunu kaydediyoruz
        startPos = transform.position;
    }

    void Update()
    {
        // Platformun yeni pozisyonunu hesaplýyoruz
        float newY = Mathf.PingPong(Time.time * speed, height);
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eðer oyuncu platforma binerse sahne geçiþi yapýyoruz
        if (other.CompareTag("Player"))
        {
            levelManager.OnLevelSuccess();
        }
    }
}
