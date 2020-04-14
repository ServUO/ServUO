namespace Server.Spells
{
    public enum SpellState
    {
        None = 0,
        Casting = 1,	// We are in the process of casting (that is, waiting GetCastTime() and doing animations). Spell casting may be interupted in this state.
        Sequencing = 2	// Casting completed, but the full spell sequence isn't. Usually waiting for a target response. Some actions are restricted in this state (using skills for example).
    }
}