using System;
using System.Net.Sockets;
using System.Text;
namespace chat_client;

class ChatMessages
{
    static void Main(string[] args)
    {



        bool isLoggedIn = false;
        bool isInMessageMode = false;


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
                    RegisterUser();
                    break;


                case ConsoleKey.L:
                    Console.WriteLine("\nAnge ditt användarnamn och lösenord för inloggning:");
                    LoginUser();
                    isLoggedIn = true;
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
    private static void RegisterUser()
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
        string registrationData = $"{regUsername},{regPassword}";

        //Socket
        //Skapar en TcpClient och anger ip och port för att ansluta till server.
        //Här får vi ange en publik ip senare om vi alla ska kunna ansluta.
        TcpClient regTcpClient = new TcpClient("127.0.0.1", 27500);

        //Hämtar nätverksström från TcpClient för att kunna kommunicera med servern.
        NetworkStream regStream = regTcpClient.GetStream();

        //Konverterar registreringsdatan genom ASCII-kodning.
        byte[] regBuffer = Encoding.ASCII.GetBytes(registrationData);

        //Skickar registreringsdata till servern genom nätverksströmmen
        regStream.Write(regBuffer, 0, regBuffer.Length);

        Console.WriteLine("Registreringen är klar.");
    }

    private static void LoginUser()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        string loginData = $"{username},{password}";

        TcpClient regTcpClient = new TcpClient("127.0.0.1", 27500);

        //Hämtar nätverksström från TcpClient för att kunna kommunicera med servern.
        NetworkStream regStream = regTcpClient.GetStream();

        //Konverterar registreringsdatan genom ASCII-kodning.
        byte[] regBuffer = Encoding.ASCII.GetBytes(loginData);

        //Skickar registreringsdata till servern genom nätverksströmmen
        regStream.Write(regBuffer, 0, regBuffer.Length);

    }
}

}
/*
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

