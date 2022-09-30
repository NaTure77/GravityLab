using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviours : MonoBehaviour {

    AudioManager audioManager;
    AudioSource audio;
    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        audio = GetComponent<AudioSource>();
    }

    public void EnableMoving()
    {
        StateManager.isMoveable = true;
    }

    public void EnableFlyable()
    {
        StateManager.isFlyable = true;
    }

    public void FootStepSound()
    {
        //if (audio.isPlaying) return;
        audio.time = 0;
        audio.volume = 0.8f;
        audio.clip = audioManager.FootStep;
        audio.Play();
    }

    public void FallDownSound()
    {
        audio.time = 0;
        audio.volume = 0.4f;
        audio.clip = audioManager.FallDown;
        audio.Play();
    }
    public void RollingSound()
    {
        audio.time = 0;
        audio.volume = 0.6f;
        audio.clip = audioManager.Rolling;
        audio.Play();
    }
    public void LandingSound()
    {
        audio.time = 0;
        audio.volume = 0.6f;
        audio.clip = audioManager.Landing;
        audio.Play();
    }

    public void FlyingSound()
    {
        StartCoroutine(flySoundCtrl());
    }
    IEnumerator flySoundCtrl()
    {
        audio.time = 0;
        audio.volume = 0.6f;
        audio.clip = audioManager.Flying;
        audio.Play();
        yield return new WaitWhile(() => StateManager.isFlying);
        audio.Stop();
    }
}
