using System.Net;
using System.Net.Sockets;
using DataStreaming.Models.Common;

namespace DataStreaming.Services.Interfaces;

public interface IHasClientProxies<TProxy> where TProxy : ISocketClientProxy
{
    Dictionary<IPEndPoint, TProxy> ClientProxies { get; }

    TProxy CreateProxy(Socket party, CancellationToken token);
}