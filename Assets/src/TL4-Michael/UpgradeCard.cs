// COMMAND PATTERN: UpgradeCard binds visible card information to a command.
// The card does not know how the upgrade works.
public class UpgradeCard
{
    public readonly string Title;
    public readonly string Description;
    private readonly IUpgradeCommand command;

    public UpgradeCard(string title, string description, IUpgradeCommand command)
    {
        Title = title;
        Description = description;
        this.command = command;
    }

    // Called by the UI when a card is selected.
    public void Apply(PlayerStats player)
    {
        command.Execute(player); // <-- COMMAND PATTERN: execute selected action
    }
}
