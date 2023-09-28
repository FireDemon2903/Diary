using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Diary
{
    internal static class UserIO
    {
        public static void Entry(DatabaseIO database)
        {
            // Make sure the user is logged in
            if (Program.UserID == Guid.Empty)
            {
                Console.WriteLine("You must be logged in to use this feature.");
                return;
            }

            // Start mainloop
            MainLoop(database);

        }

        private static void MainLoop(DatabaseIO database)
        {
            // Call testmethod
            Commands["Testfunction"]();
        }


        delegate void Command();

        private static readonly Dictionary<string, Command> Commands = new()
        {
            {"Testfunction", () => DatabaseIO.TestMethod()}
        };


    }
}
