using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles.MannequinProperty
{
    public abstract class SlayerProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.HitEffects; } }
        public abstract SlayerName Slayer { get; }
        public override int Hue { get { return 0x43FF; } }
        public override bool IsBoolen { get { return true; } }
        public override int Description { get { return 1152467; } } // This property provides increased effectiveness against specific monsters or specific groups of monsters.  When the property is found on weapons or talisman it increases damage inflicted by weapons.  When this property is found on spellbooks it increases spell damage.  <br>When this property is found on instruments it increases the success chances for barding attempts (peacemaking, provocation or discordance); it also increases damage inflicted when using the discordance bard mastery abilities.  When a character uses slayer items they will be more vulnerable to opposing groups of monsters.  This vulnerability ranges from taking more damage from opposing monsters to less effective barding attempts.
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 180; } }

        public override bool Matches(Item item)
        {
            return item is ISlayer slayer ? slayer.Slayer == Slayer || slayer.Slayer2 == Slayer : false;
        }

        public override bool Matches(List<Item> items)
        {
            foreach (var item in items)
            {
                if (item is ISlayer slayer)
                    return slayer.Slayer == Slayer || slayer.Slayer2 == Slayer;
            }

            return false;
        }
    }

    public abstract class TalismanSlayerProperty : ValuedProperty
    {
        public override Catalog Catalog { get { return Catalog.HitEffects; } }
        public abstract TalismanSlayerName Slayer { get; }
        public override int Hue { get { return 0x43FF; } }
        public override bool IsBoolen { get { return true; } }
        public override int SpriteW { get { return 270; } }
        public override int SpriteH { get { return 180; } }

        public override bool Matches(Item item)
        {
            return item is BaseTalisman talisman ? talisman.Slayer == Slayer : false;
        }

        public override bool Matches(List<Item> items)
        {
            foreach (var item in items)
            {
                if (item is BaseTalisman talisman)
                    return talisman.Slayer == Slayer;
            }

            return false;
        }
    }

    public class ReptileSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079751; } } // Reptile Slayer        
        public override SlayerName Slayer { get { return SlayerName.ReptilianDeath; } }
    }

    public class DragonSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1061284; } } // Dragon Slayer
        public override SlayerName Slayer { get { return SlayerName.DragonSlaying; } }
    }

    public class LizardmanSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079738; } } // Lizardman Slayer
        public override SlayerName Slayer { get { return SlayerName.LizardmanSlaughter; } }
    }

    public class OphidianSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079740; } } // Ophidian Slayer
        public override SlayerName Slayer { get { return SlayerName.Ophidian; } }
    }

    public class SnakeSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079744; } } // Snake Slayer
        public override SlayerName Slayer { get { return SlayerName.SnakesBane; } }
    }

    public class ArachnidSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079747; } } // Arachnid Slayer
        public override SlayerName Slayer { get { return SlayerName.ArachnidDoom; } }
    }

    public class ScorpionSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079743; } } // Scorpion Slayer
        public override SlayerName Slayer { get { return SlayerName.ScorpionsBane; } }
    }

    public class SpiderSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079746; } } // Spider Slayer
        public override SlayerName Slayer { get { return SlayerName.SpidersDeath; } }
    }

    public class TerathanSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079753; } } // Terathan Slayer
        public override SlayerName Slayer { get { return SlayerName.Terathan; } }
    }

    public class RepondSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079750; } } // Repond Slayer
        public override SlayerName Slayer { get { return SlayerName.Repond; } }
    }

    public class BatSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072506; } } // Bat Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Bat; } }
    }

    public class BearSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072504; } } // Bear Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Bear; } }
    }

    public class BeetleSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072508; } } // Beetle Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Beetle; } }
    }

    public class BirdSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072509; } } // Bird Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Bird; } }
    }

    public class BovineSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072512; } } // Bovine Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Bovine; } }
    }

    public class FlameSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072511; } } // Flame Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Flame; } }
    }

    public class GoblinSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1095010; } } // Goblin Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Goblin; } }
    }

    public class IceSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072510; } } // Ice Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Ice; } }
    }

    public class MageSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072507; } } // Mage Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Mage; } }
    }

    public class OgreSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079739; } } // Ogre Slayer
        public override SlayerName Slayer { get { return SlayerName.OgreTrashing; } }
    }

    public class OrcSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079741; } } // Orc Slayer
        public override SlayerName Slayer { get { return SlayerName.OrcSlaying; } }
    }

    public class TrollSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079754; } } // Troll Slayer
        public override SlayerName Slayer { get { return SlayerName.TrollSlaughter; } }
    }

    public class VerminSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1072505; } } // Vermin Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Vermin; } }
    }

    public class UndeadSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079752; } } // Undead Slayer
        public override int Description { get { return 1152467; } } // This property provides increased effectiveness against specific monsters or specific groups of monsters.  When the property is found on weapons or talisman it increases damage inflicted by weapons.  When this property is found on spellbooks it increases spell damage.  <br>When this property is found on instruments it increases the success chances for barding attempts (peacemaking, provocation or discordance); it also increases damage inflicted when using the discordance bard mastery abilities.  When a character uses slayer items they will be more vulnerable to opposing groups of monsters.  This vulnerability ranges from taking more damage from opposing monsters to less effective barding attempts.
        public override SlayerName Slayer { get { return SlayerName.Silver; } }
    }

    public class WolfSlayerProperty : TalismanSlayerProperty
    {
        public override int LabelNumber { get { return 1075462; } } // Wolf Slayer
        public override TalismanSlayerName Slayer { get { return TalismanSlayerName.Wolf; } }
    }

    public class DemonSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079748; } } // Demon Slayer
        public override SlayerName Slayer { get { return SlayerName.Exorcism; } }
    }

    public class GargoyleSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079737; } } // Gargoyle Slayer
        public override SlayerName Slayer { get { return SlayerName.GargoylesFoe; } }
    }

    public class FeySlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1154652; } } // Fey Slayer
        public override SlayerName Slayer { get { return SlayerName.Fey; } }
    }

    public class ElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079749; } } // Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.ElementalBan; } }
    }

    public class AirElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079733; } } // Air Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.Vacuum; } }
    }

    public class BloodElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079734; } } // Blood Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.BloodDrinking; } }
    }

    public class EarthElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079735; } } // Earth Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.EarthShatter; } }
    }

    public class FireElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079736; } } // Fire Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.FlameDousing; } }
    }

    public class PoisonElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079742; } } // Poison Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.ElementalHealth; } }
    }

    public class SnowElementalSlayerProperty : SlayerProperty
    {
        public override int LabelNumber { get { return 1079745; } } // Snow Elemental Slayer
        public override SlayerName Slayer { get { return SlayerName.SummerWind; } }
    }

    public class WaterElementalSlayerProperty : SlayerProperty
    {
        public override SlayerName Slayer { get { return SlayerName.WaterDissipation; } }
        public override int LabelNumber { get { return 1079755; } } // Water Elemental Slayer
    }
}
