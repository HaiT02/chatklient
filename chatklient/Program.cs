namespace chatklient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("---Chatklient ---");

            string anvandarnamn = "";
            while (string.IsNullOrWhiteSpace(anvandarnamn))
            {
                Console.Write("Skriv ditt användarnamn: ");
                anvandarnamn = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(anvandarnamn))
                {
                    Console.WriteLine("Användarnamn kan inte vara tomt. Försök igen.");
                }
            }

            var socket = new ChatSocketManager();
            await socket.Anslut();

            Console.WriteLine("Skriv ett meddelande och tryck Enter för att skicka.");
            Console.WriteLine("Skriv '/exit' för att stänga chatten.\n");

            while (true)
            {
                string? meddelande = Console.ReadLine();
                if (meddelande == null) break;

                meddelande = meddelande.Trim();
                if (meddelande.Length == 0) continue;

                if (meddelande.Equals("/exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                string combined = $"{anvandarnamn}: {meddelande}";
                await socket.Skicka(combined);
            }

            await socket.Nedkoppling();

            Console.WriteLine("Chatten stängs. Hejdå!");
        }
    }
}