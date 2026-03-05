/*namespace WgCleaningApp.Domain.Entities;

public class CleaningAssignment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TaskId { get; private set; }
    public DateTime WeekStartDate { get; private set; }
    public bool IsCompleted { get; private set; }

    private CleaningAssignment() { }

    public CleaningAssignment(Guid userId, Guid taskId, DateTime weekStartDate)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TaskId = taskId;
        WeekStartDate = weekStartDate;
        IsCompleted = false;
    }

    public void MarkCompleted()
    {
        if (IsCompleted)
            throw new Exception("Already completed.");

        IsCompleted = true;
    }
}*/