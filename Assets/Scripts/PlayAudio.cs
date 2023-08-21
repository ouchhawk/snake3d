using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayEat() {
        audioSource.Play();
    }
}