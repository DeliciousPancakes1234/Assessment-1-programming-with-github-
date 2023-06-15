using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InLevelUIManager : MonoBehaviour
{
    public bool isOnline = false;

    public TMP_Text centreText;

    public CanvasGroup resultGroup;

    public TMP_Text resultTitle;

    public float fadeRate;

    public void UpdateUI()
    {
        if (!isOnline)
        {
            if (LevelManager.instance.currentState == LevelManager.GameStates.Prepping)
            {
                centreText.text = "Next wave in: \n" + LevelManager.instance.timer.displayTime;
            }
            else if (LevelManager.instance.currentState == LevelManager.GameStates.InWave)
            {
                WaveManager currentWave = LevelManager.instance.waves[LevelManager.instance.currentWave];
                int EnemiesLeft = currentWave.maxNumberOverall - currentWave.killed;

                centreText.text = "survive \n" + EnemiesLeft.ToString() + "/" + currentWave.maxNumberOverall;
            }
        }
        else
        {
            if (OnlineLevelManager.instance.currentState == OnlineLevelManager.GameStates.Prepping)
            {
                centreText.text = "Next wave in: \n" + OnlineLevelManager.instance.timer.displayTime;
            }
            else if (OnlineLevelManager.instance.currentState == OnlineLevelManager.GameStates.InWave)
            {
                WaveManager currentWave = OnlineLevelManager.instance.waves[OnlineLevelManager.instance.currentWave];
                int EnemiesLeft = currentWave.maxNumberOverall - currentWave.killed;

                centreText.text = "survive \n" + EnemiesLeft.ToString() + "/" + currentWave.maxNumberOverall;
            }
        }
    }

    public void EndGameUI()
    {
        StartCoroutine(DisplayCanvas(fadeRate));
    }

    IEnumerator DisplayCanvas(float rate)
    {
        if (!isOnline)
        {
            if (LevelManager.instance.currentState == LevelManager.GameStates.Lost)
            {
                resultTitle.text = "You have died";
            }
            else if (LevelManager.instance.currentState == LevelManager.GameStates.Won)
            {
                resultTitle.text = "Level cleared!";
            }
        }
        else
        {
            if (OnlineLevelManager.instance.currentState == OnlineLevelManager.GameStates.Lost)
            {
                resultTitle.text = "You have died";
            }
            else if (OnlineLevelManager.instance.currentState == OnlineLevelManager.GameStates.Won)
            {
                resultTitle.text = "Level cleared!";
            }
        }

        while(resultGroup.alpha < 0.9)
        {
            resultGroup.alpha = Mathf.Lerp(resultGroup.alpha, 1, rate);
            yield return new WaitForEndOfFrame();
        }
        resultGroup.alpha = 1f;
        yield return null;
    }
}
