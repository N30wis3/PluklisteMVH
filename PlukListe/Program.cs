using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Plukliste
{
    class PluklisteProgram
    {
        static void Main()
        {
            // Arrange
            char readKey = ' ';
            int index = 0;

            Directory.CreateDirectory("import");

            List<string> files = LoadFiles();

            if (files == null) return;

            // Act
            while (readKey != 'Q')
            {
                if (files.Count == 0)
                {
                    Console.WriteLine("No files found.");
                }
                else
                {
                    Console.WriteLine($"Plukliste {index + 1} af {files.Count}");
                    Console.WriteLine($"\nFile: {files[index]}");

                    Pluklist? plukliste = ReadPlukliste(files[index]);
                    if (plukliste != null)
                    {
                        PrintPlukListe(plukliste);
                    }

                    PrintOptions(index, files);

                    readKey = HandleUserInput(ref index, ref files);
                    Console.Clear();
                }
            }
        }

        static List<string> LoadFiles()
        {
            if (Directory.Exists("export"))
            {
                return Directory.EnumerateFiles("export").ToList();
            }
            else
            {
                Console.WriteLine("Directory \"export\" not found");
                Console.ReadLine();
                return null;
            }
        }

        static Pluklist? ReadPlukliste(string filePath)
        {
            using (FileStream file = File.OpenRead(filePath))
            {
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                return (Pluklist?)xmlSerializer.Deserialize(file);
            }
        }

        static void PrintPlukListe(Pluklist plukliste)
        {
            if (plukliste?.Lines != null)
            {
                Console.WriteLine("\n{0,-13}{1}", "Name:", plukliste.Name);
                Console.WriteLine("{0,-13}{1}", "Forsendelse:", plukliste.Forsendelse);
                Console.WriteLine("{0,-13}{1}", "Adresse:", plukliste.Adresse);

                Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                foreach (var item in plukliste.Lines)
                {
                    Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                }
            }
        }

        static void PrintOptions(int index, List<string> files)
        {
            Console.WriteLine("\n\nOptions:");
            PrintOption("Q", "uit");

            if (index >= 0)
            {
                PrintOption("A", "fslut plukseddel");
            }
            if (index > 0)
            {
                PrintOption("F", "orrige plukseddel");
            }
            if (index < files.Count - 1)
            {
                PrintOption("N", "æste plukseddel");
            }
            PrintOption("G", "enindlæs pluksedler");
        }

        static void PrintOption(string key, string description)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(key);
            Console.ResetColor();
            Console.WriteLine(description);
        }

        static char HandleUserInput(ref int index, ref List<string> files)
        {
            char readKey = Console.ReadKey().KeyChar;
            readKey = char.ToUpper(readKey);

            switch (readKey)
            {
                case 'G':
                    files = Directory.EnumerateFiles("export").ToList();
                    index = 0;
                    Console.WriteLine("Pluklister genindlæst");
                    break;
                case 'F':
                    if (index > 0) index--;
                    break;
                case 'N':
                    if (index < files.Count - 1) index++;
                    break;
                case 'A':
                    MoveFileToImport(ref files, ref index);
                    break;
            }

            return readKey;
        }

        static void MoveFileToImport(ref List<string> files, ref int index)
        {
            string filewithoutPath = Path.GetFileName(files[index]);
            File.Move(files[index], $"import\\{filewithoutPath}");
            Console.WriteLine($"Plukseddel {files[index]} afsluttet.");
            files.RemoveAt(index);

            if (index == files.Count && index > 0) index--;
        }
    }
}