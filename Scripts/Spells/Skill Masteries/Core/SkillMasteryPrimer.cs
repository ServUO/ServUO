using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System.Collections.Generic;

namespace Server.Items
{
    public class SkillMasteryPrimer : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Volume { get; set; }

        public override bool ForceShowProperties => true;

        [Constructable]
        public SkillMasteryPrimer(SkillName skill, int volume) : base(7714)
        {
            Skill = skill;
            LootType = LootType.Cursed;

            Volume = volume;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (MasteryInfo.HasLearned(from, Skill, Volume))
                {
                    from.SendLocalizedMessage(1155884, string.Format("#{0}", MasteryInfo.GetLocalization(Skill))); // You are already proficient in this level of ~1_MasterySkill~
                }
                else if (MasteryInfo.LearnMastery(from, Skill, Volume))
                {
                    from.SendLocalizedMessage(1155885, string.Format("#{0}", MasteryInfo.GetLocalization(Skill))); // You have increased your proficiency in ~1_SkillMastery~!

                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x243);

                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                    Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0xAA8, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                    Effects.SendTargetParticles(from, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                    Delete();
                }
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1155882, string.Format("#{0}", MasteryInfo.GetLocalization(Skill))); // Primer on ~1_Skill~
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1155883, string.Format("{0}", GetVolume(Volume))); // Volume ~1_Level~
        }

        private string GetVolume(int volume)
        {
            if (volume == 1)
                return "I";

            if (volume == 2)
                return "II";

            return "III";
        }

        public static void CheckPrimerDrop(BaseCreature killed)
        {
            List<DamageStore> rights = killed.GetLootingRights();

            rights.ForEach(ds =>
            {
                if (ds.m_HasRight)
                {
                    Mobile m = ds.m_Mobile;

                    if (Utility.RandomDouble() < 0.10)
                    {
                        SkillMasteryPrimer primer = GetRandom();

                        if (primer != null)
                        {
                            if (m.Backpack == null || !m.Backpack.TryDropItem(m, primer, false))
                                m.BankBox.DropItem(primer);
                        }

                        m.SendLocalizedMessage(1156209); // You have received a mastery primer!
                    }
                }
            });
        }

        public static SkillMasteryPrimer GetRandom()
        {
            SkillName skill = MasteryInfo.Skills[Utility.RandomMinMax(3, 18)];
            int volume = 1;

            double random = Utility.RandomDouble();

            if (0.2 >= random)
                volume = 3;
            else if (0.5 >= random)
                volume = 2;

            SkillMasteryPrimer primer = new SkillMasteryPrimer(skill, volume);

            return primer;
        }

        public SkillMasteryPrimer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(Volume);
            writer.Write((int)Skill);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Volume = reader.ReadInt();
                    Skill = (SkillName)reader.ReadInt();
                    break;
                case 0:

                    Skill = (SkillName)reader.ReadInt();
                    int id = reader.ReadInt();

                    Volume = MasteryInfo.GetVolume(id, Skill);
                    break;
            }
        }
    }
}