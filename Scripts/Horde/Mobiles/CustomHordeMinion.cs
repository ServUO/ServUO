using Server;
using Server.Items;
using Server.Mobiles;

public class CustomHordeMinion : BaseCreature
{
    [Constructable]
    public CustomHordeMinion()
        : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
    {
        this.Name = "a custom horde minion";
        this.Body = 776;
        this.BaseSoundID = 357;

        this.SetStr(16, 40);
        this.SetDex(31, 60);
        this.SetInt(11, 25);

        this.SetHits(10, 24);

        this.SetDamage(5, 10);

        this.SetDamageType(ResistanceType.Physical, 100);

        this.SetResistance(ResistanceType.Physical, 15, 20);
        this.SetResistance(ResistanceType.Fire, 5, 10);

        this.SetSkill(SkillName.MagicResist, 10.0);
        this.SetSkill(SkillName.Tactics, 0.1, 15.0);
        this.SetSkill(SkillName.Wrestling, 25.1, 40.0);

        this.Fame = 500;
        this.Karma = -500;

        this.VirtualArmor = 18;

        this.AddItem(new LightSource());

        this.PackItem(new Bone(3));
    }

    public CustomHordeMinion(Serial serial)
        : base(serial)
    {
    }

    public override void OnThink()
    {
        base.OnThink();

        // Custom behavior for attacking players
        Mobile target = GetNearestPlayer();
        if (target != null)
        {
            this.Combatant = target;
        }
    }

    private Mobile GetNearestPlayer()
    {
        Mobile nearestPlayer = null;
        double nearestDistance = double.MaxValue;

        foreach (Mobile m in this.GetMobilesInRange(10))
        {
            if (m is PlayerMobile && !m.Hidden && m.Alive)
            {
                double distance = this.GetDistanceToSqrt(m);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = m;
                }
            }
        }

        return nearestPlayer;
    }

    public override void OnDamage(int amount, Mobile from, bool willKill)
    {
        base.OnDamage(amount, from, willKill);

        // Custom behavior for fleeing when damaged
        if (this.Hits < this.HitsMax * 0.1) // Flee when health is below 10%
        {
            Point3D fleeLocation = GetFleeLocation();
            this.MoveToWorld(fleeLocation, this.Map);
        }
    }

    private Point3D GetFleeLocation()
    {
        int x = this.X + Utility.RandomMinMax(-20, 20);
        int y = this.Y + Utility.RandomMinMax(-20, 20);
        int z = this.Map.GetAverageZ(x, y);
        return new Point3D(x, y, z);
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
