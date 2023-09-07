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
        private static readonly string userLoginsDataPath = @"C:\Users\Jonathan\source\repos\Diary\userLoginsData.json";

        public static void MainUser()
        {
            UserSet users = LoadFromJSON(userLoginsDataPath);

            AddUser(users);
            Login(users);
        }

        // Method for logging in
        public static void Login(UserSet users)
        {
            Console.WriteLine("Welcome to Diary!");
            Console.Write("Please enter your username: ");
            string username = Console.ReadLine();
            Console.Write("Please enter your password: ");
            string password = Console.ReadLine();

            //UserSet users = LoadFromJSON(userLoginsDataPath);

            // Check Credentials
            if (CheckCredentials(username, password, users))
            {
                Console.WriteLine("Login successful!");
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

            userSet.Add(new User(username, password));
            SaveToJSON(userSet);
        }

        // Method for checking credentials
        private static bool CheckCredentials(string username, string password, UserSet userSet)
        {
            User checkedUser = new(username, password);
            return userSet.Contains(checkedUser);
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

        public void Add(User user) { users.Add(user); }

        public void Remove(User user) { users.Remove(user); }

        public void AddFromList(List<User> userList)
        { foreach (User user in userList) users.Add(user); }

        public List<User> ToList() { return users.ToList(); }
    }

    // Class for storing user data
    public class User(string UserName, string Password)
    {
        public string Username { get; set; } = UserName;
        public string Password { get; set; } = Password;

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, Password);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            User otherUser = (User)obj;
            return Username == otherUser.Username && Password == otherUser.Password;
        }

    }

    // Class for comparing users
    public class UserComparer : EqualityComparer<User>
    {
        public override bool Equals(User x, User y)
        {
            if (x == null || y == null)
                return x == y;
            
            // Used to compare specific objects. Only returns true if the exact same object instance is passed twice
            // If both x and y are null or reference the same object, they are equal
            //if (ReferenceEquals(x, y))
            //    return true;

            return x.GetHashCode() == y.GetHashCode();
        }

        // Unused
        public override int GetHashCode(User obj)
        {
            return obj == null ? 0 : (obj.Username.GetHashCode() ^ obj.Password.GetHashCode());
        }

    }

}
