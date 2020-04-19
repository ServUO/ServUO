using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Spells.SkillMasteries
{
    public class ConduitSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Conduit", "Uus Corp Grav",
                204,
                9061,
                Reagent.NoxCrystal,
                Reagent.BatWing,
                Reagent.GraveDust
            );

        public override double RequiredSkill => 90;
        public override double UpKeep => 0;
        public override int RequiredMana => 40;
        public override bool PartyEffects => false;

        public override SkillName CastSkill => SkillName.Necromancy;
        public override SkillName DamageSkill => SkillName.SpiritSpeak;

        public int Strength { get; set; }
        public List<Item> Skulls { get; set; }
        public Rectangle2D Zone { get; set; }

        public ConduitSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration), 0x36CB, 1, 14, 0x55C, 7, 9915, 0);
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this, 10, true, Targeting.TargetFlags.None);
        }

        protected override void OnTarget(object o)
        {
            IPoint3D p = o as IPoint3D;

            if (p != null && CheckSequence())
            {
                Rectangle2D rec = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);
                Skulls = new List<Item>();

                Item skull = new InternalItem();
                skull.MoveToWorld(new Point3D(rec.X, rec.Y, Caster.Map.GetAverageZ(rec.X, rec.Y)), Caster.Map);
                Skulls.Add(skull);

                skull = new InternalItem();
                skull.MoveToWorld(new Point3D(rec.X + rec.Width, rec.Y + rec.Height, Caster.Map.GetAverageZ(rec.X + rec.Width, rec.Y + rec.Height)), Caster.Map);
                Skulls.Add(skull);

                skull = new InternalItem();
                skull.MoveToWorld(new Point3D(rec.X + rec.Width, rec.Y, Caster.Map.GetAverageZ(rec.X + rec.Width, rec.Y)), Caster.Map);
                Skulls.Add(skull);

                skull = new InternalItem();
                skull.MoveToWorld(new Point3D(rec.X, rec.Y + rec.Height, Caster.Map.GetAverageZ(rec.X, rec.Y + rec.Height)), Caster.Map);
                Skulls.Add(skull);

                skull = new InternalItem();
                skull.MoveToWorld(new Point3D(rec.X + (rec.Width / 2), rec.Y + (rec.Height / 2), Caster.Map.GetAverageZ(rec.X + (rec.Width / 2), rec.Y + (rec.Height / 2))), Caster.Map);
                Skulls.Add(skull);

                Zone = rec;
                Strength = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value + (GetMasteryLevel() * 20)) / 3.75);
                Expires = DateTime.UtcNow + TimeSpan.FromSeconds(6);

                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Conduit, 1155901, 1156053, Strength.ToString())); //Targeted Necromancy spells used on a target within the Conduit field will affect all valid targets within the field at ~1_PERCT~% strength. 

                BeginTimer();
            }
        }

        public override void EndEffects()
        {
            ColUtility.ForEach(Skulls.Where(i => i != null && !i.Deleted), i => i.Delete());
            ColUtility.Free(Skulls);

            BuffInfo.RemoveBuff(Caster, BuffIcon.Conduit);
        }

        public static bool CheckAffected(Mobile caster, IDamageable victim, Action<Mobile, double> callback)
        {
            if (victim == null || victim.Map == null)
                return false;

            foreach (SkillMasterySpell spell in EnumerateSpells(caster, typeof(ConduitSpell)))
            {
                ConduitSpell conduit = spell as ConduitSpell;

                if (conduit == null)
                    continue;

                if (conduit.Zone.Contains(victim))
                {
                    IPooledEnumerable eable = victim.Map.GetMobilesInBounds(conduit.Zone);
                    List<Mobile> toAffect = null;

                    foreach (Mobile m in eable)
                    {
                        if (m != victim && conduit.Caster.CanBeHarmful(m))
                        {
                            if (toAffect == null)
                                toAffect = new List<Mobile>();

                            toAffect.Add(m);
                        }
                    }

                    eable.Free();

                    if (toAffect != null && callback != null)
                    {
                        toAffect.ForEach(m => callback(m, conduit.Strength / 100.0));
                        ColUtility.Free(toAffect);
                        return true;
                    }
                }
            }

            return false;
        }

        private class InternalItem : Item
        {
            public InternalItem()
                : base(Utility.RandomList(0x1853, 0x1858))
            {
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                Delete();
            }
        }
    }
}
