using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx {
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(EnumList.Scene sceneType) {
        string sceneName = System.Enum.GetName(typeof(EnumList.Scene), sceneType);

        SceneManager.LoadScene(sceneName);
    }
}
