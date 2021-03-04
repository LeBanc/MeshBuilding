using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private static MenuController _instance; // to check on Awake if an instance is already existing because we only wanted one instance

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(_instance.gameObject);
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().name == "Menu")
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    public void LaunchInsideScene()
    {
        SceneManager.LoadScene("Inside");
    }

    public void LaunchOutsideScene()
    {
        SceneManager.LoadScene("Outside");
    }

}
