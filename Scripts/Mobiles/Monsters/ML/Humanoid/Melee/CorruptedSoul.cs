using System;

namespace Server.Mobiles
{
    public class CorruptedSoul : BaseCreature
    {
        [Constructable]
        public CorruptedSoul()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .1, 5)
        {
            this.Name = "a corrupted soul";
            this.Body = 0x3CA;
            this.Hue = 0x453;

            this.SetStr(102, 115);
            this.SetDex(101, 115);
            this.SetInt(203, 215);

            this.SetHits(61, 69);

            this.SetDamage(4, 40);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 61, 74);
            this.SetResistance(ResistanceType.Fire, 22, 48);
            this.SetResistance(ResistanceType.Cold, 73, 100);
            this.SetResistance(ResistanceType.Poison, 0);
            this.SetResistance(ResistanceType.Energy, 51, 60);

            this.SetSkill(SkillName.MagicResist, 80.2, 89.4);
            this.SetSkill(SkillName.Tactics, 81.3, 89.9);
            this.SetSkill(SkillName.Wrestling, 80.1, 88.7);

            this.Fame = 5000;
            this.Karma = -5000;
            // VirtualArmor = 6; Not sure
        }

        public CorruptedSoul(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysAttackable
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }// NEED TO VERIFY

        // NEED TO VERIFY SOUNDS! Known: No Idle Sound.

        /*public override int GetDeathSound()
        {
        return 0x0;
        }*/
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        /*public override int GetAngerSound()
        {
        return 0x0;
        }*/
        public override int GetAttackSound()
        {
            return 0x233;
        }

        // TODO: Proper OnDeath Effect
        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            // 1 in 20 chance that a Thread of Fate will appear in the killer's pack

            Effects.SendLocationEffect(this.Location, this.Map, 0x376A, 10, 1);
            return true;
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