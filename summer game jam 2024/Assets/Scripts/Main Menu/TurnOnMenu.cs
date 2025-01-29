using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject particles;
    public void TurnOnObject()
    {
        menu.SetActive(true);
        particles.SetActive(true);
    }

    public void DisableThis()
    {
        gameObject.SetActive(false);
    }
}
