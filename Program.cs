using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary
{
    internal class Program
    {
        //public static readonly string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string AccessToken { get; private set; }
        static void Main()
        {
            HandleUserLogin.InterfaceEntry();
            //UserIO.MainLoop();

            Console.WriteLine("Access Token: " + AccessToken);

            // TODO: Add methodcall for diary here

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void SetAccessToken(string token)
        {
            AccessToken = token;
        }
    }
}
