using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestionLoader : MonoBehaviour
{

    private List<QuestionGame.QuestionData> questions = new List<QuestionGame.QuestionData>();
    
    void Start()
    {
        var filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "qna.txt");
        var lines = System.IO.File.ReadAllLines(filePath);
        for (int i = 0; i < lines.Length; i++)
        {
            var question = RemovePreString(lines[i++]);
            if (question.Equals("")) break;
            
            var subQuestion = RemovePreString(lines[i++]);
            var answer1 = RemovePreString(lines[i++]);
            var answer2 = RemovePreString(lines[i++]);
            var answer3 = RemovePreString(lines[i++]);
            var answer4 = RemovePreString(lines[i++]);
            
            questions.Add(new QuestionGame.QuestionData
            {
                Question = question,
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
            });
        }
    }

    private string RemovePreString(string s)
    {
        s = s.Trim();
        var index = s.IndexOf(":", StringComparison.Ordinal);
        if (index < 0)
            return s;
        return s.Substring(index + 1, s.Length - index - 1);
    }

    public QuestionGame.QuestionData GetRandomQuestion()
    {
        return questions[Random.Range(0, questions.Count)];
    }
}
