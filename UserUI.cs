using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Diary
{
    internal class UserIO
    {
        public static void MainLoop()
        {
            bool _isRunning = true;
            while (_isRunning)
            {
                Console.WriteLine("Welcome to Diary!");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Create a new entry");
                Console.WriteLine("2. View previous entries");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        CreateEntry();
                        break;
                    case "2":
                        ViewEntries();
                        break;
                    case "3":
                        _isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void CreateEntry()
        {
            Console.WriteLine("Enter your entry below. Press Ctrl + S to save and exit.");
            string entry = Console.ReadLine();
            Console.WriteLine("Saving entry...");
            DiaryIO.WriteEntry(entry);
            Console.WriteLine("Entry saved!");
        }

        private static void ViewEntries()
        {
            Console.WriteLine("Viewing previous entries...");
            List<string> entries = DiaryIO.ReadEntries();
            if (entries.Count == 0)
            {
                Console.WriteLine("No entries found.");
            }
            else
            {
                foreach (string entry in entries)
                {
                    Console.WriteLine(entry);
                }
            }
        }
    }
}
