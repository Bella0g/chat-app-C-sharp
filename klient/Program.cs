namespace chat_app_c_;

class ChatMessages
{
    static void Main(string[] args)
    {
        bool isLoggedIn = false;
        bool isInMessageMode = false;

        while (true)
        {
            if (isLoggedIn)
            {

                // Användaren är inloggad, implementera keybindings för chattfunktioner
                Console.WriteLine("Tryck r för att registrera användare.");
                Console.WriteLine("Tryck l för att logga in.");
                Console.WriteLine("Tryck i för att börja skriva meddelanden.");
                Console.WriteLine("Tryck Enter för att skicka meddelandet.");
                Console.WriteLine("Tryck Backspace för att gå tillbaka till registrering/login menyn.");

                if (isInMessageMode)
                {
                    Console.WriteLine("Tryck Enter för att skicka meddelandet");
                }
                else
                {
                    Console.WriteLine("Tryck i för att börja skriva meddelanden");
                }

                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("\nSkriv meddelande och tryck Enter:");
                        string message = Console.ReadLine()!;
                        // Skicka meddelandet till servern
                        break;

                        
                    case ConsoleKey.D2:
                        Console.WriteLine("\nAnge användarnamn för privat meddelande:");
                        string recipient = Console.ReadLine();
                        Console.WriteLine($"Skriv ditt privat meddelande till {recipient} och tryck Enter:");
                        string privateMessage = Console.ReadLine();
                        // Skicka det privata meddelandet till servern
                        break;
                }

            }
        }
    }
}

class ChatClient
{

}