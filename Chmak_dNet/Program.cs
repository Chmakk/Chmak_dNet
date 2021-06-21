using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json;

namespace Chmak_dNet
{
    class Program
    {
        static void Serialize(TextAnalysis analysis ,string serializepath)
        {
            var json = JsonConvert.SerializeObject(analysis, Formatting.Indented);
            Console.WriteLine(json);
            File.WriteAllText(serializepath, json);
        }

        static void Main(string[] args)
        {
            //TextAnalysis analysis = new TextAnalysis();
            while (true)
            {
                Console.Write("Введите название файла для аназиза (разрешение .txt добавиться автоматически) : ");
                string textpath = Console.ReadLine() + ".txt";
                try
                {
                    // Создаем объект указывая ПУТЬ к файлу (в моём случае файл в корневой папке проекта)
                    TextAnalysis analysis = new TextAnalysis(textpath);
                    Console.Write("--файл проанализирован--" +
                        "\nЕсли вы хотите сохранить результат в формате JSON," +
                        " введите название (путь) файла для сохраниения" +
                        "(разрешение .txt добавиться автоматически) : ");
                    string serializepath = Console.ReadLine() + ".json";
                    try
                    {
                        Serialize(analysis, serializepath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("что-то пошло не так (возможно имя файла указано не верно)");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("что-то пошло не так (имя файла указано не верно)");
                }


            }

        }
    }
}
