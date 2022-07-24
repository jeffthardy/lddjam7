using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

namespace LDDJAM7
{
    public class ScoreKeeper : MonoBehaviour
    {
        public TextMeshProUGUI PlayerScoreText;
        public TextMeshProUGUI HighScoreText;


        public GameObject FinalScoreCanvas;
        public TextMeshProUGUI FinalScoreText;
        public TextMeshProUGUI FinalTopComboText;
        public TextMeshProUGUI FinalComboCountText;
        public TextMeshProUGUI FinalDepthBonusText;

        public TextMeshProUGUI ComboInfoText;

        public AudioClip niceClip;
        public AudioClip coolClip;
        public AudioClip awesomeClip;
        public AudioClip sickClip;

        public float minTimeForScoreboard = 2.0f;


        private int currentScore;
        private int highScore;

        private int highCombo;
        private int comboCount;
        private int depthScore;

        private bool finalScoreDisplayActive;
        private float finalScoreDisplayActiveTime;

        private AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            currentScore = 0;
            highCombo = 0;
            comboCount = 0;
            finalScoreDisplayActive = false;
            FinalScoreCanvas.SetActive(finalScoreDisplayActive);
            ComboInfoText.text = "";

            audioSource = GetComponent<AudioSource>();

            var score = PlayerPrefs.GetInt("HighScore", -1);
            if (score > 0)
                highScore = score;
        }

        // Update is called once per frame
        void Update()
        {
            PlayerScoreText.text = currentScore.ToString();
            HighScoreText.text = highScore.ToString();

            // Check for keyboard press and reload
            if(finalScoreDisplayActive)
            {

            }

        }



        public int EndCombo(int forwardFlips, int backwardFlips, float length, bool hasPuked)
        {
            var comboScore = 0;
            var trickCount = 0;
            if (forwardFlips > 0)
            {
                trickCount++;
                comboScore += 100 * forwardFlips;
                //Debug.Log("front flip score:  " + 100 * forwardFlips);
            }

            if (backwardFlips > 0)
            {
                trickCount++;
                comboScore += 100 * backwardFlips;
                //Debug.Log("back flip score: " + 100 * backwardFlips);
            }

            if (length > 10.0f)
            {
                trickCount++;
                comboScore += 1000;
                //Debug.Log("Long trip bonus: " + 1000);
            }

            if (hasPuked)
                trickCount++;

            comboScore *= trickCount;
            currentScore += comboScore;

            //Debug.Log("Total Trick Score: " + comboScore);

            if (comboScore > highCombo)
                highCombo = comboScore;

            comboCount++;

            if (hasPuked)
            {
                ComboInfoText.text = "SICK Combo!";
                audioSource.PlayOneShot(sickClip);
            }
            else
            {
                if (trickCount == 1)
                {
                    ComboInfoText.text = "Nice Combo!";
                    audioSource.PlayOneShot(niceClip);
                }
                else if (trickCount == 2)
                {
                    ComboInfoText.text = "Cool Combo!";
                    audioSource.PlayOneShot(coolClip);
                }
                else if (trickCount > 2)
                {
                    ComboInfoText.text = "Awesome Combo!";
                    audioSource.PlayOneShot(awesomeClip);
                }
            }

            if (trickCount > 0)
                StartCoroutine(ClearComboText(2));


            return comboScore;
        }

        public void SaveFinalScore(int depth)
        {
            depthScore = depth * -10;
            currentScore += depthScore;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);

            }

            DisplayFinalScore(depth);
        }


        public void DisplayFinalScore(int depth)
        {
            FinalScoreText.text = currentScore.ToString();
            FinalTopComboText.text = highCombo.ToString();
            FinalComboCountText.text = comboCount.ToString();
            FinalDepthBonusText.text = depthScore.ToString();

            finalScoreDisplayActive = true;
            finalScoreDisplayActiveTime = Time.time + minTimeForScoreboard;
            FinalScoreCanvas.SetActive(finalScoreDisplayActive);

        }


        public void SpacebarPressed(InputAction.CallbackContext context)
        {
            if(finalScoreDisplayActive && (Time.time > finalScoreDisplayActiveTime))
            {
                SceneManager.LoadScene("Dive");
            }
        }
        IEnumerator ClearComboText(int secs)
        {
            yield return new WaitForSeconds(secs);
            ComboInfoText.text = "";
        }
    }
}