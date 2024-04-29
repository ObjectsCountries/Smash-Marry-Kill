// SPDX-License-Identifier: MIT OR Unlicense
// Autogenerated using the Scaffold feature in wawa.Editors.
// Code generated by wawa.Editors is in public domain.
// Source: https://github.com/Emik03/wawa

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Wawa.TwitchPlays;
using Wawa.TwitchPlays.Domains;
using Random = UnityEngine.Random;

/// <summary>Implements Twitch Plays support for <see cref="Solvable"/>.</summary>
public sealed class SolvableTwitch : Twitch<Solvable>
{
    enum Button
    {
        [Alias("top-left"), UsedImplicitly]
        TL,
        [Alias("top-right"), UsedImplicitly]
        TR,
        [Alias("bottom-left"), UsedImplicitly]
        BL,
        [Alias("bottom-right"), UsedImplicitly]
        BR,
    }

    /// <inheritdoc />
    public override IEnumerable<Instruction> ForceSolve()
    {
        throw new NotImplementedException();
    }

    /// <summary>Holds a button for a specified duration.</summary>
    /// <example><para>Examples of commands that invoke this method:<list type="bullet">
    /// <item><description>hold 1</description></item>
    /// <item><description>HOLD 2.45</description></item>
    /// <item><description>hOlD&#160;&#160;&#160;&#160;6.1 tR</description></item>
    /// </list></para></example>
    /// <param name="duration">How long to hold the buttons for.</param>
    /// <param name="button">Which button to hold, by default the top-left.</param>
    /// <returns>For demonstration purposes, this always throws.</returns>
    [Command]
    IEnumerable<Instruction> Hold(float duration, Button button = Button.TL)
    {
        throw new NotImplementedException();
    }

    /// <summary>Presses a sequence of buttons.</summary>
    /// <example><para>Examples of commands that invoke this method:<list type="bullet">
    /// <item><description>submit</description></item>
    /// <item><description>SUBMIT TL</description></item>
    /// <item><description>sUbMiT tR bL bL bR</description></item>
    /// </list></para></example>
    /// <param name="buttons">The buttons to press, in order.</param>
    /// <returns>For demonstration purposes, this always throws.</returns>
    [Command("submit", 1)] // With priority '1', this method gets evaluated sooner than the method 'Hold'.
    static IEnumerable<Instruction> Press([NotNull] params Button[] buttons)
    {
        throw new NotImplementedException();
    }
}
