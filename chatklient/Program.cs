namespace chatklient;

class Program
{
    static async Task Main(string[] args)
    {
        // Vi ansluter till Socket servern.
        await SocketManager.Connect();
            
        while (true)
        {

        }
    }
}