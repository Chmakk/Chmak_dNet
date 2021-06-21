using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Chmak_dNet
{

    [Serializable]
    class TextAnalysis
    {
        [JsonProperty]
        string fileName = "";
        [JsonProperty]
        int symbolsCount = 0;
        [JsonProperty]
        int lettersCount = 0;
        [JsonProperty]
        Dictionary<char, int> dictLetters = new Dictionary<char, int>();
        [JsonProperty]
        int wordsCount = 0;
        [JsonProperty]
        Dictionary<string, int> dictWords = new Dictionary<string, int>();
        [JsonProperty]
        int linesCount = 0;
        [JsonProperty]
        int digitalsCount = 0;
        [JsonProperty]
        int numbersCount = 0;
        [JsonProperty]
        string longestWord = "";
        [JsonProperty]
        int dashwordsCount = 0;
        [JsonProperty]
        int punctuation = 0;
        [JsonProperty]
        Dictionary<char, int> dictPunctuation = new Dictionary<char, int>();


        string fileText = "";
        string linetext = "";
        
        Dictionary<char, int> dictDigitals = new Dictionary<char, int>();
        
        // Конструктор класса из текстового файла
        // Так же вычисляет количество строк в файле
        public TextAnalysis(string way)
        {
            fileName = way;
            StreamReader sr = new StreamReader(way);

            while (!sr.EndOfStream)
            {
                linesCount++;
                linetext = sr.ReadLine();
                fileText += linetext + " ";
                if (linetext.Equals(""))
                    continue;
                CharsCount();
                UniqWordsCount();
                NumbersCount();
            }
            symbolsCount = fileText.Length-1;
            //CharsTotalCount();
            //updatedict = (from d in updatedict orderby d.Value descending select d).ToDictionary(p => p.Key, p => p.Value);
        }


        public void GetInfo()
        {
            Console.WriteLine("FileName : " + fileName +
                "\nSymbolsCount : " + symbolsCount +
                "\nLettersCount : " + lettersCount);
            Console.WriteLine("В файл входят буквы: ");
            foreach (var item in dictLetters.OrderByDescending(key => key.Value))
                Console.WriteLine(item.Key + " - " + item.Value);
            Console.WriteLine("WordsCount : " + wordsCount);
            Console.WriteLine("В файл входят слова: ");
            foreach (var item in dictWords.OrderByDescending(key => key.Value))
                Console.WriteLine(item.Key + " : " + item.Value);
            Console.WriteLine("LinesCount : " + linesCount +
                "\nDigitalCount : " + digitalsCount +
                "\nNumbersCount : " + numbersCount +
                "\nLongestWord : " + longestWord +
                "\nDashWordsCount : " + dashwordsCount +
                "\nPunctuation : " + punctuation);
            Console.WriteLine("В файл входят знаки препинания: ");
            foreach (var item in dictPunctuation.OrderByDescending(key => key.Value))
                Console.WriteLine(item.Key + " : " + item.Value);
        }

        // ------------------------------------------------------------

        // Составление словаря символов (слов) 
        // С прогоном по каждой строке файла словарь тот же самый
        static Dictionary<char, int> CharCount(Dictionary<char, int> dictionary, string str)
        {
            Dictionary<char, int> updatedict = dictionary;

            foreach (char ch in str)
            {
                if (updatedict.ContainsKey(ch) == false)
                    updatedict.Add(ch, 1);
                else
                    updatedict[ch] = updatedict[ch] + 1; // +1 к счётчику слова
            }

            return updatedict;
        }
        static Dictionary<string, int> StringCount(Dictionary<string, int> dictionary, string[] str)
        {
            Dictionary<string, int> updatedict = dictionary;

            foreach (string ch in str)
            {
                if (updatedict.ContainsKey(ch) == false)
                    updatedict.Add(ch, 1);
                else
                    updatedict[ch] = updatedict[ch] + 1; // +1 к счётчику слова
            }
            //updatedict.Add(ch, str.Where(c => c.Equals(ch)).Count());

            return updatedict;
        }


        string resLetters;
        string resDigitals ;
        string resPunctuation;
        string str;
        // Метод для нахождения информации о символах в тексте (цифры, буквы и знаки препинания + их кол-во)
        public void CharsCount()
        {
            // Создаем промежуточное хранилище для ВСЕХ символов по отдельности для разных типов
            resLetters = string.Empty;
            resDigitals = string.Empty;
            resPunctuation = string.Empty;

            str = linetext;

            // Находим в тексте ВСЕ буквы, цифры и другие символы (знаки препинания) --> записываем в ранее созданное хранилище
            foreach (char ch in str)
            {
                if (char.IsLetter(ch))
                    resLetters += ch;
                else if (char.IsDigit(ch))
                    resDigitals += ch;
                else
                    resPunctuation += ch;
            }
            // паралельно нахождения считаем общее кол-во
            lettersCount += resLetters.Length;
            digitalsCount += resDigitals.Length;
            punctuation += resPunctuation.Length;
            // Для удобства и соответствия каждому символу числа его употреблений
            // скомпонуем всё в СЛОВАРЬ
            dictLetters = CharCount(dictLetters, resLetters);
            dictDigitals = CharCount(dictDigitals, resDigitals);
            dictPunctuation = CharCount(dictPunctuation, resPunctuation);
        }


        MatchCollection matchesWords;
        public void UniqWordsCount()
        {
            // Количество слов (отдельные числа не считаются)
            // Это отличие от подсчета в ВОРДЕ, так что значения могут отличаться
            matchesWords = Regex.Matches(linetext, @"\b[A-Za-z'][^ ]*\b");
            wordsCount += matchesWords.Count;
            
            // нахождение самого длинного слова
            string[] words = new string[matchesWords.Count];
            for(int i = 0; i< matchesWords.Count; i++)
            {
                if (matchesWords[i].Value.Length > longestWord.Length)
                    longestWord = matchesWords[i].Value;
                words[i] = matchesWords[i].Value;
            }

            dictWords = StringCount(dictWords, words);

            
            // Поиск слов с дефизом и подсчет их кол-ва
            MatchCollection matchesDashWords = Regex.Matches(linetext, @"([A-Za-z]+)([\-]{1})");
            dashwordsCount += matchesDashWords.Count;

        }

        // Нахождение ВСЕХ чисел в тексте и длинну самого длинного из них
        MatchCollection matchesNumbers;
        public void NumbersCount()
        {
            matchesNumbers = Regex.Matches(linetext, @"[0-9]+");
            numbersCount += matchesNumbers.Count;
        }

        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
       
        // GETеры для искомых переменных по отдельности
        public string GetFileText()
        {
            return fileText;
        }
        public string GetFileName()
        {
            return fileName;
        }
        public int GetSymbolsCount()
        {
            return symbolsCount;
        }
        public int GetLettersCount()
        {
            return lettersCount;
        }
        public Dictionary<char, int> GetDictLettersCount()
        {
            return dictLetters;
        }
        public int GetWordsCount()
        {
            return wordsCount;
        }
        public Dictionary<string, int> GetDictWordsCount()
        {
            return dictWords;
        }
        public int GetLinesCount()
        {
            return linesCount;
        }
        public int GetDigitalsCount()
        {
            return digitalsCount;
        }
        public int GetNumbersCount()
        {
            return numbersCount;
        }
        public string GetLongestWord()
        {
            return longestWord;
        }
        public int GetDashWordsCount()
        {
            return dashwordsCount;
        }
        public int GetPunctuationCount()
        {
            return punctuation;
        }
        public Dictionary<char, int> GetDictPunctuationCount()
        {
            return dictPunctuation;
        }
    }
}
