using System;
using System.Collections.Generic;
using Armageddon.Mechanics.Stats;
using Armageddon.Sheets.Abilities;
using Armageddon.Sheets.Effects;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Mechanics.Abilities
{
    public class Ability : IStatSource
    {
        public Ability(object source, AbilitySheet sheet, int level)
        {
            Source = source;
            Sheet = sheet;
            Level = level;
        }

        [TableColumnWidth(40)]
        [ShowAsString]
        public string Name => Sheet.Name;

        [ShowAsString]
        public AbilityType Type => Sheet.Type;

        public AbilitySheet Sheet { get; }

        [ShowAsString]
        public int Level { get; set; }

        [ShowAsString]
        public int ExtraLevel { get; set; }

        public object Source { get; }

        public List<Effect> Effects
        {
            get
            {
                try
                {
                    if (Level == 0)
                    {
                        return null;
                    }

                    EffectDetailsRow row = Sheet.DetailsRows[Level - 1];
                    return row.Effects;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    return new List<Effect>();
                }
            }
        }

        public List<Effect> NextLevelEffects
        {
            get
            {
                try
                {
                    if (Level + 1 >= Sheet.DetailsRows.Count - 1)
                    {
                        return null;
                    }

                    EffectDetailsRow row = Sheet.DetailsRows[Level + 1 - 1];
                    return row.Effects;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    return new List<Effect>();
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
