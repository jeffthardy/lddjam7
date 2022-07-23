using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LDDJAM7
{
    public class ScoreKeeper : MonoBehaviour
    {
        public TextMeshProUGUI PlayerScoreText;
        public TextMeshProUGUI HighScoreText;


        private int currentScore;
        private int highScore;

        // Start is called before the first frame update
        void Start()
        {
            currentScore = 0;

            
            var score = PlayerPrefs.GetInt("HighScore", -1);
            if (score > 0)
                highScore = score;
        }

        // Update is called once per frame
        void Update()
        {
            PlayerScoreText.text = currentScore.ToString();
            HighScoreText.text = highScore.ToString();

        }



        public int EndCombo(int forwardFlips, int backwardFlips, float length)
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

            comboScore *= trickCount;
            currentScore += comboScore;

            //Debug.Log("Total Trick Score: " + comboScore);

            return comboScore;
        }

        public void SaveFinalScore()
        {
            if (currentScore > highScore) 
            { 
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);

            }
        }
    }
}