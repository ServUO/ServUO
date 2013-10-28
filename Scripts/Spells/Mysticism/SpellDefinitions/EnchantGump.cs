using System;
using System.Collections;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class EnchantGump : Gump
    {
        public BaseWeapon m_Item;
        private static readonly Hashtable m_Table = new Hashtable();
        private string m_Name;
        public EnchantGump()
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(130, 90, 280, 180, 9270);   /// Background
            this.AddAlphaRegion(141, 101, 257, 158);   /// Alpha Region
            this.AddImageTiled(374, 100, 26, 160, 10460);   /// Celctic Bars on right
            this.AddItem(133, 98, 6882);   /// Top-Left Skull
            this.AddItem(340, 98, 6883);   /// Top-Right Skull
            this.AddItem(350, 250, 6881);   /// Bottom-Right Skull
            this.AddItem(122, 250, 6880);   /// Bottom-Left Skull
            this.AddHtml(165, 112, 117, 18, @"<BASEFONT COLOR=AQUA>Select Enchant</BASEFONT>", (bool)false, (bool)false);
            this.AddButton(165, 140, 9702, 9703, 1, GumpButtonType.Reply, 1);
            this.AddLabel(185, 138, 87, @"Hit Dispel");
            this.AddButton(165, 160, 9702, 9703, 2, GumpButtonType.Reply, 2);
            this.AddLabel(185, 158, 87, @"Hit Fireball");
            this.AddButton(165, 180, 9702, 9703, 3, GumpButtonType.Reply, 3);
            this.AddLabel(185, 178, 87, @"Hit Harm");
            this.AddButton(165, 200, 9702, 9703, 4, GumpButtonType.Reply, 4);
            this.AddLabel(185, 198, 87, @"Hit Lightning");
            this.AddButton(165, 220, 9702, 9703, 5, GumpButtonType.Reply, 5);
            this.AddLabel(185, 218, 87, @"Hit Magic Arrow");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile m = state.Mobile;
           
            BaseWeapon weapon = m.Weapon as BaseWeapon;
            TimeSpan duration = TimeSpan.FromSeconds((m.Skills[SkillName.Mysticism].Value / 1.2) + 1.0);//needs work, just a base formula.

            switch (info.ButtonID)
            {
                case 0:
                    {
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
                case 1:
                    {
                        ExpireTimer t = (ExpireTimer)m_Table[weapon];
                        if (t != null)
                            t.DoExpire();
                        m.SendMessage("You added Hit Dispel to your weapon");
                        weapon.WeaponAttributes.HitDispel += 30;
                        m.FixedParticles(0x3728, 1, 13, 9912, 1150, 7, EffectLayer.Head);
                        m.FixedParticles(0x3779, 1, 15, 9502, 67, 7, EffectLayer.Head);
                        if (weapon.Name == null)
                            this.m_Name = weapon.GetType().Name;
                        else
                            this.m_Name = weapon.Name;
                        weapon.Name = this.m_Name + " [Enchanted]";
                        m_Table[weapon] = t = new ExpireTimer(weapon, AosWeaponAttribute.HitDispel, duration);
                        t.Start();
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
                case 2:
                    {
                        ExpireTimer t = (ExpireTimer)m_Table[weapon];
                        if (t != null)
                            t.DoExpire();
                        m.SendMessage("You added Hit Fireball to your weapon");
                        weapon.WeaponAttributes.HitFireball += 30;
                        m.FixedParticles(0x3728, 1, 13, 9912, 1150, 7, EffectLayer.Head);
                        m.FixedParticles(0x3779, 1, 15, 9502, 67, 7, EffectLayer.Head);
                        if (weapon.Name == null)
                            this.m_Name = weapon.GetType().Name;
                        else
                            this.m_Name = weapon.Name;
                        weapon.Name = this.m_Name + " [Enchanted]";
                        m_Table[weapon] = t = new ExpireTimer(weapon, AosWeaponAttribute.HitFireball, duration);
                        t.Start();
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
                case 3:
                    {
                        ExpireTimer t = (ExpireTimer)m_Table[weapon];
                        if (t != null)
                            t.DoExpire();
                        m.SendMessage("You added Hit Harm to your weapon");
                        weapon.WeaponAttributes.HitHarm += 30;
                        m.FixedParticles(0x3728, 1, 13, 9912, 1150, 7, EffectLayer.Head);
                        m.FixedParticles(0x3779, 1, 15, 9502, 67, 7, EffectLayer.Head);
                        if (weapon.Name == null)
                            this.m_Name = weapon.GetType().Name;
                        else
                            this.m_Name = weapon.Name;
                        weapon.Name = this.m_Name + " [Enchanted]";
                        m_Table[weapon] = t = new ExpireTimer(weapon, AosWeaponAttribute.HitHarm, duration);
                        t.Start();
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
                case 4:
                    {
                        ExpireTimer t = (ExpireTimer)m_Table[weapon];
                        if (t != null)
                            t.DoExpire();
                        m.SendMessage("You added Hit Lightning to your weapon");
                        weapon.WeaponAttributes.HitLightning += 30;
                        m.FixedParticles(0x3728, 1, 13, 9912, 1150, 7, EffectLayer.Head);
                        m.FixedParticles(0x3779, 1, 15, 9502, 67, 7, EffectLayer.Head);
                        if (weapon.Name == null)
                            this.m_Name = weapon.GetType().Name;
                        else
                            this.m_Name = weapon.Name;
                        weapon.Name = this.m_Name + " [Enchanted]";
                        m_Table[weapon] = t = new ExpireTimer(weapon, AosWeaponAttribute.HitLightning, duration);
                        t.Start();
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
                case 5:
                    {
                        ExpireTimer t = (ExpireTimer)m_Table[weapon];
                        if (t != null)
                            t.DoExpire();
                        m.SendMessage("You added Hit Magic Arrow to your weapon");
                        weapon.WeaponAttributes.HitMagicArrow += 30;
                        m.FixedParticles(0x3728, 1, 13, 9912, 1150, 7, EffectLayer.Head);
                        m.FixedParticles(0x3779, 1, 15, 9502, 67, 7, EffectLayer.Head);
                        if (weapon.Name == null)
                            this.m_Name = weapon.GetType().Name;
                        else
                            this.m_Name = weapon.Name;
                        weapon.Name = this.m_Name + " [Enchanted]";
                        m_Table[weapon] = t = new ExpireTimer(weapon, AosWeaponAttribute.HitMagicArrow, duration);
                        t.Start();
                        m.CloseGump(typeof(EnchantGump));
                        break;
                    }
            }
        }

        private class ExpireTimer : Timer
        {
            private readonly BaseWeapon m_Weapon;
            private readonly AosWeaponAttribute m_Attribute;
            private readonly DateTime m_End;
            public ExpireTimer(BaseWeapon weapon, AosWeaponAttribute attribute, TimeSpan delay)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Weapon = weapon;
                this.m_Attribute = attribute;
                this.m_End = DateTime.UtcNow + delay;

                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.Stop();
                if (this.m_Weapon != null)
                {
                    if (this.m_Weapon.Name == this.m_Weapon.GetType().Name + " [Enchanted]")
                        this.m_Weapon.Name = null;
                    else
                        this.m_Weapon.Name = this.m_Weapon.Name.Replace(" [Enchanted]", "");

                    switch (this.m_Attribute)
                    {
                        case AosWeaponAttribute.HitDispel:
                            this.m_Weapon.WeaponAttributes.HitDispel -= 30;
                            break;
                        case AosWeaponAttribute.HitFireball:
                            this.m_Weapon.WeaponAttributes.HitFireball -= 30;
                            break;
                        case AosWeaponAttribute.HitHarm:
                            this.m_Weapon.WeaponAttributes.HitHarm -= 30;
                            break;
                        case AosWeaponAttribute.HitLightning:
                            this.m_Weapon.WeaponAttributes.HitLightning -= 30;
                            break;
                        case AosWeaponAttribute.HitMagicArrow:
                            this.m_Weapon.WeaponAttributes.HitMagicArrow -= 30;
                            break;
                    }
                }
                m_Table.Remove(this.m_Weapon);
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow >= this.m_End)
                {
                    this.DoExpire();
                }
            }
        }
    }
}
/* TimeSpan duration = TimeSpan.FromSeconds((Caster.Skills[SkillName.Mysticism].Value / 1.2) + 1.0);//needs work, just a base formula.

Timer t = (Timer)m_Table[weapon];

if (t != null)
t.Stop();

weapon.Enchanted = true;

m_Table[weapon] = t = new ExpireTimer(weapon, duration);

t.Start();
}

FinishSequence();
}

private static Hashtable m_Table = new Hashtable();

private class ExpireTimer : Timer
{
private BaseWeapon m_Weapon;

public ExpireTimer(BaseWeapon weapon, TimeSpan delay)
: base(delay)
{
m_Weapon = weapon;
Priority = TimerPriority.OneSecond;
}

protected override void OnTick()
{
m_Weapon.Enchanted = false;
Effects.PlaySound(m_Weapon.GetWorldLocation(), m_Weapon.Map, 0xFA);
m_Table.Remove(this);
}
}

private class SoundEffectTimer : Timer
{
private Mobile m_Mobile;

public SoundEffectTimer(Mobile m)
: base(TimeSpan.FromSeconds(0.75))
{
m_Mobile = m;
Priority = TimerPriority.FiftyMS;
}

protected override void OnTick()
{
m_Mobile.PlaySound(0xFA);
}
}*/