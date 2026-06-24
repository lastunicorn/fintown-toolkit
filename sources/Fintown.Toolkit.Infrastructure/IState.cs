namespace DustInTheWind.Fintown.Toolkit.Infrastructure;

public interface IState<TState, in TContext>
    where TState : struct, Enum
{
    TState Id { get; }

    Task<TState?> ExecuteAsync(TContext context);
}
