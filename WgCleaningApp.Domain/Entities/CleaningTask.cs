/*namespace WgCleaningApp.Domain.Entities;

public class CleaningTask
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid WgId { get; private set; }

    public Guid? AssignedToUserId { get; private set; }

    private CleaningTask() { }

    public CleaningTask(string name, Guid wgId)
    {
        Id = Guid.NewGuid();
        Name = name;
        WgId = wgId;
    }

    public void AssignTo(Guid userId)
    {
        AssignedToUserId = userId;
    }
}*/