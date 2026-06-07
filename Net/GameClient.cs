using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Handles connecting to the server and drawing the screen
public class GameClient
{
    private readonly string _host;
    private readonly int _port;
    private readonly NetworkClientView _view = new NetworkClientView();

    private TcpClient _socket;
    private StreamReader _reader;
    private StreamWriter _writer;
    private readonly object _writeLock = new object();
    private readonly object _stateLock = new object();

    private GameStateDto? _snapshot;
    private string _lastLoggedMessage = "";
    private int _playerId;
    private int _cursorPos = 0;
    private bool _isLogShown = false;
    private volatile bool _running = true;
    private volatile bool _gameOver = false;

    public GameClient(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public void Run()
    {
        try
        {
            Connect();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to {_host}:{_port} - {ex.Message}");
            return;
        }

        // Get the initial game state from the server
        ServerMessage? init = ReadMessage();
        if (init == null || init.Type != ServerMessageType.Init || init.State == null)
        {
            Console.WriteLine("Server did not send a valid Init message.");
            return;
        }

        _playerId = init.PlayerId;
        _snapshot = init.State;

        _view.Initialize();
        _view.Log($"Connected to {_host}:{_port} as player {_playerId}.");

        lock (_stateLock) { RenderCurrent(); }

        new Thread(ReceiveLoop) { IsBackground = true, Name = "client-rx" }.Start();

        InputLoop();   // blocks on the main thread

        _running = false;
        try { _socket.Close(); } catch { }
    }

    private void Connect()
    {
        _socket = new TcpClient();
        _socket.Connect(_host, _port);

        NetworkStream stream = _socket.GetStream();
        _reader = new StreamReader(stream, Encoding.UTF8);
        _writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
    }

    private ServerMessage? ReadMessage()
    {
        string? line = _reader.ReadLine();
        return line == null ? null : NetJson.Deserialize<ServerMessage>(line);
    }

    private void ReceiveLoop()
    {
        try
        {
            while (_running)
            {
                ServerMessage? msg = ReadMessage();
                if (msg == null) break;            // server closed
                if (msg.State == null) continue;

                lock (_stateLock)
                {
                    _snapshot = msg.State;

                    // Make sure the cursor doesn't go out of bounds
                    int invCount = _snapshot.You.Inventory.Count;
                    if (_cursorPos >= invCount) _cursorPos = invCount > 0 ? invCount - 1 : 0;

                    if (_snapshot.Message != _lastLoggedMessage)
                    {
                        _view.Log(_snapshot.Message);
                        _lastLoggedMessage = _snapshot.Message;
                    }

                    RenderCurrent();

                    if (_snapshot.IsGameOver || _snapshot.You.Health <= 0)
                    {
                        _gameOver = true;
                        _running = false;
                    }
                }
            }
        }
        catch
        {
            // lost connection to the server
        }
        finally
        {
            _running = false;
            lock (_stateLock)
            {
                if (_gameOver) _view.ShowGameOver();
                else _view.ShowDisconnected();
            }
        }
    }

    private void InputLoop()
    {
        while (_running)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (!_running) break;

            if (key == ConsoleKey.Escape)
            {
                _running = false;
                break;
            }

            if (key == ConsoleKey.J)
            {
                lock (_stateLock) { _isLogShown = !_isLogShown; RenderCurrent(); }
                continue;
            }

            if (key == ConsoleKey.I)
            {
                lock (_stateLock)
                {
                    int invCount = _snapshot?.You.Inventory.Count ?? 0;
                    if (invCount > 0) _cursorPos = (_cursorPos + 1) % invCount;
                    RenderCurrent();
                }
                continue;
            }

            ActionType? action = KeyMapper.ToAction(key);
            if (action != null)
            {
                int slot;
                lock (_stateLock) { slot = _cursorPos; }
                Send(new ActionMessage { Action = action.Value, Slot = slot });
            }
        }
    }

    // call this with the state lock held
    private void RenderCurrent()
    {
        if (_snapshot != null)
        {
            _view.Render(_snapshot, _cursorPos, _isLogShown);
        }
    }

    private void Send(ActionMessage msg)
    {
        string json = NetJson.Serialize(msg);
        try
        {
            lock (_writeLock) { _writer.WriteLine(json); }
        }
        catch
        {
            _running = false;
        }
    }
}
