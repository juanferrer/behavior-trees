using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour
{

    public AudioClip[] playlist;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (playlist.Length > 0)
        {
            audioSource.clip = playlist[Random.Range(0, playlist.Length)];
        }
    }

    void Start()
    {
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
            playRandomMusic();
    }

    void playRandomMusic()
    {
        audioSource.clip = playlist[Random.Range(0, playlist.Length)];
        audioSource.Play();
    }
}