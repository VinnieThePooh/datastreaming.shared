using System.Net;
using DataStreaming.Models.FileTransfer;

namespace DataStreaming.Services.Interfaces;

//todo: remove it later
public interface IRetranslationServer
{
    Dictionary<IPEndPoint, ClientProxy> ClientProxies { get; }
}