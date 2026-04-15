using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace lab1
{
    public class Calculator
    {
        public (int len, int vowl, int cons, int wrd, int sent) GetAllStats(string text)
        { 
            if (string.IsNullOrWhiteSpace(text)) return (0, 0, 0, 0, 0);

            return (
                GetLength(text),
                CountVowels(text),
                CountConsonants(text),
                CountWords(text),
                CountSentences(text)
            );
        }

        public int GetLength(string text) => text.Length;

        public int CountVowels(string text) =>
            text.ToLower().Count(c => "aeiouyаеёиоуыэюя".Contains(c));

        public int CountConsonants(string text) =>
            text.ToLower().Count(c => char.IsLetter(c) && !"aeiouyаеёиоуыэюя".Contains(c));

        public int CountWords(string text) =>
            text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

        public int CountSentences(string text) =>
            Regex.Split(text, @"[.!?]+").Where(s => !string.IsNullOrWhiteSpace(s)).Count();
    }
}