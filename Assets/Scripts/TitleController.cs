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
            if (isTopLevel)
            {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
#if (UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
                Application.Quit();
#elif (UNITY_WEBGL)
                //Application.OpenURL("about:blank");
#endif
            }
            else
                SceneManager.LoadScene(previousScene);
        }
    }
}