using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary
{
    internal static class HandleUserLogin
    {
        public static void Login()
        {
            Console.WriteLine("Welcome to Diary!");
            Console.WriteLine("Please enter your username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter your password: ");
            string password = Console.ReadLine();
        }
    }
}
