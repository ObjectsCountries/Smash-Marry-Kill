using System.Collections.Generic;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;

public sealed class SMK_TP : Twitch<SmashMarryKill>
{
    [Command("")]
    public override IEnumerable<Instruction> ForceSolve()
    {
        yield return null;
    }
}
