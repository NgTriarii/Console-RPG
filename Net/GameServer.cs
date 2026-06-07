using OOD_Project.Entities;
using OOD_Project.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OOD_Project.Net;

// Hosts the game and manages all connected players
public class GameServer
{
    private const int EnemyTickMs = 500;

    private readonly Game _host;            // the host's own game (player 1)
    private readonly GameModel _model;
    private readonly Player _hostPlayer;

    private readonly object _modelLock = new object();
    private readonly TcpListener _listener;
    private readonly Dictionary<int, ClientHandler> _clients = new Dictionary<int, ClientHandler>();
    private readonly object _clientsLock = new object();
    private readonly Queue<(int, IGameCommand)> _actionQueue = new Queue<(int, IGameCommand)>();
    private readonly object _queueLock = new object();

    private volatile bool _running = true;

    public GameServer(Game host, int port)
    {
        _host = host;
        _model = host.Model;
        _hostPlayer = host.LocalPlayer;
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Run()
    {
        _listener.Start();
        _host.InitView();

        new Thread(AcceptLoop) { IsBackground = true, Name = "accept" }.Start();
        new Thread(ConsumerLoop) { IsBackground = true, Name = "consumer" }.Start();
        new Thread(EnemyLoop) { IsBackground = true, Name = "enemy" }.Start();

        lock (_modelLock) { _host.RenderFrame(); }

        HostInputLoop();   // runs until the host leaves

        _running = false;
        lock (_queueLock) { Monitor.PulseAll(_queueLock); }   // wake the consumer so it stops
        try { _listener.Stop(); } catch { }
        lock (_modelLock) { _host.ShowGameOver(); }
    }



    public void EnqueueAction(int playerId, IGameCommand cmd)
    {
        lock (_queueLock)
        {
            if (_running)
            {
                _actionQueue.Enqueue((playerId, cmd));
                Monitor.Pulse(_queueLock);
            }
        }
    }

    public void RemoveClient(int playerId)
    {
        lock (_clientsLock)
        {
            _clients.Remove(playerId);
        }
        var snapshots = ApplyAndSnapshot(() => _model.RemovePlayer(playerId));
        Broadcast(snapshots);
    }



    private void HostInputLoop()
    {
        while (_running && !_hostPlayer.IsDead && !_model.IsGameOver)
        {
            ConsoleKey key = _host.ReadKey();   // don't hold the lock while waiting for a key

            var snapshots = ApplyAndSnapshot(() =>
            {
                bool handled = _host.InputChain.Handle(key, _host);
                if (!handled)
                {
                    _hostPlayer.LastMessage = $"[{key}] is not a valid action. Check available controls.";
                }
            });
            Broadcast(snapshots);
        }
    }

    private void ConsumerLoop()
    {
        while (_running)
        {
            (int playerId, IGameCommand cmd) item = default;
            bool hasItem = false;

            lock (_queueLock)
            {
                if (_actionQueue.Count > 0)
                {
                    item = _actionQueue.Dequeue();
                    hasItem = true;
                }
                else
                {
                    Monitor.Wait(_queueLock, 50); // wait for a new action
                }
            }

            if (hasItem && _running)
            {
                var snapshots = ApplyAndSnapshot(() =>
                {
                    if (_model.Players.TryGetValue(item.playerId, out Player? actor))
                    {
                        item.cmd.Execute(_model, actor);
                    }
                });
                Broadcast(snapshots);
            }
        }
    }

    private void EnemyLoop()
    {
        while (_running)
        {
            Thread.Sleep(EnemyTickMs);
            if (!_running) break;

            var snapshots = ApplyAndSnapshot(() => _model.AdvanceEnemyTurn());
            Broadcast(snapshots);
        }
    }

    private void AcceptLoop()
    {
        while (_running)
        {
            TcpClient socket;
            try { socket = _listener.AcceptTcpClient(); }
            catch { break; }   // listener stopped

            int id;
            ServerMessage? init = null;

            lock (_modelLock)
            {
                var (spawnX, spawnY) = _model.FindSpawnPoint();
                var player = new Player
                {
                    Name = "Player",
                    X = spawnX,
                    Y = spawnY,
                    LastMessage = _model.CurrentMessage
                };

                id = _model.AddPlayer(player);
                if (id != -1)
                {
                    init = new ServerMessage
                    {
                        Type = ServerMessageType.Init,
                        PlayerId = id,
                        State = StateMapper.ToDto(_model, id)
                    };
                }
                _host.RenderFrame();
            }

            if (id == -1)   // game full
            {
                try { socket.Close(); } catch { }
                continue;
            }

            var handler = new ClientHandler(id, socket, this);
            handler.Send(init);
            lock (_clientsLock)
            {
                _clients[id] = handler;
            }
            handler.Start();

            Broadcast(ApplyAndSnapshot(() => { }));
        }
    }



    // Apply changes to the game and create snapshots to broadcast
    private List<(ClientHandler client, ServerMessage msg)> ApplyAndSnapshot(Action mutate)
    {
        lock (_modelLock)
        {
            mutate();
            _host.RenderFrame();

            var snapshots = new List<(ClientHandler, ServerMessage)>();
            lock (_clientsLock)
            {
                foreach (ClientHandler client in _clients.Values)
                {
                    var msg = new ServerMessage
                    {
                        Type = ServerMessageType.State,
                        PlayerId = client.PlayerId,
                        State = StateMapper.ToDto(_model, client.PlayerId)
                    };
                    snapshots.Add((client, msg));
                }
            }
            return snapshots;
        }
    }

    private void Broadcast(List<(ClientHandler client, ServerMessage msg)> snapshots)
    {
        foreach (var (client, msg) in snapshots)
        {
            client.Send(msg);
        }
    }
}
