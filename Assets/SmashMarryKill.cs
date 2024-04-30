using Wawa.Extensions;
using Wawa.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class SmashMarryKill : ModdedModule
{

    public KMSelectable module;
    public TextMesh result;
    public KMSelectable[] candidates;
    public TextMesh[] candidateNames;

    private int currentIndex = 0;
    ///<value>The words "SMASH", "MARRY" and "KILL", for ease of use.</value>
    private enum SMKwords
    {
        SMASH,
        MARRY,
        KILL
    }

    protected override void Awake()
    {
        foreach (KMSelectable candidate in candidates)
        {
            KMSelectable unused = candidate.Set(onInteract: () =>
            {
                if (candidate.GetComponentInChildren<TextMesh>().color == Color.white)
                {
                    SMKselect(currentIndex, candidate, candidate.GetComponentInChildren<TextMesh>());
                    currentIndex += 1;
                    currentIndex %= 3;
                    if (currentIndex == 0)
                    {
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            candidate_.GetComponentInChildren<TextMesh>().color = Color.white;
                        }
                    }
                }
            });
        }
    }

    ///<summary>Selection of Smash/Marry/Kill.</summary>
    ///<param name="index">The current index of the Smash/Marry/Kill cycle.</param>
    ///<param name="candidateButton">The button to alter.</param>
    ///<param name="candidateName">The button's TextMesh to alter.</param>
    private void SMKselect(int index, KMSelectable candidateButton, TextMesh candidateName)
    {
        result.text = "" + (SMKwords)index;
        candidateName.color = Color.green;
    }
}
