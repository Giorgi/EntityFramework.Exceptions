namespace EntityFramework.Exceptions.Common;

public class ExceptionProcessorOptions
{
    /// <summary>
    /// When <c>true</c>, deadlock exceptions thrown by non-SaveChanges commands (e.g. ExecuteUpdate,
    /// ExecuteDelete) are converted to <see cref="DeadlockException"/>. This prevents the execution
    /// strategy from detecting the transient error and retrying automatically.
    /// When <c>false</c> (default), deadlocks from non-SaveChanges commands are left for the execution
    /// strategy to handle (retry or surface as-is). Deadlocks from SaveChanges are always converted.
    /// </summary>
    public bool HandleDeadlock { get; set; }
}
