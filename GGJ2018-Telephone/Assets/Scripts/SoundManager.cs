using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource switchOn, switchOff;
    public AudioSource connectorOn, connectorOff;
    public AudioSource page;

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
}
