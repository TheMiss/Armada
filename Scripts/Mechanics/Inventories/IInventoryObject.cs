namespace Armageddon.Mechanics.Inventories
{
    /// <summary>
    ///     Implement this interface will make that object be able to insert into slot
    /// </summary>
    public interface IInventoryObject
    {
        string InstanceId { get; }
    }
}
