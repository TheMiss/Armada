using System;
using System.Collections.Generic;
using Armageddon.Mechanics.Stats;
using Armageddon.Sheets.Effects;
using Armageddon.Sheets.Enchantments;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Mechanics.Enchantments
{
    public class Enchantment : IStatSource
    {
        public Enchantment(object source, EnchantmentSheet sheet, int level)
        {
            Source = source;
            Sheet = sheet;
            Level = level;
        }

        [TableColumnWidth(40)]
        [ShowAsString]
        public string Name => Sheet.Name;

        public EnchantmentSheet Sheet { get; }

        [ShowAsString]
        public int Level { get; set; }

        public object Source { get; }

        public List<Effect> Effects
        {
            get
            {
                try
                {
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

        public override string ToString()
        {
            return Name;
        }
    }
}
