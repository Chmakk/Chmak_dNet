using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Chmak_dNet
{


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
        string longestNumber = "";
        string[] textMass;

        
        Dictionary<char, int> dictDigitals = new Dictionary<char, int>();
        
 
        Dictionary<string, int> dictDashWords = new Dictionary<string, int>();

        List<char> list = new List<char>(); //коллекция символов 

        // Конструктор класса из текстового файла
        // Так же вычисляет количество строк в файле
        public TextAnalysis(string way)
        {
            fileName = way;
            StreamReader sr = new StreamReader(way);

            while (!sr.EndOfStream)
            {
                linesCount++;
                fileText += sr.ReadLine() + " ";
            }
            symbolsCount = fileText.Length-1;
            CharsCount();
            UniqWordsCount();
            NumbersCount();
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
        // Используем класс HushSet для создания уникальной коллекции
        static List<char> UniqSimbols(string str)
        {
            // HUSHSET -- это коллекция, которая не содержит повторяющихся элементов,
            // элементы которой не имеют определенного порядка.
            HashSet<char> HS = new HashSet<char>(); //коллекция уникальных символов
            foreach (char ch in str)
                HS.Add(ch);
            return HS.ToList<char>();
        }
        static List<string> UniqWords(string[] text)
        {
            // HUSHSET -- это коллекция, которая не содержит повторяющихся элементов,
            // элементы которой не имеют определенного порядка.
            HashSet<string> HS = new HashSet<string>(); //коллекция уникальных символов
            foreach (string str in text)
                HS.Add(str);
            
            return HS.ToList();
        }

        // Составление словаря типа (СИМВОЛ -- КОЛИЧЕСТВО ВСТЕЧ В ТЕКСТЕ)
        // из ВСЕЙ массы определенного типа символа и УНИКАЛЬНОГО листа этого типа символов
        // (например все буквы и список всех встречавшихся в тексте букв) 
        static Dictionary<char, int> CharCount(string str, List<char> Uniq)
        {
            Dictionary<char, int> dict = new Dictionary<char, int>();

            foreach (char ch in Uniq)
                dict.Add(ch, str.Where(c => c.Equals(ch)).Count());
            dict = (from d in dict orderby d.Value descending select d).ToDictionary(p => p.Key, p => p.Value);
            return dict;
        }
        static Dictionary<string, int> StringCount(string[] str, List<string> Uniq)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (string word in Uniq)
                dict.Add(word, str.Where(w => w.Contains(word)).Count());

            
            return dict;
        }

       
        // вычисляет при помощи трех статических методов ВСЕ ИСПОЛЬЗУЕМЫЕ СИМВОЛЫ и ИХ КОЛИЧЕСТВО
        public void CharsCount()
        {
            // Создаем промежуточное хранилище для всех БУКВ и ЦИФР по отдельности
            string resLetters = string.Empty;
            string resDigitals = string.Empty;
            string resPunctuation = string.Empty;
            // Указываем какой текст мы анализируем
            string str = fileText;

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

            // Используя хранилища БУКВ и ЦИФР 
            // Из них составляем УНИКАЛЬНЫЕ КОЛЛЕКЦИИ
            List<char> uniqLetters = UniqSimbols(resLetters);
            List<char> uniqDigitals = UniqSimbols(resDigitals);
            List<char> uniqPunctuation = UniqSimbols(resPunctuation);

            // Для удобства и соответствия каждому символу числа его употреблений
            // скомпонуем всё в СЛОВАРЬ
            dictLetters = CharCount(resLetters, uniqLetters);
            dictDigitals = CharCount(resDigitals, uniqDigitals);
            dictPunctuation = CharCount(resPunctuation, uniqPunctuation);

            // Далле идет вывод полученных словарей в консоль
            // + ПОДСЧЕТ общего числа букв, цифр и знаков препинания
            //Console.WriteLine("\nВ файл входят буквы: ");
            foreach (var item in dictLetters)
            {
                //Console.WriteLine(item.Key + " - " + item.Value);
                lettersCount += item.Value;
            }
                
            //Console.WriteLine("\nВ файл входят цифры: ");
            foreach (var item in dictDigitals.OrderByDescending(key => key.Value))
            {
            //    Console.WriteLine(item.Key + " - " + item.Value);
                digitalsCount += item.Value;
            }
            //Console.WriteLine("\nВ файл входят знаки препиннания: ");
            foreach (var item in dictPunctuation.OrderByDescending(key => key.Value))
            {
            //    Console.WriteLine(item.Key + " - " + item.Value);
                punctuation += item.Value;
            }
            //Console.WriteLine("В файле всего {0} букв", lettersCount);
            //Console.WriteLine("В файле всего {0} цифр", digitalsCount);
            //Console.WriteLine("В файле всего {0} знаков препиннания", punctuation);
        }
        public void UniqWordsCount()
        {
            // Количество слов (числа не считаются)
            MatchCollection matchesWords = Regex.Matches(fileText, @"\b[A-Za-z'][^ ]*\b");
            wordsCount = matchesWords.Count;
            //Console.WriteLine("\nВ файле всего {0} слов", wordsCount);
            string[] words = new string[matchesWords.Count];
            for(int i = 0; i< matchesWords.Count; i++)
            {
                if (matchesWords[i].Value.Length > longestWord.Length)
                    longestWord = matchesWords[i].Value;
                words[i] = matchesWords[i].Value;
            }
            List<string> uniqWords = UniqWords(words);
            dictWords = StringCount(fileText.Split(' '), uniqWords);

            //Console.WriteLine("\nВ файл входят слова: ");
            //foreach (var item in dictWords.OrderByDescending(key => key.Value))
            //{
            //    Console.WriteLine(item.Key + " : " + item.Value);
            //}
            // Количество слов с дефизом
            MatchCollection matchesDashWords = Regex.Matches(fileText, @"([A-Za-z]+)([\-]{1})");
            dashwordsCount = matchesDashWords.Count;

        }

        // Нахождение ВСЕХ чисел в тексте и длинну самого длинного из них
        // Нахождение самого длинного слова
        public void NumbersCount()
        {
            MatchCollection matchesNumbers = Regex.Matches(fileText, @"[0-9]+");
            numbersCount = matchesNumbers.Count;
            // Ищем самое длинное слово            
            for (int count = 0; count < matchesNumbers.Count; count++)
            {
                if (matchesNumbers[count].Value.Length > longestNumber.Length)
                    longestNumber = matchesNumbers[count].Value;
                //Console.Write("-" + matchesNumbers[count].Value + "-");
            }
        }

        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        // возвращает количество строк в текстовом файле (в блокноте будет 1 строка если нет абзацев)

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
    //string longestWord = "";
    //int dashwordsCount = 0;
    //int punctuation = 0;
}
