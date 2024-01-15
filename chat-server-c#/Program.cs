using System;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chat_server_c
{
    class Program
    {
        static void Main(string[] args)
        {
            const string clientString = "mongodb://localhost:27017";

            MongoClient dbClient = new MongoClient(clientString);

            var database = dbClient.GetDatabase("Users");
            var collection = database.GetCollection<User>("users");

            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            var user = new User
            {
                Username = username,
                Password = password
            };

            collection.InsertOne(user);

            Console.WriteLine("User is now registered!");
         
        }
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}