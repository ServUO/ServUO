using System;
using Server.Items;
using Server.Items.Holiday;

namespace Server.Mobiles
{
    [CorpseName("a killer pumpkin corpse")]
    public class PumpkinHead : BaseCreature
    {
        [Constructable]
        public PumpkinHead()
            : base(Utility.RandomBool() ? AIType.AI_Melee : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.05, 0.1)
        {
            this.Name = "a killer pumpkin";
            this.Body = 1246 + Utility.Random(2);

            this.BaseSoundID = 268;

            this.SetStr(350);
            this.SetDex(125);
            this.SetInt(250);

            this.SetHits(500);
            this.SetMana(1000);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55);
            this.SetResistance(ResistanceType.Fire, 50);
            this.SetResistance(ResistanceType.Cold, 50);
            this.SetResistance(ResistanceType.Poison, 65);
            this.SetResistance(ResistanceType.Energy, 80);

            this.SetSkill(SkillName.DetectHidden, 100.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.Necromancy, 100.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);
            this.SetSkill(SkillName.Magery, 160.0);
            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 49;
        }

        public PumpkinHead(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            if (Utility.RandomDouble() < .05)
            {
                //PackItem( new TwilightLantern() );  OLD Halloween
                switch( Utility.Random(5) )
                {
                    case 0:
                        this.PackItem(new PaintedEvilClownMask());
                        break;
                    case 1:
                        this.PackItem(new PaintedDaemonMask());
                        break;
                    case 2:
                        this.PackItem(new PaintedPlagueMask());
                        break;
                    case 3:
                        this.PackItem(new PaintedEvilJesterMask());
                        break;
                    case 4:
                        this.PackItem(new PaintedPorcelainMask());
                        break;
                    default:
                        break;
                }
            }

            this.PackItem(new WrappedCandy());
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public virtual void Lifted_Callback(Mobile from)
        {
            if (from != null && !from.Deleted && from is PlayerMobile)
            {
                this.Combatant = from;

                this.Warmode = true;
            }
        }

        public override Item NewHarmfulItem()
        {
            Item bad = new AcidSlime(TimeSpan.FromSeconds(10), 25, 30);

            bad.Name = "gooey nasty pumpkin hummus";

            bad.Hue = 144;

            return bad;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomBool())
            {
                if (from != null && from.Map != null && this.Map != Map.Internal && this.Map == from.Map && from.InRange(this, 12))
                {
                    this.SpillAcid((willKill) ? this : from, (willKill) ? 3 : 1);
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}