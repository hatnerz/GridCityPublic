using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMusic : MonoBehaviour
{
    private void Start()
    {
        // Викликається тільки при першому завантаженні об'єкта
        SetupMusic();
    }

    private void OnEnable()
    {
        // Підписуємося на подію завантаження сцени
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Відписуємося від події завантаження сцени
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Викликається кожного разу при завантаженні нової сцени
        SetupMusic();
    }

    private void SetupMusic()
    {
        Debug.Log("ReturnToGlobalMusic: Setting up music...");

        var globalMusicManager = FindObjectOfType<MusicManager>();
        if (globalMusicManager != null)
        {
            var globalAudioSource = globalMusicManager.GetComponent<AudioSource>();
            if (!globalAudioSource.isPlaying)
            {
                Debug.Log("Global music is resuming.");
                globalAudioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("GlobalMusicManager not found.");
        }
    }
}
