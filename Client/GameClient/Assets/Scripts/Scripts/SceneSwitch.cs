using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    //for switching scenes when a level has been completed
    public void SceneName(string scene)
    { 
        SceneManager.LoadScene(scene);
    }
}
