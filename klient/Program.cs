using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;
using ZstdSharp;
using System.IO;

namespace chat_client;

class ChatMessages
{
    //private static TcpClient tcpClient = new TcpClient("127.0.0.1", 27500);
    private static TcpClient tcpClient = new TcpClient("213.64.250.75", 27500);
    private static NetworkStream stream = tcpClient.GetStream();
    static void Main(string[] args)
    {
        Thread listenerThread = new Thread(ListenForMessages);
        listenerThread.IsBackground = true;
        listenerThread.Start();

        MainMenu(stream);

    }

    private static void MainMenu(NetworkStream stream)
    {
        while (true)
        {
            // Användaren är inte inloggad, implementera keybindings för inloggning och registrering
            Console.WriteLine("Tryck r för att registrera användare.");
            Console.WriteLine("Tryck l för att logga in.");
            Console.WriteLine("Tryck q för att avsluta programmet.");
            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.R:
                    Console.WriteLine("\nAnge ditt användarnamn och lösenord för registrering:");
                    RegisterUser(stream);
                    break;

                case ConsoleKey.L:
                    Console.WriteLine("\nAnge ditt användarnamn och lösenord för inloggning:");
                    LoginUser(stream);
                    break;

                case ConsoleKey.Q:
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("\nOgiltig tangent. Försök igen.");
                    break;
            }
        }
    }

    public static void RegisterUser(NetworkStream stream)
    {
        string regUsername;
        string regPassword;

        do
        {
            Console.WriteLine("Enter username: "); //Asking the user to enter username using the terminal
            regUsername = Console.ReadLine()!; //Read the input for username from the terminal

            if (string.IsNullOrWhiteSpace(regUsername)) //Checking if the username string is null or whitespace
            {
                Console.WriteLine("Error: You need to enter a username."); //Error message for empty input
            }
        } while (string.IsNullOrWhiteSpace(regUsername)); //Condition for the loop to continue to execute as long as the username is null or a empty string

        do
        {
            Console.WriteLine("Enter password: "); //Asking the user to enter password using the terminal
            regPassword = Console.ReadLine()!; //Read the input for password from the terminal

            if (string.IsNullOrWhiteSpace(regPassword)) //Checking if the password string is null or whitespace
            {
                Console.WriteLine("Error: You need to enter a password."); //Error message for empty input
            }
        } while (string.IsNullOrWhiteSpace(regPassword)); //Condition for the loop to continue to execute as long as the password is null or whitespaced

        // Skapa en sträng för registreringsdata för att skicka till servern.
        string registrationData = $"REGISTER.{regUsername},{regPassword}";
        SendToServer(stream, registrationData);

        string replyData = ReadFromServer(stream);
        Console.WriteLine(replyData);
    }

    private static void LoginUser(NetworkStream stream)
    {
        Console.Write("Enter username: ");
        string? username = Console.ReadLine();

        Console.Write("Enter password: ");
        string? password = Console.ReadLine();
        Console.Clear();

        string? loginData = $"LOGIN.{username},{password}";
        SendToServer(stream, loginData);

        System.Threading.Thread.Sleep(100);

        string replyData = ReadFromServer(stream);
        Console.WriteLine($"{replyData}\n");

        if (replyData.Contains("Welcome"))
        {
            LoggedInMenu(stream, username);
        }
    }

    private static void LogoutUser(NetworkStream stream, string username)
    {
        string logoutData = ($"LOGOUT.{username}");
        SendToServer(stream, logoutData);
    }

    private static void LoggedInMenu(NetworkStream stream, string username)
    {

        while (true)
        {
            Console.WriteLine("\n1. Public chat");
            Console.WriteLine("2. Private chat");
            Console.WriteLine("3. Logout");
            Console.WriteLine("4. Send a message to the server");

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    PublicChat(stream, username);
                    break;

                case ConsoleKey.D2:
                    PrivateChat(stream, username);
                    break;

                case ConsoleKey.D3:
                    LogoutUser(stream, username);
                    SendLogoutMessage(stream, username);
                    return;

                case ConsoleKey.D4:
                    Message(stream, username); //Send message to server.
                    break;

                default:
                    Console.WriteLine("\nInvalid choice. Try again.");
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
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the public chat!");
            Console.WriteLine("Press q to leave the public chat.");
            string option = Console.ReadLine()!;
            if (option == "q" || option == "Q")
            {
                break;
            }

            Console.WriteLine("\nPrivate Menu:");
            Console.WriteLine("1. Send Message");
            Console.WriteLine("2. Back to Main Menu");

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Message(stream, username);
                    break;

                case ConsoleKey.D2:
                    return; // Return to the login menu

                default:
                    Console.WriteLine("\nInvalid choice. Try again.");
                    break;
            }
        }
    }

    private static void PrivateChat(NetworkStream stream, string username)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the private chat!");
            Console.WriteLine("Press q to leave the private chat.");
            string option = Console.ReadLine()!;
            if (option == "q" || option == "Q")
            {
                break;
            }

            Console.WriteLine("\nPrivate Menu:");
            Console.WriteLine("1. Send Message");
            Console.WriteLine("2. Back to Main Menu");

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Message(stream, username);
                    break;

                case ConsoleKey.D2:
                    return; // Return to the login menu

                default:
                    Console.WriteLine("\nInvalid choice. Try again.");
                    break;
            }

        }
    }

    private static void Message(NetworkStream stream, string username)
    {
        Console.WriteLine("Type message: ");
        string? message = Console.ReadLine();
        string messageData = ($"MESSAGE.{username},{message}");
        SendToServer(stream, messageData);
    }
    private static void SendLogoutMessage(NetworkStream stream, string username)
    {
        string message = "LOGOUT." + username;
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        stream.Write(buffer, 0, buffer.Length);
    }

    static void ListenForMessages()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                ProcessMessage(message);
            }

            Thread.Sleep(100);
        }
    }

    static void ProcessMessage(string message)
    {
        // Handle different types of messages (e.g., broadcast messages)
        string[] messageParts = message.Split('.');
        string messageType = messageParts[0];

        switch (messageType)
        {
            case "BROADCAST":
                // Process the broadcast message (e.g., display it)
                string broadcastMessage = messageParts[1];
                Console.WriteLine($"[Broadcast] {broadcastMessage}");
                break;

            // Add cases for other message types if needed

            default:
                break;
        }
    }


    //private static void ReadAndPrintMessages(NetworkStream stream, string username) //Method to read messages from the server and prints to the console
    //{
    //    do
    //    {
    //        string messages = ReadFromServer(stream);
    //        Console.WriteLine($"{messages}\n");

    //        Console.WriteLine("Press 'M' to go back to the menu...");
    //        ConsoleKeyInfo key = Console.ReadKey();

    //        if (key.Key == ConsoleKey.M)
    //        {
    //            return; // Return to the menu
    //        }

    //    } while (true); //the loop continues as long as the recived message is not null or empty

    //}
}

