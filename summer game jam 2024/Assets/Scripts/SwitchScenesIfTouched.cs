using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenesIfTouched : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FMODbanks.Instance.OnSceneSwitch();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Destroy(gameObject);
        }
    }
}
