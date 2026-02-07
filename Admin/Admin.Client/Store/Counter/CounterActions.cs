namespace Admin.Client.Store.Counter;

// Synchronous actions
public record IncrementCounterAction;

public record DecrementCounterAction;

public record ResetCounterAction;

// Asynchronous actions for effects
public record IncrementCounterAsyncAction(string IncrementedBy);

public record IncrementCounterAsyncSuccessAction(int NewCount, string IncrementedBy);

public record IncrementCounterAsyncFailureAction(string ErrorMessage);
