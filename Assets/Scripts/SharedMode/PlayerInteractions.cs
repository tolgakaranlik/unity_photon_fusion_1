using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : NetworkBehaviour
{
    public Image Bubble1;
    public Image Bubble2;

    private void Start()
    {
        Quiet();
    }

    public void SayHi()
    {
        Quiet();
        Bubble1.gameObject.SetActive(true);
    }

    public void SayWhatsUp()
    {
        Quiet();
        Bubble2.gameObject.SetActive(true);
    }

    public void Quiet()
    {
        Bubble1.gameObject.SetActive(false);
        Bubble2.gameObject.SetActive(false);
    }
}
