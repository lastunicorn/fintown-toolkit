namespace DustInTheWind.Fintown.Toolkit.Infrastructure;

public class StateMachine<TState, TContext>
	where TState : struct, Enum
{
	private readonly Dictionary<TState, IState<TState, TContext>> statesById = new();
	private TContext context;

	public TState? InitialState { get; set; }

	public TState? CurrentState { get; private set; }

	public StateMachine()
	{
	}

	public StateMachine(IEnumerable<IState<TState, TContext>> states)
	{
		ArgumentNullException.ThrowIfNull(states);

		foreach (IState<TState, TContext> state in states)
			AddState(state);
	}

	public StateMachine<TState, TContext> AddState(IState<TState, TContext> state)
	{
		AddStateInternal(state);
		return this;
	}

	public StateMachine<TState, TContext> AddState(IEnumerable<IState<TState, TContext>> states)
	{
		ArgumentNullException.ThrowIfNull(states);

		foreach (IState<TState, TContext> state in states)
			AddStateInternal(state);

		return this;
	}

	private void AddStateInternal(IState<TState, TContext> state)
	{
		ArgumentNullException.ThrowIfNull(state);

		bool isFirstState = statesById.Count == 0;

		bool success = statesById.TryAdd(state.Id, state);

		if (!success)
			throw new ArgumentException($"A state with id '{state.Id}' is already registered.", nameof(state));

		if (isFirstState && InitialState == null)
			InitialState = state.Id;
	}

	public async Task ExecuteAllAsync(TContext context)
	{
		Start(context);

		while (CurrentState != null)
			await MoveNextAsync();
	}

	public void Start(TContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		CurrentState = InitialState;
	}

	public async Task<bool> MoveNextAsync()
	{
		TState? currentState = CurrentState;

		if (!currentState.HasValue)
			return false;

		if (!statesById.TryGetValue(currentState.Value, out IState<TState, TContext> state))
			throw new InvalidOperationException($"No state registered for '{currentState.Value}'.");

		CurrentState = await state.ExecuteAsync(context);

		return true;
	}
}