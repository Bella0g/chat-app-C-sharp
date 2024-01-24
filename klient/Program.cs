using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace chat_client;

class ChatMessages
{
    static void Main(string[] args)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", 27500);
        //Hämtar nätverksström från TcpClient för att kunna kommunicera med servern.
        NetworkStream stream = tcpClient.GetStream();
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

        //Socket
        //Skapar en TcpClient och anger ip och port för att ansluta till server.
        //Här får vi ange en publik ip senare om vi alla ska kunna ansluta.


        //Konverterar registreringsdatan genom ASCII-kodning.
        byte[] regBuffer = Encoding.ASCII.GetBytes(registrationData);

        //Skickar registreringsdata till servern genom nätverksströmmen
        stream.Write(regBuffer, 0, regBuffer.Length);

        string replyData = ReadServerReply(stream);

        Console.WriteLine(replyData);
    }
    private static void LoginUser(NetworkStream stream)
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        string loginData = $"LOGIN.{username},{password}";

        //Konverterar registreringsdatan genom ASCII-kodning.
        byte[] loginBuffer = Encoding.ASCII.GetBytes(loginData);

        //Skickar registreringsdata till servern genom nätverksströmmen
        stream.Write(loginBuffer, 0, loginBuffer.Length);

        string replyData = ReadServerReply(stream);

        Console.WriteLine(replyData);

        if (replyData.Contains("Welcome"))
        {
            LoggedInMenu(stream, username);
        }

    }
    private static string ReadServerReply(NetworkStream stream)
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

    private static void LoggedInMenu(NetworkStream stream, string username)
    {
        while (true)
        {
            Console.WriteLine("1. Message to server");
            Console.WriteLine("2. Private chat");
            Console.WriteLine("3. Logout");

            ConsoleKeyInfo key = Console.ReadKey();
            byte[] userChoiceBuffer;

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Type message: ");
                    string message = Console.ReadLine();
                    string messageData = ($"MESSAGE.{ username },{ message }");
                    byte[] messageBuffer = Encoding.ASCII.GetBytes(messageData);
                    stream.Write(messageBuffer, 0, messageBuffer.Length);
                    break;

                case ConsoleKey.D2:
                    userChoiceBuffer = Encoding.ASCII.GetBytes("2");
                    stream.Write(userChoiceBuffer, 0, userChoiceBuffer.Length);
                    break;

                case ConsoleKey.D3:
                    // Logout option
                    MainMenu(stream);
                    break;

                default:
                    Console.WriteLine("\nInvalid choice. Try again.");
                    break;
            }
        }
    }

}


/*
* 
* bool isLoggedIn = false;
   bool isInMessageMode = false;
 
if (isLoggedIn)
            {
                // Användaren är inloggad, implementera keybindings för chattfunktioner
                Console.WriteLine("Tryck i för att börja skriva meddelanden.");
                Console.WriteLine("Tryck Enter för att skicka meddelandet.");
                Console.WriteLine("Tryck l för att logga ut.");
 
                if (isInMessageMode)
                {
                    Console.WriteLine("Tryck Enter för att skicka meddelandet");
                }
 
                ConsoleKeyInfo key = Console.ReadKey();
 
switch (key.Key)
{
    case ConsoleKey.I:
        isInMessageMode = true;
        break;
 
    case ConsoleKey.Enter:
        if (isInMessageMode)
        {
            Console.WriteLine("\nSkriv ditt meddelande:");
            string message = Console.ReadLine()!;
            // Skicka meddelandet till servern
            isInMessageMode = false;
        }
        break;
 
    case ConsoleKey.L:
        isLoggedIn = false;
        Console.WriteLine("\nDu har loggat ut.");
        break;
 
    default:
        Console.WriteLine("\nOgiltig tangent. Försök igen.");
        break;
}
 
            }
*/