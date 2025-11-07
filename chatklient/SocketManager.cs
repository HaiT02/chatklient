namespace chatklient;

using SocketIOClient;

    public static class SocketManager
    {
        private static SocketIO? _client;
        private const string ServerUrl = "wss://api.leetcode.se";
        private const string Path = "/sys25d";
        private const string EventNamn = "hampus_message";
        private const string AnslutHändelse = "användare_anslöt";
        private const string LämnaHändelse = "användare_lämna";
        public record ChatMeddelande(string anvandare, string meddelande, string tidpunkt);
        public static async Task Anslut()
        {
            if (_client != null && _client.Connected)
            {
                Console.WriteLine("Redan ansluten.");
                return;
            }
            
            _client = new SocketIO(ServerUrl, new SocketIOOptions
            {
                Path = Path,
                Reconnection = true
            });
            
            _client.OnConnected += (sender, args) =>
            {
                Console.WriteLine($"[{Nu()}] Ansluten till servern.");
            };
            
            _client.OnDisconnected += (sender, args) =>
            {
                Console.WriteLine($"[{Nu()}] Kopplad från servern. Orsak: {args}");
            };
            
            _client.On(EventNamn, svar =>
            {
                try
                {
                    string text = svar.GetValue<string>();
                    Console.WriteLine($"[{Nu()}] Mottaget: {text}");
                }
                catch
                {
                    Console.WriteLine($"[{Nu()}] Mottog ett meddelande, kunde inte läsa det.");
                }
            });
            
            _client.On(AnslutHändelse, response =>
            {
                try
                {
                    string user = response.GetValue<string>();
                    Console.WriteLine($"[{Nu()}] {user} har gått med i chatten!");
                }
                catch
                {
                    Console.WriteLine($"[{Nu()}] En användare gick med.");
                }
            });
            
            _client.On(LämnaHändelse, response =>
            {
                try
                {
                    string user = response.GetValue<string>();
                    Console.WriteLine($"[{Nu()}] {user} har lämnat chatten.");
                }
                catch
                {
                    Console.WriteLine($"[{Nu()}] En användare lämnade chatten.");
                }
            });
            
            try
            {
                Console.WriteLine("Försöker ansluta...");
                await _client.ConnectAsync();
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kunde inte ansluta: {ex.Message}");
            }
        }
        
        public static async Task Skicka(string text)
        {
            if (_client == null || !_client.Connected)
            {
                Console.WriteLine("Du är inte ansluten.");
                return;
            }

            try
            {
                await _client.EmitAsync(EventNamn, text);
                Console.WriteLine($"[{Nu()}] Du skickade: {text}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid sändning: {ex.Message}");
            }
        }
        
        public static async Task Nedkoppling()
        {
            if (_client == null) return;

            try
            {
                Console.WriteLine("Kopplar ner...");
                await _client.DisconnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid nedkoppling: {ex.Message}");
            }
            finally
            {
                _client.Dispose();
                _client = null;
                Console.WriteLine("Nedkopplad.");
            }
        }

        private static string Nu() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

