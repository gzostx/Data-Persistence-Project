using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class SceneControl : MonoBehaviour
{
   // private string nameText;

    
    public void ReadStringInput(string s)
    {
       MenuManager.Instance.NameText = s ;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
}
