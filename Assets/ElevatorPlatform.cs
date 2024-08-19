using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorPlatform : MonoBehaviour
{
    public float speed = 2.0f; // Platformun hareket h�z�
    public float height = 5.0f; // Platformun ula�aca�� maksimum y�kseklik
    public Vector3 startPos; // Platformun ba�lang�� pozisyonu

    public LevelManager levelManager;

    void Start()
    {
        // Platformun ba�lang�� pozisyonunu kaydediyoruz
        startPos = transform.position;
    }

    void Update()
    {
        // Platformun yeni pozisyonunu hesapl�yoruz
        float newY = Mathf.PingPong(Time.time * speed, height);
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // E�er oyuncu platforma binerse sahne ge�i�i yap�yoruz
        if (other.CompareTag("Player"))
        {
            levelManager.OnLevelSuccess();
        }
    }
}
