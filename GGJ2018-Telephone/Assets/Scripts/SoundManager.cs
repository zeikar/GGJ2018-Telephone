using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource switchOn, switchOff;
    public AudioSource connectorOn, connectorOff;
    public AudioSource page, bookOpen, bookClose;
    public AudioSource transmission;

    public AudioSource speech;
    public AudioClip[] speechClips;

    public AudioMixerSnapshot normalShot, machineShot;

    void Awake()
    {
        instance = this;
    }

    public void SwitchSound(bool onoff)
    {
        if (onoff)
        {
            switchOn.Play();
        }
        else
        {
            switchOff.Play();
        }
    }

    public void ConnectorSound(bool onoff)
    {
        if (onoff)
        {
            connectorOn.Play();
        }
        else
        {
            connectorOff.Play();
        }
    }

    public void pageSound()
    {
        page.Play();
    }

    public void bookSound(bool onoff)
    {
        if (onoff)
        {
            bookOpen.Play();
            machineShot.TransitionTo(1f);
        }
        else
        {
            bookClose.Play();
            normalShot.TransitionTo(1f);
        }
    }

    public void speechSound(int index)
    {
        speech.clip = speechClips[index];
        speech.pitch = Random.Range(0.95f, 1.05f);
        speech.Play();
    }

    public void transmissionSound()
    {
        transmission.Play();
    }
}
