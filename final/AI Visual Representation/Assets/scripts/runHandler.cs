using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Sorting;
using UnityEngine.UI;
using System;

public class runHandler : MonoBehaviour
{

    private Sequence mainSequence;
    private int lastScore = Int32.MinValue;

    public Toggle sequential;
    public Toggle showSteps;
    public InputField inputField;
    public InputField difficultyField;
    public Text current;
    public Text champion;
    public Text pauseResumeButton;
    public Text currentScore;
    public Text championScore;
    private bool isPaused = true;
    private int nextBaby = 0;
    // Start is called before the first frame update
    void Start()
    {
        newSequence();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            pauseResumeButton.text = "Pause";


            Sequence seq;
            int bestTempScore = int.MinValue;

            if (sequential.isOn)
            {
                // New version, incremented babies by nextBaby : int
                int a = 10;
                do
                {
                    a--;
                    seq = mainSequence.baby(nextBaby);
                    bestTempScore = Educator.getConstTrainVal(seq);
                    nextBaby++;
                } while (a > 0 && bestTempScore <= lastScore && !showSteps.isOn);
            }
            else
            {
                // Old version, random babies
                seq = mainSequence.baby();
                bestTempScore = Int32.MinValue;
                int a = 10;
                do
                {
                    a--;
                    Sequence tempseq = seq.baby();
                    int tempScore = Educator.getConstTrainVal(tempseq);
                    if (tempScore > bestTempScore || (tempScore == bestTempScore && seq.save().Length > tempseq.save().Length))
                    {
                        bestTempScore = tempScore;
                        seq = tempseq;
                    }
                } while (a > 0 && !showSteps.isOn);
            }


            current.text = Educator.getConstTrainAI(seq).traverseTreeWithUnityCharacteristics();
            currentScore.text = "Score: " + bestTempScore.ToString() + "(T" + Educator.getConstTier().ToString() + ")\nOriginal:\t" + Educator.getConstGoal() + "\nReply:\t" + Educator.getConstReply();
            if (bestTempScore > lastScore || (bestTempScore == lastScore && mainSequence.save().Length > seq.save().Length))
            {
                mainSequence = seq;
                lastScore = bestTempScore;
                championScore.text = currentScore.text;
                nextBaby = 0;
            }

            string temp = mainSequence.save();
            if (inputField.text.CompareTo(temp) != 0)
                inputField.text = temp;

            champion.text = Educator.getConstTrainAI(mainSequence).traverseTreeWithUnityCharacteristics();
        }
        else
        {
            pauseResumeButton.text = "Resume";
        }
    }

    public void pause()
    {
        isPaused = !isPaused;
    }

    public void newSequence()
    {
        mainSequence = new Sequence();
        inputField.text = mainSequence.save();
        champion.text = Educator.getConstTrainAI(mainSequence).traverseTreeWithUnityCharacteristics();
        lastScore = Educator.getConstTrainVal(mainSequence);
        championScore.text = "Score: " + lastScore.ToString();
        current.text = "";
        currentScore.text = "";
    }

    public void newArray()
    {
        Educator.getConstGoal(Int32.Parse(difficultyField.text), true);
        championScore.text = "Score: " + Educator.getConstTrainVal(mainSequence).ToString() +
            "(T" + Educator.getConstTier().ToString() + ")\nOriginal:\t" + Educator.getConstGoal() + "\nReply:\t" + Educator.getConstReply();
        lastScore = Educator.getConstTrainVal(mainSequence);
    }

    public void loadSequence()
    {
        try
        {
            Sequence tempSequence = new Sequence(inputField.text);
            AI temp = tempSequence.build();
            Educator.educate(temp, 5);
            mainSequence = tempSequence;
            champion.text = Educator.getConstTrainAI(mainSequence).traverseTreeWithUnityCharacteristics();
            lastScore = Educator.getConstTrainVal(mainSequence);
            championScore.text = "Score: " + lastScore.ToString() + "(T" + Educator.getConstTier().ToString() + ")\nOriginal:\t" + Educator.getConstGoal() + "\nReply:\t" + Educator.getConstReply();

        }
        catch {
            isPaused = true;
            inputField.text = "Error: Sequence throws an error.";
        }
    }

}
