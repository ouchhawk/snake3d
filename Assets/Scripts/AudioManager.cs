using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource FoodAte;
    public AudioSource BoxDestroyed;
    public AudioSource StageClear;
    public AudioSource Hit1;
    public AudioSource Hit2;
    public AudioSource GameOver;

    public void PlayFoodAte()
    {
        FoodAte.Play();
    }

    public void PlayBoxDestroyed()
    {
        BoxDestroyed.Play();
    }
    public void PlayStageClear()
    {
        StageClear.Play();
    }
    public void PlayGameOver()
    {
        GameOver.Play();
    }

    public void PlayHit1()
    {
        Hit1.Play();
    }
    public void PlayHit2()
    {
        Hit2.Play();
    }
}