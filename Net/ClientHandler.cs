using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Handles a connection from a single player to the server
public class ClientHandler
{
    public int PlayerId { get; }

    private readonly TcpClient _socket;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly object _writeLock = new object();
    private readonly GameServer _server;
    private volatile bool _running = true;

    public ClientHandler(int playerId, TcpClient socket, GameServer server)
    {
        PlayerId = playerId;
        _socket = socket;
        _server = server;

        NetworkStream stream = socket.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8);
        // send each message on its own line
        _writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
    }

    public void Start()
    {
        var thread = new Thread(ReadLoop) { IsBackground = true, Name = $"client-{PlayerId}-read" };
        thread.Start();
    }

    public void Send(ServerMessage message)
    {
        string json = NetJson.Serialize(message);
        try
        {
            lock (_writeLock)
            {
                _writer.WriteLine(json);
            }
        }
        catch
        {
            Disconnect();
        }
    }

    private void ReadLoop()
    {
        try
        {
            while (_running)
            {
                string? line = _reader.ReadLine();
                if (line == null) break;                  // connection closed
                if (string.IsNullOrWhiteSpace(line)) continue;

                ActionMessage? action = NetJson.Deserialize<ActionMessage>(line);
                if (action == null) continue;

                IGameCommand? command = CommandFactory.FromAction(action);
                if (command != null)
                {
                    _server.EnqueueAction(PlayerId, command);
                }
            }
        }
        catch
        {
            // stop if there's an error
        }
        finally
        {
            Disconnect();
        }
    }

    public void Disconnect()
    {
        if (!_running) return;
        _running = false;

        try { _socket.Close(); } catch { /* already closed */ }

        _server.RemoveClient(PlayerId);
    }
}
