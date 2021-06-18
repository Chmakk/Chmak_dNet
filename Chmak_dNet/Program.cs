using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json;

namespace Chmak_dNet
{
    class Program
    {
        static void Serialize(TextAnalysis analysis ,string filepath)
        {
            var json = JsonConvert.SerializeObject(analysis, Formatting.Indented);
            Console.WriteLine(json);
            File.WriteAllText(filepath, json);
        }

        static void Main(string[] args)
        {
            // Создаем объект указывая ПУТЬ к файлу (в моём случае файл в корневой папке проекта)
            TextAnalysis analysis = new TextAnalysis("text.txt");
            Serialize(analysis, "serialized.json");
        }
    }
}
