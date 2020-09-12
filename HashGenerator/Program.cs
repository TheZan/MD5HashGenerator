using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите путь к папке с файлами, для которых нужно сгенерировать хэш:");
            string path = Console.ReadLine();
            string endFolder = path.Substring(path.LastIndexOf("\\"), path.Length - path.LastIndexOf("\\"));

            List<string> files = DirSearch(path);

            if (files.Any())
            {

                List<FileHash> hashes = new List<FileHash>();

                Console.WriteLine("Список файлов:");

                foreach (var file in files)
                {
                    hashes.Add(new FileHash()
                    {
                        FileName = file.Substring(file.IndexOf($"{endFolder}") + endFolder.Length + 1,
                            file.Length - file.IndexOf($"{endFolder}") - endFolder.Length - 1),
                        Hash = CalculateMD5(file)
                    });

                    Console.WriteLine(file);
                }

                GenerateJson(hashes);
            }
            else
            {
                Console.WriteLine("В выбранной папке файлы не найдены. Для завершения работы нажмите любую клавишу...");
            }

            Console.ReadKey();
        }

        static List<string> DirSearch(string sDir)
        {
            List<string> files = new List<string>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return files;
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        static async void GenerateJson(List<FileHash> hashes)
        {
            try
            {
                using (FileStream fs = new FileStream("MD5Hash.json", FileMode.Create))
                {
                    await JsonSerializer.SerializeAsync(fs, hashes);
                }

                Console.WriteLine("Генерация завершена успешно. Создан файл MD5MD5Hash.json. Для завершения работы нажмите любую клавишу...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
