using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;

namespace Diary
{
    internal class HandleUserLogin
    {
        // Path to userLoginsData.json
        private static readonly string userLoginsDataPath = @"Data\userLoginsData.json";

        public static void InterfaceEntry()
        {
            UserSet userSet = LoadFromJSON(userLoginsDataPath);

            AddUser(userSet);

            Login(userSet);
        }

        // Method for logging in
        public static void Login(UserSet userSet)
        {
            Console.WriteLine("Welcome to Diary!");

            while (true)
            {
                Console.Write("Please enter your username: ");
                string username = Console.ReadLine();
                Console.Write("Please enter your password: ");
                string password = Console.ReadLine();

                // Use LINQ to retrieve all users with the specified username
                IEnumerable<User> usersWithSameUsername = userSet.ToList().Where(user => user.Username == username);

                foreach (var user in usersWithSameUsername)
                {
                    // Get salt
                    byte[] salt = Convert.FromBase64String(user.Salt);

                    // Convert the password to bytes
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                    // Combine the password and salt, then hash the result
                    using var sha256 = new SHA256Managed();
                    byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                    passwordBytes.CopyTo(saltedPassword, 0);
                    salt.CopyTo(saltedPassword, passwordBytes.Length);

                    // Hash the salted password
                    byte[] hashedPassword = sha256.ComputeHash(saltedPassword);

                    // Check if the hashed password matches the stored password
                    if (Convert.ToBase64String(hashedPassword) == user.PasswordHash)
                    {
                        Console.WriteLine("Login successful!");
                        
                        Program.SetUserData(user.UserID, user.Username, userSet.GetAccessToken(user));

                        return;
                    }
                }
                
                Console.WriteLine("Login failed!");
            }
        }

        // Method for adding a user
        private static void AddUser(UserSet userSet)
        {
            Console.WriteLine("Enter a username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter a password: ");
            string password = Console.ReadLine();

            userSet.CreateUser(username, password);

            SaveToJSON(userSet);
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

        public void CreateUser(string username, string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Convert the password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Combine the password and salt, then hash the result
            using var sha256 = new SHA256Managed();
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
            passwordBytes.CopyTo(saltedPassword, 0);
            salt.CopyTo(saltedPassword, passwordBytes.Length);

            byte[] hashedPassword = sha256.ComputeHash(saltedPassword);

            // Store the salt and hashed password in your User object
            User newUser = new()
            {
                UserID = Guid.NewGuid(),
                Username = username,
                PasswordHash = Convert.ToBase64String(hashedPassword),
                Salt = Convert.ToBase64String(salt),
                AccessToken = User.GenerateAccessToken()
            };

            users.Add(newUser);
        }

        public void Remove(User user) { users.Remove(user); }

        public void AddFromList(List<User> userList)
        { foreach (User user in userList) users.Add(user); }

        public List<User> ToList() { return users.ToList(); }

        public string GetAccessToken(User user)
        { return users.ToList().Find(x => x.Equals(user)).AccessToken; }
    }

    // Class for storing user data
    public class User
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public string AccessToken { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, PasswordHash);
        }

        // TODO: Understand
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            User otherUser = (User)obj;
            return Username == otherUser.Username && PasswordHash == otherUser.PasswordHash;
        }

        public static string GenerateAccessToken()
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();
            string token = new(Enumerable.Repeat(CHARS, 64).Select(s => s[random.Next(s.Length)]).ToArray());
            //Console.WriteLine("Access Token Generated: " + token);
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
        public override bool Equals(User x, User y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }

            return x.Username == y.Username && x.PasswordHash == y.PasswordHash && x.AccessToken == y.AccessToken;
        }

        public override int GetHashCode(User obj)
        {
            return obj == null ? 0 : (HashCode.Combine(obj.Username, obj.PasswordHash, obj.AccessToken));
        }

    }

}
