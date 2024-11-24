using System.Collections.Generic;
using System.Linq;
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
        Module.Log("Force solved by Twitch mod.");
        if (Module.modulesSolved == Module.totalNonIgnored)
        {
            yield return Module.candidates[1];
        }
        else
        {
            if (!Module.doneWithCategorization)
            {
                TextMesh[] candidateTexts = Module.candidates.Select(x => x.GetComponentInChildren<TextMesh>()).ToArray();
                foreach (TextMesh t in candidateTexts)
                {
                    if (t.gameObject.activeSelf)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
            }
            Module.result.fontSize = 90;
            Module.result.transform.localPosition = new Vector3(0, 0.0106f, 0);
            Module.Solve("Done!");
            Module.result.text = "DONE";
        }
    }
}
