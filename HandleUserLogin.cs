using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace Diary
{
    internal class HandleUserLogin
    {
        // Path to userLoginsData.json
        public static string userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string userLoginsDataPath = @$"{userPath}\source\repos\Diary\userLoginsData.json";

        public static void InterfaceEntry()
        {
            UserSet userSet = LoadFromJSON(userLoginsDataPath);

            foreach (User user in userSet.ToList())
                Console.WriteLine(user);

            AddUser(userSet);
            Login(userSet);
        }

        // Method for logging in
        public static void Login(UserSet userSet)
        {
            Console.WriteLine("Welcome to Diary!");
            Console.Write("Please enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Please enter your password: ");
            string password = Console.ReadLine();

            // Generate user object from input
            User user = new(username, password);

            // Check Credentials
            if (CheckCredentials(user, userSet))
            {
                Console.WriteLine("Login successful!");
                //Console.WriteLine("Access Token: " + userSet.GetAccessToken(user));
            }
            else
            {
                Console.WriteLine("Login failed. Incorrect username or password.");
            }
        }

        // Method for adding a user
        private static void AddUser(UserSet userSet)
        {
            Console.WriteLine("Enter a username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter a password: ");
            string password = Console.ReadLine();

            User user = new(username, password);

            if (CheckCredentials(user, userSet))
            {
                Console.WriteLine("User alredy exists");
            }
            else
            {
                userSet.Add(user);
            }
            SaveToJSON(userSet);
        }

        // Method for checking credentials
        private static bool CheckCredentials(User user, UserSet userSet)
        {
            if (userSet.Contains(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Method for saving to JSON
        public static void SaveToJSON(UserSet userSet)
        {
            UserListJson userList = new() { UserList = userSet.ToList() };
            
            JsonSerializerOptions options = new()
            {
                WriteIndented = true  // Format the JSON with indentation
            };

            string jsonString = JsonSerializer.Serialize(userList, options);
            File.WriteAllText(userLoginsDataPath, jsonString);
            Console.WriteLine("Saved");
        }

        // Method for loading from JSON
        public static UserSet LoadFromJSON(string jsonPath)
        {
            string jsonString = File.ReadAllText(jsonPath);
            UserListJson userListJson = JsonSerializer.Deserialize<UserListJson>(jsonString);

            UserSet userSet = new();

            userSet.AddFromList(userListJson.UserList);

            return userSet;
        }
    }

    // Class specifically for reading and writing JSON
    public class UserListJson
    {
        public List<User> UserList { get; set; }
    }

    // Class for storing users
    public class UserSet
    {
        private readonly HashSet<User> users = new(new UserComparer());

        public bool Contains(User user) { return users.Contains(user); }

        public void Add(User user)
        {
            users.Add(user);
            user.AccessToken ??= user.GenerateAccessToken();
        }

        public void Remove(User user) { users.Remove(user); }

        public void AddFromList(List<User> userList)
        { foreach (User user in userList) users.Add(user); }

        public List<User> ToList() { return users.ToList(); }

        public string GetAccessToken(User user)
        { return users.ToList().Find(x => x.Equals(user)).AccessToken; }
    }

    // Class for storing user data
    public class User(string username, string password)
    {
        public string Username { get; set; } = username;
        public string Password { get; set; } = password;
        public string AccessToken { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, Password);
        }

        // TODO: Understand
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            User otherUser = (User)obj;
            return Username == otherUser.Username && Password == otherUser.Password;
        }

        public string GenerateAccessToken()
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();
            string token = new(Enumerable.Repeat(CHARS, 64).Select(s => s[random.Next(s.Length)]).ToArray());
            Console.WriteLine("Access Token Generated: " + token);
            return token;
        }

        public static bool operator ==(User user1, User user2)
        { return EqualityComparer<User>.Default.Equals(user1, user2); }

        public static bool operator !=(User user1, User user2)
        { return !(user1 == user2); }

        //public override string ToString()
        //{ return "Username: " + Username + "\nPassword: " + Password + "\nAccess Token: " + AccessToken; }

    }

    // Class for comparing users
    public class UserComparer : EqualityComparer<User>
    {
        // Check if current works ('==' override from User class)
        public override bool Equals(User x, User y)
        {
            //if (x == null || y == null)
            //    return x == y;

            return x == y;

            //return x.GetHashCode() == y.GetHashCode();
        }

        public bool Equals(User x, User y, bool ignoreAccessToken)
        {
            if (x == null || y == null)
                return x == y;

            if (ignoreAccessToken)
                return x.Username == y.Username && x.Password == y.Password;
            else
                return x.Username == y.Username && x.Password == y.Password && x.AccessToken == y.AccessToken;
        }

        // Unused
        public override int GetHashCode(User obj)
        {
            return obj == null ? 0 : (obj.Username.GetHashCode() ^ obj.Password.GetHashCode());
        }

    }

}
