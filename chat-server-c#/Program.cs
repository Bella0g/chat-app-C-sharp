using System;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace chat_server_c
{
    class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "mongodb://localhost:27017";

            MongoClient dbClient = new MongoClient(connectionString);

            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }
    }
}