using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SIAairportSecurity
{
    [CreateAssetMenu(fileName = "QuestionDatabase", menuName = "ScriptableObjects/QuestionDatabase", order = 1)]
    public class QuestionDatabase : ScriptableObject
    {
        [System.Serializable]
        public class QuestionData
        {
            public int itemID; // Assuming this corresponds to the itemID in ItemDatabase
            public string questionText;
            public int correctAnswer;
            public string[] answers;
        }

        public QuestionData[] questions;
    }
}
