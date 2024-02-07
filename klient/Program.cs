using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using ZstdSharp;
using System.IO;
using System.IO.Enumeration;
using System.Data;

namespace chat_client;

class Client
{
    //private static TcpClient tcpClient = new TcpClient("127.0.0.1", 27500);
    private static TcpClient tcpClient = new TcpClient("213.64.250.75", 27500);
    private static NetworkStream stream = tcpClient.GetStream();

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the ChatApp!\n");
        MainMenu(stream);
    }

    private static void MainMenu(NetworkStream stream)
    {
        while (true)
        {
            Console.WriteLine("1. Register user");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit program");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    Console.Clear();
                    Console.WriteLine("Register a new user\n");
                    RegisterUser(stream);
                    break;
                case ConsoleKey.D2:
                    Console.Clear();
                    Console.WriteLine("Login with an existing user\n");
                    LoginUser(stream);
                    break;
                case ConsoleKey.D3:
                    tcpClient.Close();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    public static void RegisterUser(NetworkStream stream)
    {
        while (true)
        {
            Console.Write("Enter username: ");
            string? username = Console.ReadLine();

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();



            if (isValidString(username) && isValidString(password))
            {
                string? registrationData = $"REGISTER.{username},{password}";
                SendToServer(stream, registrationData);

                System.Threading.Thread.Sleep(100);

                string replyData = ReadFromServer(stream);
                Console.WriteLine($"{replyData}\n");
                MainMenu(stream);
            }
            else
            {
                Console.WriteLine("Invalid username or password. Comma \",\" and spacebar \" \" are not allowed.\nPlease try again\n");
                MainMenu(stream);
            }
        }
    }

    private static void LoginUser(NetworkStream stream)
    {
        while (true)
        {

            Console.Write("Enter username: ");
            string? username = Console.ReadLine();

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();



            if (isValidString(username) && isValidString(password))
            {

                string? loginData = $"LOGIN.{username},{password}";
                SendToServer(stream, loginData);

                System.Threading.Thread.Sleep(100);

                string replyData = ReadFromServer(stream);
                Console.WriteLine($"{replyData}\n");

                if (replyData.Contains("Welcome"))
                {
                    LoggedInMenu(stream, username);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Console.WriteLine("There is no such user in the database. Please try again.\n");
                return;
            }
        }
    }

    private static void LogoutUser(NetworkStream stream, string logoutData)
    {
        SendToServer(stream, logoutData);
        Console.Clear();
        MainMenu(stream);
    }

    private static void LoggedInMenu(NetworkStream stream, string username)
    {
        bool stopListening = false;

        Thread serverListenerThread = new Thread(() =>
        {
            while (!stopListening)
            {
                if (stream.DataAvailable)
                {
                    string reply = ReadFromServer(stream);
                    if (reply.Contains("has logged in") || reply.Contains(username)){

                        Console.WriteLine(reply);
                    } 
                }
            }
        });
        serverListenerThread.Start();

        while (true)
        {
            Console.WriteLine("1. Public chat");
            Console.WriteLine("2. Private chat");
            Console.WriteLine("3. Send a message to the server");
            Console.WriteLine("4. Logout");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    stopListening = true;
                    PublicChat(stream, username);
                    break;
                //case ConsoleKey.D2:
                //    PrivateChat(stream, username);
                //    break;
                //case ConsoleKey.D3:
                //    stopListening = true;
                //    Message(stream, username);
                //    break;
                case ConsoleKey.D4:
                    stopListening = true;
                    LogoutUser(stream, ($"LOGOUT.{username}"));
                    MainMenu(stream);
                    break;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    private static void SendToServer(NetworkStream stream, string data)
    {
        //Konverterar registreringsdatan genom ASCII-kodning och skickar denna till servern.
        byte[] messageBuffer = Encoding.ASCII.GetBytes(data);
        stream.Write(messageBuffer, 0, messageBuffer.Length);
    }

    private static string ReadFromServer(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytesRead;
        StringBuilder replyDataBuilder = new StringBuilder();
        //Läser in data från klienten så länge det finns data att läsa.
        do
        {
            bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                // Convert the incoming byte array to a string and append it to the reply data.
                string partialReplyData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                replyDataBuilder.Append(partialReplyData);

                // Clear the buffer for the next iteration.
                Array.Clear(buffer, 0, buffer.Length);
            }
        } while (stream.DataAvailable);

        // Display the complete server's reply.
        return replyDataBuilder.ToString();
    }

    private static void PublicChat(NetworkStream stream, string username)
    {
        bool stopListening = false;
        string message = "";

        Console.WriteLine("Welcome to the Public Chat!\n");
        Console.WriteLine("Say something: ");

        Thread serverListenerThread = new Thread(() =>
        {
            while (!stopListening)
            {
                if (stream.DataAvailable)
                {
                    string reply = ReadFromServer(stream);
                    Console.WriteLine(reply);
                }
            }
        });
        serverListenerThread.Start();

        while (true)
        {
            
            message = Console.ReadLine();
            if (message == "exit")
            {
                stopListening = true;
                LoggedInMenu(stream, username);
            }

            string messageData = ($"PUBLIC_MESSAGE.{username},{message}");
            SendToServer(stream, messageData);
        }

        //Console.Write("Type message: ");
        //string? message = Console.ReadLine();
        //string messageData = ($"PUBLIC_MESSAGE.{username},{message}");
        //SendToServer(stream, messageData);


        //System.Threading.Thread.Sleep(100);

        //string reply = ReadFromServer(stream);
        //Console.WriteLine(reply);

        //LoggedInMenu(stream, username);
    }

    private static bool isValidString(string str)
    {
        return !string.IsNullOrWhiteSpace(str) && !str.Contains(" ") && !str.Contains(",");
    }
}

//private static void PrivateChat(NetworkStream stream, string username)
//{
//    while (true)
//    {

//        Console.WriteLine("Welcome to the private chat!");
//        Console.WriteLine("Press q to leave the private chat.");
//        string option = Console.ReadLine()!;
//        if (option == "q" || option == "Q")
//        {
//            break;
//        }

//        Console.WriteLine("\nPrivate Menu:");
//        Console.WriteLine("1. Send Message");
//        Console.WriteLine("2. Back to Main Menu");

//        ConsoleKeyInfo key = Console.ReadKey();

//        switch (key.Key)
//        {
//            case ConsoleKey.D1:
//                Message(stream, username);
//                break;

//            case ConsoleKey.D2:
//                return; // Return to the login menu

//            default:
//                Console.WriteLine("\nInvalid choice. Try again.");
//                break;
//        }

//    }
//}

//private static void PublicChat(NetworkStream stream, string username)
//{
//    while (true)
//    {

//        Console.WriteLine("Welcome to the public chat!");
//        Console.WriteLine("Press q to leave the public chat.");
//        string option = Console.ReadLine()!;
//        if (option == "q" || option == "Q")
//        {
//            break;
//        }

//        Console.WriteLine("\nPrivate Menu:");
//        Console.WriteLine("1. Send Message");
//        Console.WriteLine("2. Back to Main Menu");

//        ConsoleKeyInfo key = Console.ReadKey();

//        switch (key.Key)
//        {
//            case ConsoleKey.D1:
//                Message(stream, username);
//                break;

//            case ConsoleKey.D2:
//                return; // Return to the login menu

//            default:
//                Console.WriteLine("\nInvalid choice. Try again.");
//                break;
//        }
//    }
//}