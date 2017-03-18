using System;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public interface IBloodCreature
    {
    }

    [CorpseName("a bloodworm corpse")]
    public class BloodWorm : BaseCreature, IBloodCreature
    {
        [Constructable]
        public BloodWorm()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bloodworm";
            Body = 287;

            SetStr(401, 473);
            SetDex(80);
            SetInt(18, 19);

            SetHits(374, 422);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);
				
            SetResistance(ResistanceType.Physical, 52, 55);
            SetResistance(ResistanceType.Fire, 42, 50);
            SetResistance(ResistanceType.Cold, 29, 31);
            SetResistance(ResistanceType.Poison, 69, 75);
            SetResistance(ResistanceType.Energy, 26, 27);

            SetSkill(SkillName.MagicResist, 35.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
        }

        public BloodWorm(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.02)            
                c.DropItem(new LuckyCoin());            
        }

        public override int GetIdleSound()
        {
            return 1503;
        }

        public override int GetAngerSound()
        {
            return 1500;
        }

        public override int GetHurtSound()
        {
            return 1502;
        }

        public override int GetDeathSound()
        {
            return 1501;
        }

        private static Dictionary<Mobile, AnemiaTimer> m_AnemiaTable = new Dictionary<Mobile, AnemiaTimer>();

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            TryInfect(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.10 > Utility.RandomDouble())
            {
                if (m_AnemiaTable.ContainsKey(defender))
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 1111668); // * The creature is repulsed by your diseased blood. *
                }
                else
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 1111698); // *The creature drains some of your blood to replenish its health.*

                    Hits = Math.Min(HitsMax, Hits + Utility.RandomMinMax(50, 70));
                }
            }

            TryInfect(defender);
        }

        private void TryInfect(Mobile attacker)
        {
            if (!m_AnemiaTable.ContainsKey(attacker) && this.InRange(attacker, 1) && 0.25 > Utility.RandomDouble() && !FountainOfFortune.UnderProtection(attacker))
            {
                // The creature's attack weakens you. You have become anemic.
                attacker.SendLocalizedMessage(1111669, "", 0x25);

                attacker.PlaySound(0x213);
                Effects.SendTargetParticles(attacker, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);

                AnemiaTimer timer = new AnemiaTimer(attacker);
                timer.Start();

                int str = attacker.RawStr / 3;
                int dex = attacker.RawDex / 3;
                int Int = attacker.RawInt / 3;

                attacker.AddStatMod(new StatMod(StatType.Str, "BloodWorm_Str", str, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Dex, "BloodWorm_Dex", dex, TimeSpan.FromSeconds(60)));
                attacker.AddStatMod(new StatMod(StatType.Int, "BloodWorm_Int", Int, TimeSpan.FromSeconds(60)));

                // -~1_STR~ strength.<br>-~2_INT~ intelligence.<br>-~3_DEX~ dexterity.<br> Drains all stamina.
                BuffInfo.AddBuff(attacker, new BuffInfo(BuffIcon.BloodwormAnemia, 1153797, 1153824, String.Format("{0}\t{1}\t{2}", str, dex, Int)));

                m_AnemiaTable.Add(attacker, timer);
            }
        }

        private class AnemiaTimer : Timer
        {
            private DateTime _Expires;
            private Mobile m_Victim;

            public AnemiaTimer(Mobile m)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Victim = m;

                _Expires = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            }

            protected override void OnTick()
            {
                if (_Expires < DateTime.UtcNow || m_Victim.Deleted || !m_Victim.Alive || m_Victim.IsDeadBondedPet)
                {
                    // You no longer feel sick.
                    m_Victim.SendLocalizedMessage(1111673);

                    m_AnemiaTable.Remove(m_Victim);

                    BuffInfo.RemoveBuff(m_Victim, BuffIcon.BloodwormAnemia);

                    Stop();
                }
                else
                {
                    m_Victim.Stam -= m_Victim.Stam < 2 ? 0 : Utility.RandomMinMax(2, 5);
                }
            }
        }

        public override void OnAfterMove(Point3D oldLocation)
        {
            base.OnAfterMove(oldLocation);

            if (Hits < HitsMax && 0.25 > Utility.RandomDouble())
            {
                Corpse toAbsorb = null;

                foreach (Item item in Map.GetItemsInRange(Location, 1))
                {
                    if (item is Corpse)
                    {
                        Corpse c = (Corpse)item;

                        if (c.ItemID == 0x2006)
                        {
                            toAbsorb = c;
                            break;
                        }
                    }
                }

                if (toAbsorb != null)
                {
                    toAbsorb.ProcessDelta();
                    toAbsorb.SendRemovePacket();
                    toAbsorb.ItemID = Utility.Random(0xECA, 9); // bone graphic
                    toAbsorb.Hue = 0;
                    toAbsorb.Direction = Direction.North;
                    toAbsorb.ProcessDelta();

                    Hits = HitsMax;

                    // * The creature drains blood from a nearby corpse to heal itself. *
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 1111699);
                }
            }
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