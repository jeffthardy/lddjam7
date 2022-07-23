using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LDDJAM7
{
    public class TitleController : MonoBehaviour
    {
        public string nextScene = "Dive";
        public string previousScene = "Dive";
        public bool isTopLevel = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGame()
        {
            SceneManager.LoadScene(nextScene);
        }

        public void QuitGame()
        {
            if(isTopLevel)
                Application.Quit();
            else
                SceneManager.LoadScene(previousScene);
        }
    }
}