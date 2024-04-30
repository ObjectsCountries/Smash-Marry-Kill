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
            candidate.Set(onInteract: () =>
            {
                if (candidate.GetComponentInChildren<TextMesh>().color == Color.white)
                {
                    result.text = "" + (SMKwords)currentIndex;
                    candidate.GetComponentInChildren<TextMesh>().color = Color.green;
                    Log("Pressed button " + (currentIndex + 1));
                    if (currentIndex == 0)
                    {
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            candidate_.GetComponentInChildren<TextMesh>().color = Color.white;
                        }
                    }
                    currentIndex += 1;
                    currentIndex %= 3;
                }
            });
        }
    }
}
