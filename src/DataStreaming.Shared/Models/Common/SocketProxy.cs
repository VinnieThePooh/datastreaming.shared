using System.Net;
using System.Net.Sockets;
using DataStreaming.Protocols.Interfaces;

namespace DataStreaming.Models.Common;

public interface ISocketClientProxy : IDisposable
{
   IServerSocketProtocol ServerProtocol { get; }
   
   CancellationTokenSource? TokenSource { get; }
   
   Socket? Party { get; }
   
   IPEndPoint EndPoint { get; }

   Task DoCommunication(CancellationToken token);
}