using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Stats
{
    [Exchange(AssignEnumValue = true)]
    public enum StatModifierType
    {
        Flat = 4,

        Percentage = 5
        // PercentMultiply = 300 // Unlike that this will be used.
        // BaseFlat = 2,
        // BasePercentage = 3,
        // TotalFlat = 4
        // TotalPercentageAdditive = 5
        // TotalPercentageMultiplicative = 6
    }
}
