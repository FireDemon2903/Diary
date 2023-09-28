using System;

namespace Diary
{
    internal class Program
    {
        public static string UserAccessToken { get; private set; }
        public static Guid UserID { get; private set; }
        public static string UserName { get; private set; }

        static void Main()
        {
            HandleUserLogin.InterfaceEntry();

            Console.WriteLine("User ID: " + UserID);
            Console.WriteLine("User Name: " + UserName);
            Console.WriteLine("Access Token: " + UserAccessToken);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void SetUserData(Guid userID, string userName, string accessToken)
        {
            UserID = userID;
            UserName = userName;
            UserAccessToken = accessToken;
        }

        public static void ClearUserData()
        {
            UserID = Guid.Empty;
            UserName = null;
            UserAccessToken = null;
        }

    }
}
