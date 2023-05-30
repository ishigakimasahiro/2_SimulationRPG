using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string sceneName;

    public global::System.String SceneName { get => sceneName; }

    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
