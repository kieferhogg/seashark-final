using Microsoft.AspNetCore.SignalR;

namespace final_backend.Hubs
{
  public class UpdatesHub : Hub
  {
    public async Task BroadcastUpdate(string updateType, string updateMessage)
    {
      await Clients.All.SendAsync("ReceiveUpdate", updateType, updateMessage);
    }
  }
}