using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Diary
{
    // Documentation: https://learn.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-u-sql-get-started
    internal class DatabaseIO
    {
        public SQLiteConnection Conn { get; private set; }

        public DatabaseIO()
        {
            // Create the database if it doesn't exist
            if (!File.Exists("database.db"))
            {
                SQLiteConnection.CreateFile("database.db");
            }
            // Connect to database.db
            Conn = new("Data Source=database.db;Version=3;");

        }

        public void CreateEntry(string title, string content)
        {
            // Create a new entry
            SQLiteCommand command = new("INSERT INTO entries (title, content) VALUES (@title, @content)", Conn);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@content", content);
            command.ExecuteNonQuery();
        }

        public static void TestMethod()
        {
            Console.WriteLine("Test");
        }

    }
}