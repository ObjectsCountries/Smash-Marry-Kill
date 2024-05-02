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
    public KMBombInfo bomb;
    public KMSelectable[] candidates;
    public KMBossModule boss;
    private string[] ignoredModules = new string[] { };

    private int currentIndex = 0;
    private bool firstClick = false;
    private bool doneWithCategorization = false;
    private enum SMKwords
    {
        SMASH,
        MARRY,
        KILL
    }

    private List<SMKwords> possibilities = new List<SMKwords>();
    private SMKwords currentSelection;

    private List<string> moduleNames = new List<string>();
    private List<string> usedModules = new List<string>();
    private List<string> smashModules = new List<string>();
    private List<string> marryModules = new List<string>();
    private List<string> killModules = new List<string>();
    private List<string> newSolved = new List<string>();
    private string[] newSolvedCopy = new string[] { };
    private string lastSolvedModule = "";
    private string[] oldSolved = new string[] { };

    private int modulesLeft = -1;
    private int totalNonIgnored = -1;

    protected override void Awake()
    {
        totalNonIgnored = bomb.GetSolvableModuleNames().Count(x => !ignoredModules.Contains(x));
        ignoredModules = boss.GetIgnoredModules("Smash, Marry, Kill", new string[]{
                "100 Levels of Defusal",
                "Amnesia",
                "BadTV",
                "Bamboozling Time Keeper",
                "Button Messer",
                "Cookie Jars",
                "Custom Keys",
                "Divided Squares",
                "Doomsday Button",
                "Don't Touch Anything",
                "Encrypted Hangman",
                "Encryption Bingo",
                "Four-Card Monte",
                "Hogwarts",
                "Lunchtime",
                "Mental Math",
                "OmegaDestroyer",
                "Password Destroyer",
                "Pow",
                "Rules",
                "Sbemail Songs",
                "Smash, Marry, Kill",
                "Supermassive Black Hole",
                "SUSadmin",
                "Sysadmin",
                "Tax Returns",
                "Tech Support",
                "The Impostor",
                "The Klaxon",
                "The Stopwatch",
                "The Time Keeper",
                "The Troll",
                "Timing is Everything",
                "Turn The Key",
                "Turn The Keys",
                "White Hole",
                "Zener Cards",
                "Blind Maze",
                "Colour Code",
                "Free Parking",
                "Heraldry",
                "Langton's Ant",
                "Laundry",
                "Password Generator",
                "Planets",
                "Waste Management",
                "4D Maze",
                "7",
                "ASCII Maze",
                "Bamboozled Again",
                "Bamboozling Button",
                "Bamboozling Button Grid",
                "Beanboozled Again",
                "Black Cipher",
                "Bordered Keys",
                "Burger Alarm",
                "Cheat Checkout",
                "Colour Shuffle",
                "Connected Monitors",
                "Cruello",
                "Cruel Colour Flash",
                "Cruel Match 'em",
                "Cruel Stars",
                "Cruel Synesthesia",
                "The cRule",
                "Cryptic Cycle",
                "The Cube",
                "Cursed Double-Oh",
                "Cursed Vault",
                "Decay",
                "Devilish Eggs",
                "Dimension King",
                "Disordered Keys",
                "Dragon Energy",
                "Dreamcipher",
                "Dungeon",
                "Dungeon 2nd Floor",
                "Echolocation",
                "Encrypted Morse",
                "English Entries",
                "Factory Maze",
                "Faulty RGB Maze",
                "Forget Me Now",
                "Forget's Ultimate Showdown",
                "Game of Life Cruel",
                "Graphic Memory",
                "The Great Void",
                "hexOS",
                "Hill Cycle",
                "The Hypercolor",
                "Hyperrullo",
                "Identifying Soulless",
                "Indigo Cipher",
                "Jenga",
                "Jumble Cycle",
                "Kudosudoku",
                "Latin Hypercube",
                "LEGOs",
                "Lombax Cubes",
                "Lousy Chess",
                "Matrix Mapping",
                "Mazeswapper",
                "Melody Memory",
                "Micro-Modules",
                "Mineswapper",
                "Misery Squares",
                "Mislocation",
                "Misordered Keys",
                "Mystic Maze",
                "The Necronomicon",
                "Neutrinos",
                "Not Coordinates",
                "Not Double-Oh",
                "Not Murder",
                "Not X01",
                "Number Nimbleness",
                "The Octadecayotton",
                "Odd One Out",
                "Old Fogey",
                "One Links To All",
                "Orange Cipher",
                "Outrageous",
                "Pattern Hypercube",
                "Phosphorescence",
                "Polygrid",
                "Puzzword",
                "Quintuples",
                "Railway Cargo Loading",
                "Rain Hell",
                "Recorded Keys",
                "Red Cipher",
                "Reordered Keys",
                "Repo Selector",
                "RGB Arithmetic",
                "RGB Hypermaze",
                "RGB Maze",
                "Robit Programming",
                "Robot Programming",
                "The Samsung",
                "Scalar Dials",
                "Seven Choose Four",
                "Shapes And Bombs",
                "Silo Authorization",
                "Simon Sends",
                "Simon Sings",
                "Simon Stores",
                "Simon's Ultimate Showdown",
                "Simon Swindles",
                "The Sphere",
                "Sporadic Segments",
                "Superposition",
                "Ten-Button Color Code",
                "Three Cryptic Steps",
                "Torture",
                "Turtle Robot",
                "Ultimate Cipher",
                "Ultimate Cycle",
                "UltraStores",
                "Uncolour Flash",
                "Unfair Cipher",
                "Unfair's Cruel Revenge",
                "Unfair's Forgotten Ciphers",
                "Unfair's Revenge",
                "Walking Cube",
                "Wonder Cipher",
                "X-Ring",
                "X-Rotor",
                "XY-Ray"
            });
        moduleNames = bomb.GetSolvedModuleNames().Where(x => !ignoredModules.Contains(x)).ToList();
        if (moduleNames.Count == 0)
        {
            candidates[0].GetComponentInChildren<TextMesh>().text = "";
            candidates[0].gameObject.SetActive(false);
            candidates[1].GetComponentInChildren<TextMesh>().fontSize = 90;
            candidates[1].GetComponentInChildren<TextMesh>().text = "SOLVE";
            candidates[1].GetComponentInChildren<KMHighlightable>().transform.localScale = new Vector3(8, 1, 8);
            candidates[2].GetComponentInChildren<TextMesh>().text = "";
            candidates[2].gameObject.SetActive(false);
            result.text = "";
            candidates[1].Set(onInteract: () =>
            {
                Solve("No non-ignored mods, solving.");
                candidates[1].gameObject.SetActive(false);
            });
            return;
        }
        foreach (KMSelectable candidate in candidates)
        {
            TextMesh moduleName = candidate.GetComponentInChildren<TextMesh>();
            do
            {
                moduleName.text = moduleNames[UnityEngine.Random.Range(0, moduleNames.Count)];
            } while (usedModules.Contains(moduleName.text));
            usedModules.Add(moduleName.text);
            candidate.Set(onInteract: () =>
            {
                if (moduleName.color == Color.white)
                {
                    Log("Categorized " + moduleName.text + " as " + result.text + ".");
                    moduleName.color = Color.green;
                    moduleNames.Remove(moduleName.text);
                    if (currentIndex == 2 && firstClick && moduleNames.Count > 0)
                    {
                        bool done = false;
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            TextMesh moduleName_ = candidate_.GetComponentInChildren<TextMesh>();
                            if ((candidate_.name == "secondCandidate" && moduleNames.Count == 1) || (candidate_.name == "thirdCandidate" && (moduleNames.Count == 1 || moduleNames.Count == 2)))
                            {
                                moduleName_.text = "";
                                done = true;
                                candidate_.gameObject.SetActive(false);
                            }
                            if (done)
                            {
                                continue;
                            }
                            if (moduleNames.Count != 1)
                            {
                                do
                                {
                                    moduleName_.text = moduleNames[UnityEngine.Random.Range(0, moduleNames.Count)];
                                } while (usedModules.Contains(moduleName_.text));
                            }
                            else
                            {
                                moduleName_.text = moduleNames[0];
                            }
                            moduleName_.color = Color.white;
                            usedModules.Add(moduleName_.text);
                        }
                    }
                    firstClick = true;
                    switch (currentIndex)
                    {
                        case 0:
                            smashModules.Add(moduleName.text);
                            break;
                        case 1:
                            marryModules.Add(moduleName.text);
                            break;
                        case 2:
                        default:
                            killModules.Add(moduleName.text);
                            break;
                    }
                    currentIndex++;
                    currentIndex %= 3;
                    result.text = "" + (SMKwords)currentIndex;
                    if (moduleNames.Count > 0)
                    {
                        Log("DEBUG: moduleNames is now " + String.Join(", ", moduleNames.ToArray()) + ".");
                    }
                    else
                    {
                        Log("The SMASH modules are " + String.Join(", ", smashModules.ToArray()) + ".");
                        Log("The MARRY modules are " + String.Join(", ", marryModules.ToArray()) + ".");
                        Log("The KILL modules are " + String.Join(", ", killModules.ToArray()) + ".");
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            candidate_.GetComponentInChildren<TextMesh>().text = "";
                        }
                        for (int s = 0; s < smashModules.Count; s++)
                        {
                            possibilities.Add(SMKwords.SMASH);
                        }
                        for (int m = 0; m < marryModules.Count; m++)
                        {
                            possibilities.Add(SMKwords.MARRY);
                        }
                        for (int k = 0; k < killModules.Count; k++)
                        {
                            possibilities.Add(SMKwords.KILL);
                        }
                        doneWithCategorization = true;
                        SMKselect("");
                    }
                }
            });
        }
    }

    private void SMKselect(string lastSolved)
    {
        currentSelection = possibilities.PickRandom();
        result.text = "" + currentSelection;
        result.fontSize = 90;
        result.transform.localPosition = new Vector3(0, 0.0106f, 0);
        Log("Current selection: " + currentSelection);
        string currentlySolvable = "";
        switch (currentSelection)
        {
            case SMKwords.SMASH:
                currentlySolvable = String.Join(", ", smashModules.ToArray());
                break;
            case SMKwords.MARRY:
                currentlySolvable = String.Join(", ", marryModules.ToArray());
                break;
            case SMKwords.KILL:
                currentlySolvable = String.Join(", ", killModules.ToArray());
                break;
        }
        Log("You are allowed to solve any of the following: " + currentlySolvable + ".");
        if (smashModules.Contains(lastSolved))
        {
            possibilities.Remove(SMKwords.SMASH);
        }
        else if (marryModules.Contains(lastSolved))
        {
            possibilities.Remove(SMKwords.MARRY);
        }
        else if (killModules.Contains(lastSolved))
        {
            possibilities.Remove(SMKwords.KILL);
        }
        Log("DEBUG: possibilities is " + String.Join(", ", possibilities.Select(s => s.ToString()).ToArray()) + ".");
    }

    void Update()
    {
        if (modulesLeft != bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x)) && doneWithCategorization)
        {
            modulesLeft++;
            newSolved = bomb.GetSolvedModuleNames().Where(x => !ignoredModules.Contains(x)).ToList();
            newSolvedCopy = bomb.GetSolvedModuleNames().Where(x => !ignoredModules.Contains(x)).ToArray();
            foreach (string module in oldSolved)
            {
                if (newSolved.Contains(module))
                {
                    newSolved.Remove(module);
                }
            }
            oldSolved = newSolvedCopy;
            Log("DEBUG: modulesLeft is " + modulesLeft + ".");
            if (modulesLeft == totalNonIgnored)
            {
                Solve("Done!");
            }
            else if (modulesLeft != 0)
            {
                Log("---");
                if (newSolved.Count(x => !ignoredModules.Contains(x)) != 0)
                {
                    lastSolvedModule = newSolved[0];
                    Log("The last solved module is " + lastSolvedModule + ".");
                }
            }
        }
    }
}
