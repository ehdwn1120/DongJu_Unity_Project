using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerScripts : MonoBehaviour
{
    public bool isCurrect = false;
    public QuizManager quizManager;

    public void Answer()
    {
        Debug.Log("Answer");

        if (isCurrect)
        {
            Debug.Log("Correct Answer;");
            quizManager.correct();
        }
        else
        {
            Debug.Log("Wrong Answer");
            quizManager.Wrong();
        }
    }
}
