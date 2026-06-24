namespace DustInTheWind.Fintown.Toolkit.Infrastructure;

public class StateMachine<TState, TContext>
	where TState : struct, Enum
{
	private readonly Dictionary<TState, IState<TState, TContext>> statesById = new();

	public TState? InitialState { get; set; }

	public TState? CurrentState { get; private set; }

	public StateMachine()
	{
	}

	public StateMachine(IEnumerable<IState<TState, TContext>> steps)
	{
		ArgumentNullException.ThrowIfNull(steps);

		foreach (IState<TState, TContext> step in steps)
			AddStep(step);
	}

	public StateMachine<TState, TContext> AddStep(IState<TState, TContext> state)
	{
		AddStateInternal(state);

		return this;
	}

	public StateMachine<TState, TContext> AddState(IEnumerable<IState<TState, TContext>> state)
	{
		ArgumentNullException.ThrowIfNull(state);

		foreach (IState<TState, TContext> step in state)
			AddStateInternal(step);

		return this;
	}

	private void AddStateInternal(IState<TState, TContext> state)
	{
		ArgumentNullException.ThrowIfNull(state);

		bool isFirstStep = statesById.Count == 0;

		bool success = statesById.TryAdd(state.Id, state);

		if (!success)
			throw new ArgumentException($"A step with id '{state.Id}' is already registered.", nameof(state));

		if (isFirstStep && InitialState == null)
			InitialState = state.Id;
	}

	public async Task ExecuteAllAsync(TContext context)
	{
		Start(context);

		while (await MoveNextAsync())
		{
		}
	}
	
	// public async Task ExecuteAllAsync(TContext context)
	// {
	// 	TStep? current = InitialStep;
	//
	// 	while (current.HasValue)
	// 	{
	// 		if (!steps.TryGetValue(current.Value, out IStep<TStep, TContext> step))
	// 			throw new InvalidOperationException($"No step registered for '{current.Value}'.");
	//
	// 		current = await step.ExecuteAsync(context);
	// 	}
	// }

	private TContext context;

	public void Start(TContext context)
	{
		this.context = context ?? throw new ArgumentNullException(nameof(context));
		CurrentState = InitialState;
	}

	public async Task<bool> MoveNextAsync()
	{
		TState? currentStep = CurrentState;

		if (!currentStep.HasValue)
			return false;

		if (!statesById.TryGetValue(currentStep.Value, out IState<TState, TContext> step))
			throw new InvalidOperationException($"No step registered for '{currentStep.Value}'.");

		CurrentState = await step.ExecuteAsync(context);

		return true;
	}
}