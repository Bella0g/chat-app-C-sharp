namespace chat_client
{
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
                    Console.WriteLine("Tryck i för att börja skriva meddelanden.");
                    Console.WriteLine("Tryck Enter för att skicka meddelandet.");
                    Console.WriteLine("Tryck l för att logga ut.");

                    if (isInMessageMode)
                    {
                        Console.WriteLine("Tryck Enter för att skicka meddelandet");
                    }

                    ConsoleKeyInfo key = Console.ReadKey();

                    switch (char.ToLower(key.KeyChar))
                    {
                        case 'i':
                            isInMessageMode = true;
                            break;

                        case '\r': // '\r' represents the Enter key
                            if (isInMessageMode)
                            {
                                Console.WriteLine("\nSkriv ditt meddelande:");
                                string message = Console.ReadLine()!;
                                // Skicka meddelandet till servern
                                isInMessageMode = false;
                            }
                            break;

                        case 'l':
                            isLoggedIn = false;
                            Console.WriteLine("\nDu har loggat ut.");
                            break;

                        default:
                            Console.WriteLine("\nOgiltig tangent. Försök igen.");
                            break;
                    }
                }

                else
                {
                    // Användaren är inte inloggad, implementera keybindings för inloggning och registrering
                    Console.WriteLine("Tryck r för att registrera användare.");
                    Console.WriteLine("Tryck l för att logga in.");
                    Console.WriteLine("Tryck q för att avsluta programmet.");

                    ConsoleKeyInfo key = Console.ReadKey();

                    switch (char.ToLower(key.KeyChar))
                    {
                        case 'r':
                            Console.WriteLine("\nAnge ditt användarnamn och lösenord för registrering:");
                            // Registreringslogik
                            break;

                        case 'l':
                            Console.WriteLine("\nAnge ditt användarnamn och lösenord för inloggning:");
                            // Inloggningslogik och sätt isLoggedIn till true om inloggningen är framgångsrik
                            isLoggedIn = true;
                            break;

                        case 'q':
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("\nOgiltig tangent. Försök igen.");
                            break;
                    }
                }
            }
        }
    }
}


class ChatClient
{
    // Add client logik
}