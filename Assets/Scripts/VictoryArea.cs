using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryArea : MonoBehaviour
{

    public string SceneToLoad;

    public void LoadNextScene()
    {
        if (SceneToLoad != null && SceneToLoad.Trim() != "")
        {
            SceneManager.LoadScene(SceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
