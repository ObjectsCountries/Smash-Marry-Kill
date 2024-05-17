using System.Collections.Generic;
using UnityEngine;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;

public sealed class SMK_TP : Twitch<SmashMarryKill>
{
    [Command("")]
    public IEnumerable<Instruction> Select(string order)
    {
        order = order.ToLowerInvariant();
        if (order == "finish")
        {
            if (Module.modulesSolved == Module.totalNonIgnored)
            {
                yield return null;
                yield return new KMSelectable[] { Module.candidates[1] };
                yield break;
            }
            else
            {
                yield return TwitchString.SendToChatError("{0}, you can’t solve yet.");
                yield break;
            }
        }
        yield return null;
        foreach (char number in order)
        {
            int num;
            if (int.TryParse(number.ToString(), out num))
            {
                if (Module.modulesSolved != Module.totalNonIgnored && !Module.doneWithCategorization)
                {
                    if (num > 0 && num < 4)
                    {
                        yield return null;
                        yield return Module.candidates[num - 1];
                        yield return new WaitForSeconds(.1f);
                    }
                    else
                    {
                        yield return TwitchString.SendToChatError("{0}, your message contained a number outside the range of 1 to 3.");
                        yield break;
                    }
                }
                else
                {
                    yield return TwitchString.SendToChatError("{0}, you cannot categorize right now.");
                    yield break;
                }
            }
            else
            {
                yield return TwitchString.SendToChatError("{0}, your message must consist of either the word “finish” or a sequence of numbers from 1 to 3.");
                yield break;
            }
        }
    }

    public override IEnumerable<Instruction> ForceSolve()
    {
        if (Module.modulesSolved == Module.totalNonIgnored)
        {
            yield return Module.candidates[1];
        }
        else
        {
            yield return TwitchString.SendToChatError("{0}, you can’t solve yet.");
            yield break;
        }
    }
}
