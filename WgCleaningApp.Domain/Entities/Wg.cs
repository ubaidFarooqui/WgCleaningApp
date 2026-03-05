namespace WgCleaningApp.Domain.Entities;

public class Wg
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string InviteCode { get; private set; } = string.Empty;

    private Wg() { }

    public Wg(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        InviteCode = GenerateInviteCode();
    }

    private string GenerateInviteCode()
    {
        return Guid.NewGuid().ToString().Substring(0, 6);
    }
}