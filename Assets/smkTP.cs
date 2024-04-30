using System.Collections.Generic;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;

public sealed class smkTP : Twitch<SmashMarryKill>
{
    public override IEnumerable<Instruction> ForceSolve()
    {
        yield return null;
    }
}
