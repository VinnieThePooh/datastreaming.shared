using System.Net;
using DataStreaming.Models.FileTransfer;

namespace DataStreaming.Services.Interfaces;

public interface IRetranslationServer
{
    Dictionary<IPEndPoint, ClientProxy> ClientProxies { get; }
    Task<bool> Start();

    Task<bool> Stop();
}