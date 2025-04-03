using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMusic : MonoBehaviour
{
    private void Start()
    {
        // ����������� ����� ��� ������� ����������� ��'����
        SetupMusic();
    }

    private void OnEnable()
    {
        // ϳ��������� �� ���� ������������ �����
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // ³��������� �� ��䳿 ������������ �����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ����������� ������� ���� ��� ����������� ���� �����
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
