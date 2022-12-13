using Orleans.Runtime;
using TicTacToe.Engine;
using TicTacToe.Interfaces.Grains;
using TicTacToe.Shared.Models;

namespace TicTacToe.OrleansSilo.GrainImplementations;

public class GameGrain : IGameGrain, IGrainBase
{
    public IGrainContext GrainContext { get; }

    private readonly Game game = new();
    private readonly Dictionary<Symbol, string> users = new();

    public GameGrain(IGrainContext grainContext)
    {
        this.GrainContext = grainContext;
    }

    public Task<Symbol> JoinGameAsync(string gameId, string userId)
    {
        if (!users.TryGetValue(Symbol.X, out _))
        {
            users[Symbol.X] = userId;
            return Task.FromResult(Symbol.X);
        }

        if (!users.TryGetValue(Symbol.O, out _))
        {
            users[Symbol.O] = userId;
            return Task.FromResult(Symbol.O);
        }

        throw new InvalidOperationException("No players left in game");
    }

    public Task<GameState> GetGameStateAsync()
    {
        return Task.FromResult(game.CurrentState);
    }

    public Task AttemptPlayAsync(string userId, Play play)
    {
        if (!users.TryGetValue(play.Symbol, out var symbolUser))
        {
            throw new InvalidOperationException("No player assigned to symbol " + play.Symbol);
        }
        if (symbolUser != userId)
        {
            throw new InvalidOperationException("Player not assigned to symbol " + play.Symbol);
        }

        game.AttemptPlay(play);

        return Task.CompletedTask;
    }
}
