using Wawa.Extensions;
using Wawa.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private SMKwords? currentSelection = null;

    private List<string> moduleNames = new List<string>();
    private List<string> usedModules = new List<string>();
    private Dictionary<string, SMKwords> allModules = new Dictionary<string, SMKwords>();
    private List<string> newSolved = new List<string>();
    private string[] newSolvedCopy = new string[] { };
    private string lastSolvedModule = "";
    private string[] oldSolved = new string[] { };

    private int modulesSolved = -1;
    private int totalNonIgnored = -1;

    protected void Start()
    {
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
        totalNonIgnored = bomb.GetSolvableModuleNames().Count(x => !ignoredModules.Contains(x));
        moduleNames = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Distinct().ToList();
        TextMesh[] candidateTexts = candidates.Select(x => x.GetComponentInChildren<TextMesh>()).ToArray();
        if (moduleNames.Count == 0)
        {
            Get<KMSelectable>().Children = new[] { candidates[1] };
            Get<KMSelectable>().UpdateChildrenProperly();
            candidateTexts[0].text = "";
            candidateTexts[0].gameObject.SetActive(false);
            candidateTexts[1].fontSize = 90;
            candidateTexts[1].text = "SOLVE";
            candidates[1].Highlight.transform.localScale = new Vector3(8, 1, 8);
            candidateTexts[2].text = "";
            candidateTexts[2].gameObject.SetActive(false);
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
            if (moduleNames.Count > 2)
            {
                do
                {
                    moduleName.text = moduleNames[UnityEngine.Random.Range(0, moduleNames.Count)];
                } while (usedModules.Contains(moduleName.text));
            }
            else
            {
                moduleName.text = Array.IndexOf(candidates, candidate) < moduleNames.Count ? moduleNames[Array.IndexOf(candidates, candidate)] : "";
                if (moduleName.text == "")
                {
                    List<KMSelectable> children = Get<KMSelectable>().Children.ToList();
                    children.Remove(candidate);
                    Get<KMSelectable>().Children = children.ToArray();
                    Get<KMSelectable>().UpdateChildrenProperly();
                    candidate.gameObject.SetActive(false);
                }
            }
            usedModules.Add(moduleName.text);
            candidate.Set(onInteract: () =>
            {
                if (moduleName.color == Color.white)
                {
                    Log("Categorized " + moduleName.text + " as " + result.text + ".");
                    moduleName.color = Color.green;
                    moduleNames.Remove(moduleName.text);
                    allModules.Add(moduleName.text, (SMKwords)currentIndex);
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
                    currentIndex++;
                    currentIndex %= 3;
                    result.text = "" + (SMKwords)currentIndex;
                    if (moduleNames.Count == 0)
                    {
                        if (allModules.Where(pair => pair.Value == SMKwords.SMASH).Select(pair => pair.Key).Count() == 0)
                        {
                            Log("No modules have been categorized under SMASH.");
                        }
                        else
                        {
                            Log("The SMASH modules are " + string.Join(", ", allModules.Where(pair => pair.Value == SMKwords.SMASH).Select(pair => pair.Key).ToArray()) + ".");
                        }
                        if (allModules.Where(pair => pair.Value == SMKwords.MARRY).Select(pair => pair.Key).Count() == 0)
                        {
                            Log("No modules have been categorized under MARRY.");
                        }
                        else
                        {
                            Log("The MARRY modules are " + string.Join(", ", allModules.Where(pair => pair.Value == SMKwords.MARRY).Select(pair => pair.Key).ToArray()) + ".");
                        }
                        if (allModules.Where(pair => pair.Value == SMKwords.KILL).Select(pair => pair.Key).Count() == 0)
                        {
                            Log("No modules have been categorized under KILL.");
                        }
                        else
                        {
                            Log("The KILL modules are " + string.Join(", ", allModules.Where(pair => pair.Value == SMKwords.KILL).Select(pair => pair.Key).ToArray()) + ".");
                        }
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            candidate_.GetComponentInChildren<TextMesh>().text = "";
                        }
                        string[] mods = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).ToArray();
                        foreach (KeyValuePair<string, SMKwords> module in allModules)
                        {
                            for (int i = 0; i < mods.Count(x => x == module.Key && !bomb.GetSolvedModuleNames().Contains(x)); i++)
                            {
                                possibilities.Add(module.Value);
                            }
                        }
                        Get<KMSelectable>().Children = new KMSelectable[] { };
                        Get<KMSelectable>().UpdateChildrenProperly();
                        candidateTexts[0].gameObject.SetActive(false);
                        candidateTexts[1].gameObject.SetActive(false);
                        candidateTexts[2].gameObject.SetActive(false);
                        doneWithCategorization = true;
                        result.fontSize = 90;
                        result.transform.localPosition = new Vector3(0, 0.0106f, 0);
                        SMKselect("");
                    }
                }
            });
        }
    }

    private void SMKselect(string lastSolved)
    {
        if (lastSolved != "" && allModules.ContainsKey(lastSolved) && allModules[lastSolved] != currentSelection)
        {
            Strike("STRIKE! Solved " + lastSolved + ", a " + allModules[lastSolved] + " module when a " + currentSelection + " module was meant to be solved.");
        }
        if (allModules.ContainsKey(lastSolved))
        {
            possibilities.Remove(allModules[lastSolved]);
        }
        currentSelection = possibilities.PickRandom();
        result.text = "" + currentSelection;
        Log("Current selection: " + currentSelection);
        string currentlySolvable = string.Join(", ", allModules.Where(pair => pair.Value == currentSelection && bomb.GetSolvedModuleNames().Count(x => x == pair.Key) != bomb.GetSolvableModuleNames().Count(x => x == pair.Key)).Select(pair => pair.Key).ToArray());
        Log("You are allowed to solve any of the following: " + currentlySolvable + ".");
    }

    void Update()
    {
        if (modulesSolved != bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x)) && doneWithCategorization)
        {
            modulesSolved++;
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
            if (modulesSolved == totalNonIgnored)
            {
                Solve("Done!");
                result.text = "DONE";
            }
            else if (modulesSolved != 0)
            {
                Log("---");
                if (newSolved.Count(x => !ignoredModules.Contains(x)) != 0)
                {
                    lastSolvedModule = newSolved[0];
                    Log("The last solved module is " + lastSolvedModule + ".");
                    SMKselect(lastSolvedModule);
                }
            }
        }
        else if (modulesSolved != bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x)))
        {
            modulesSolved++;
            if (modulesSolved != 0)
            {
                Strike("STRIKE! Solved a module before categorization was finished.");
            }
        }
    }
}
