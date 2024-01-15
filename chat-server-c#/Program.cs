using System;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chat_server_c
{
    class Program
    {
        static void Main(string[] args)

        {
            //Connection string
            const string clientString = "mongodb://localhost:27017";

            //Creating a MongoDB client using the clientString to localhost
            MongoClient dbClient = new MongoClient(clientString);

            //Access the database Users in MongoDB
            var database = dbClient.GetDatabase("Users");

            //Reference the users collection on the database
            var collection = database.GetCollection<User>("users");

            //Asking the user to enter username and password using the terminal
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();

            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();

            //Creating a new User using the entered username and password
            var user = new User
            {
                Username = username,
                Password = password
            };

            //Inserts the user to the MongoDB collection 
            collection.InsertOne(user);

            Console.WriteLine("User is now registered!");
         
        }

        //User class to represent the user data
        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}