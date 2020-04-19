namespace Server.Items
{
    /// <summary>
    /// Available on some crossbows, this special move allows archers to fire while on the move.
    /// This shot is somewhat less accurate than normal, but the ability to fire while running is a clear advantage.
    /// </summary>
    public class MovingShot : WeaponAbility
    {
        public override int BaseMana => 20;

        public override int AccuracyBonus => -35;

        public override bool ValidatesDuringHit => false;

        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            return (Validate(attacker) && CheckMana(attacker, true));
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            //Validates in OnSwing for accuracy scalar
            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060089); // You fail to execute your special move
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            //Validates in OnSwing for accuracy scalar
            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1060216); // Your shot was successful
        }
    }
}
