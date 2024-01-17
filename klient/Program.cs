namespace chat_client;

class ChatClient
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
                Console.WriteLine("1. Tryck l för att logga in.");
                Console.WriteLine("2. Tryck i för att börja skriva meddelanden");
                Console.WriteLine("3. Tryck Enter för att skicka.");
                Console.WriteLine("4. Tryck Backspace för att gå tillbaka till login menyn.");

                if (isInMessageMode)
                {
                    Console.WriteLine("Tryck Enter för att skicka meddelandet");
                }
                else
                {
                    Console.WriteLine("Tryck i för att börja skriva meddelanden");
                }

                ConsoleKeyInfo key = Console.ReadKey();
            }
        }
    }
}