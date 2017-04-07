using System;
using Server.Gumps;

namespace Server.Mobiles 
{ 
    [CorpseName("an ethereal warrior corpse")] 
    public class EtherealWarrior : BaseCreature 
    { 
        private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);
        private DateTime m_NextResurrect;
        [Constructable] 
        public EtherealWarrior()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        { 
            this.Name = NameList.RandomName("ethereal warrior");
            this.Body = 123;

            this.SetStr(586, 785);
            this.SetDex(177, 255);
            this.SetInt(351, 450);

            this.SetHits(352, 471);

            this.SetDamage(13, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 50.1, 75.0);
            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 99.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 100.0);

            this.Fame = 7000;
            this.Karma = 7000;

            this.VirtualArmor = 120;
        }

        public EtherealWarrior(Serial serial)
            : base(serial)
        { 
        }

        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 5 : 0;
            }
        }

        public override TribeType Tribe { get { return TribeType.Fey; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override int Feathers
        {
            get
            {
                return 100;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
            this.AddLoot(LootPack.Gems);
        }

        public override void OnMovement(Mobile from, Point3D oldLocation)
        {
            if (!from.Alive && (from is PlayerMobile))
            {
                if (!from.Frozen && (DateTime.UtcNow >= this.m_NextResurrect) && this.InRange(from, 4) && !this.InRange(oldLocation, 4) && this.InLOS(from))
                {
                    this.m_NextResurrect = DateTime.UtcNow + ResurrectDelay;
                    if (!from.Criminal && (from.Kills < 5) && (from.Karma > 0))
                    {
                        if (from.Map != null && from.Map.CanFit(from.Location, 16, false, false))
                        {
                            this.Direction = this.GetDirectionTo(from);
                            from.PlaySound(0x1F2);
                            from.FixedEffect(0x376A, 10, 16);
                            from.CloseGump(typeof(ResurrectGump));
                            from.SendGump(new ResurrectGump(from, ResurrectMessage.Healer));
                        }
                    }
                }
            }
        }

        public override int GetAngerSound()
        {
            return 0x2F8;
        }

        public override int GetIdleSound()
        {
            return 0x2F8;
        }

        public override int GetAttackSound()
        {
            return Utility.Random(0x2F5, 2);
        }

        public override int GetHurtSound()
        {
            return 0x2F9;
        }

        public override int GetDeathSound()
        {
            return 0x2F7;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.25 > Utility.RandomDouble())
            {
                int toSap = Utility.RandomMinMax(20, 30);

                switch (Utility.Random(3))
                {
                    case 0:
                        defender.Damage(toSap, this);
                        Hits += toSap;
                        break;
                    case 1:
                        defender.Stam -= toSap;
                        Stam += toSap;
                        break;
                    case 2:
                        defender.Mana -= toSap;
                        Mana += toSap;
                        break;
                }
            }
            /*defender.Damage(Utility.Random(10, 10), this);
            defender.Stam -= Utility.Random(10, 10);
            defender.Mana -= Utility.Random(10, 10);*/
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            /*attacker.Damage(Utility.Random(10, 10), this);
            attacker.Stam -= Utility.Random(10, 10);
            attacker.Mana -= Utility.Random(10, 10);*/
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
