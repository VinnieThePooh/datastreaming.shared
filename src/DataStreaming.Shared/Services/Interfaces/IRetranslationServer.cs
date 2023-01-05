using System.Net;
using DataStreaming.Models;
using DataStreaming.Models.FileTransfer;

namespace DataStreaming.Services.Interfaces;

public interface IRetranslationServer
{
    Task<bool> Start();

    Task<bool> Stop();

    Dictionary<IPEndPoint, ClientProxy> ClientProxies { get; }
}