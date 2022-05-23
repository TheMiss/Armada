namespace Armageddon.Mechanics
{
    public interface IBadge
    {
        string InstanceId { get; }
        bool IsNoticed { get; set; }
    }
}
