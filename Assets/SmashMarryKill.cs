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
    private KMBombModule org = null;
    private string[] ignoredModules = new string[] { };

    private int currentIndex = 0;
    private bool firstClick = false;
    internal bool doneWithCategorization = false;
    private enum SMKwords
    {
        SMASH,
        MARRY,
        KILL
    }

    private static List<SMKwords> possibilities = new List<SMKwords>();
    private static SMKwords? currentSelection = null;
    private bool newSelectionChosen = false;

    private List<string> moduleNames = new List<string>();
    private List<string> usedModules = new List<string>();
    private static Dictionary<string, SMKwords> allModules = new Dictionary<string, SMKwords>();
    private List<string> newSolved = new List<string>();
    private string[] newSolvedCopy = new string[] { };
    private string lastSolvedModule = "";
    private string[] oldSolved = new string[] { };

    internal int modulesSolved = -1;
    internal int totalNonIgnored = -1;
    private int orgIndex = -1;
    private KMBombModule[] allMMs;
    private List<string> allHidden = new List<string>();
    private int numberHiddenByMM = 0;
    private bool recursionPrevention = false;

    private static List<SmashMarryKill> allSMKs = new List<SmashMarryKill>();
    private bool mmHidingThis = false;
    private List<string> orgOrder = new List<string>();

    public void MysteryModuleHiding()
    {
        Log("This module is currently being hidden by Mystery Module.");
        mmHidingThis = true;
    }

    public void MysteryModuleRevealing()
    {
        Log("This module has been revealed by Mystery Module.");
        mmHidingThis = false;
    }

    private IEnumerator WaitForBosses()
    {
        yield return new WaitForSeconds(.1f);
        orgIndex = transform.parent.GetComponentsInChildren<KMBombModule>().IndexOf(m => m.ModuleDisplayName == "Organization");
        allMMs = transform.parent.GetComponentsInChildren<KMBombModule>().Where(m => m.ModuleDisplayName == "Mystery Module").ToArray();
        allSMKs = transform.parent.GetComponentsInChildren<SmashMarryKill>().ToList();
        if (orgIndex > -1)
        {
            org = transform.parent.GetComponentsInChildren<KMBombModule>()[orgIndex];
        }
        orgOrder = org != null ? org.GetComponent("OrganizationScript").GetValue<List<string>>("order") : new List<string>() { };
        if (allMMs.Length > 0)
        {
            foreach (KMBombModule MM in allMMs)
            {
                allHidden.Add(MM.GetComponent("MysteryModuleScript").GetValue<KMBombModule>("mystifiedModule").ModuleDisplayName);
            }
        }
        Setup();
    }

    protected override void OnActivate()
    {
        allModules.Clear();
        possibilities.Clear();
        allSMKs.Clear();
        currentSelection = null;
        StartCoroutine(WaitForBosses());
    }

    private void CandidateSync()
    {
        try
        {
            for (int i = 1; i < allSMKs.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (allSMKs[i].candidates[j].gameObject.activeSelf)
                    {
                        allSMKs[i].candidates[j].GetComponentInChildren<TextMesh>().color = allSMKs[0].candidates[j].GetComponentInChildren<TextMesh>().color;
                        allSMKs[i].candidates[j].GetComponentInChildren<TextMesh>().text = allSMKs[0].candidates[j].GetComponentInChildren<TextMesh>().text;
                    }
                }
                allSMKs[i].result.text = allSMKs[0].result.text;
            }
        }
        catch (NullReferenceException)
        {
            return;
        }
    }

    private void SingleSync(int index)
    {
        foreach (SmashMarryKill smk in allSMKs)
        {
            if (smk != this)
            {
                smk.recursionPrevention = true;
                smk.candidates[index].OnInteract();
                smk.recursionPrevention = false;
            }
        }
    }

    private void Setup()
    {
        ignoredModules = boss.GetIgnoredModules("Smash, Marry, Kill", new string[] { "Smash, Marry, Kill", "Mystery Module", "Organization" });
        totalNonIgnored = bomb.GetSolvableModuleNames().Count(x => !ignoredModules.Contains(x));
        moduleNames = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).Distinct().ToList();
        foreach (string module in allHidden)
        {
            if (bomb.GetSolvableModuleNames().Count(m => m == module) == allHidden.Count(m => m == module))
            {
                moduleNames.Remove(module);
            }
        }
        TextMesh[] candidateTexts = candidates.Select(x => x.GetComponentInChildren<TextMesh>()).ToArray();
        if (moduleNames.Count == 0)
        {
            candidateTexts[0].text = "";
            candidateTexts[0].gameObject.SetActive(false);
            candidateTexts[1].fontSize = 90;
            candidateTexts[1].text = "SOLVE";
            candidates[1].Highlight.transform.localScale = new Vector3(8, 1, 8);
            candidateTexts[2].text = "";
            candidateTexts[2].gameObject.SetActive(false);
            Get<KMSelectable>().Children = new[] { candidates[1] };
            Get<KMSelectable>().UpdateChildrenProperly();
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
                    moduleName.text = moduleNames.PickRandom();
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
                if (!recursionPrevention)
                {
                    candidate.AddInteractionPunch(.1f);
                    Play(Sound.BigButtonPress);
                }
                if (moduleName.color == Color.white)
                {
                    Log("Categorized " + moduleName.text + " as " + result.text + ".");
                    moduleName.color = Color.green;
                    moduleNames.Remove(moduleName.text);
                    if (!allModules.ContainsKey(moduleName.text))
                    {
                        allModules.Add(moduleName.text, (SMKwords)currentIndex);
                    }
                    if (currentIndex == 2 && firstClick && moduleNames.Count > 0)
                    {
                        foreach (KMSelectable candidate_ in candidates)
                        {
                            TextMesh moduleName_ = candidate_.GetComponentInChildren<TextMesh>();
                            if ((candidate_.name == "secondCandidate" && moduleNames.Count == 1) || (candidate_.name == "thirdCandidate" && (moduleNames.Count == 1 || moduleNames.Count == 2)))
                            {
                                moduleName_.text = "";
                                candidate_.gameObject.SetActive(false);
                                continue;
                            }
                            if (moduleNames.Count != 1)
                            {
                                do
                                {
                                    moduleName_.text = moduleNames.PickRandom();
                                } while ((candidate_.name == "secondCandidate" && candidates[0].GetComponentInChildren<TextMesh>().text == moduleName_.text) || (candidate_.name == "thirdCandidate" && (candidates[0].GetComponentInChildren<TextMesh>().text == moduleName_.text || candidates[1].GetComponentInChildren<TextMesh>().text == moduleName_.text)));
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
                    if (!recursionPrevention)
                    {
                        SingleSync(Array.IndexOf(candidates, candidate));
                    }
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
                        List<string> mods = bomb.GetSolvableModuleNames().Where(x => !ignoredModules.Contains(x)).ToList();
                        foreach (string module in allHidden)
                        {
                            if (bomb.GetSolvableModuleNames().Count(m => m == module) == 1)
                            {
                                mods.Remove(module);
                            }
                        }
                        foreach (KeyValuePair<string, SMKwords> module in allModules)
                        {
                            numberHiddenByMM = allHidden.Contains(module.Key) ? 1 : 0;
                            for (int i = 0; i < mods.Count(x => x == module.Key && !bomb.GetSolvedModuleNames().Contains(x)) - numberHiddenByMM; i++)
                            {
                                if (this == allSMKs[0])
                                {
                                    possibilities.Add(module.Value);
                                }
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
        bool ignore = false;
        if (allHidden.Contains(lastSolved) && bomb.GetSolvedModuleNames().Count(x => x == lastSolved) == bomb.GetSolvableModuleNames().Count(x => x == lastSolved))
        {
            Log("Solved the last instance of " + lastSolved + ", which was hidden by Mystery Module.");
            ignore = true;
        }
        if (!ignore)
        {
            if (lastSolved != "" && allModules.ContainsKey(lastSolved) && allModules[lastSolved] != currentSelection && !mmHidingThis && this == allSMKs[0])
            {
                if (org == null)
                {
                    Strike("STRIKE! Solved " + lastSolved + ", a " + allModules[lastSolved] + " module when a " + currentSelection + " module was supposed to be solved.");
                }
                else
                {
                    Log("STRIKE (from Organization)! Solved " + lastSolved + ", a " + allModules[lastSolved] + " module when a " + currentSelection + " module was supposed to be solved.");
                }
            }
            if (allModules.ContainsKey(lastSolved) && this == allSMKs[0])
            {
                possibilities.Remove(allModules[lastSolved]);
            }
            if (lastSolved == "" && modulesSolved == totalNonIgnored && !mmHidingThis)
            {
                candidates[0].gameObject.SetActive(false);
                candidates[1].gameObject.SetActive(true);
                candidates[2].gameObject.SetActive(false);
                Get<KMSelectable>().Children = new[] { candidates[1] };
                Get<KMSelectable>().UpdateChildrenProperly();
                candidates[1].Highlight.transform.localScale = new Vector3(8, 1, 8);
                result.text = "SOLVE";
                candidates[1].Set(onInteract: () =>
                {
                    Solve("Solved all non-ignored modules before categorization was finished.");
                    result.text = "DONE";
                    candidates[1].gameObject.SetActive(false);
                });
                return;
            }
        }
        if (orgOrder.Count == 0)
        {
            if (((allHidden.Contains(lastSolved) && bomb.GetSolvedModuleNames().Count(x => x == lastSolved) != bomb.GetSolvableModuleNames().Count(x => x == lastSolved)) || !allHidden.Contains(lastSolved)) && possibilities.Count > 0)
            {
                StartCoroutine(SyncCurrentSelection());
            }
        }
        else
        {
            StartCoroutine(OrgUpdateList());
        }
    }

    private IEnumerator SyncCurrentSelection()
    {
        if (this == allSMKs[0])
        {
            currentSelection = possibilities.PickRandom();
            yield return new WaitForSeconds(.05f);
            newSelectionChosen = true;
            yield return new WaitForSeconds(.05f);
            newSelectionChosen = false;
        }
        else
        {
            yield return new WaitUntil(() => allSMKs[0].newSelectionChosen);
        }
        Log("Current selection: " + currentSelection);
        string currentlySolvable = string.Join(", ", allModules.Where(pair => pair.Value == currentSelection && bomb.GetSolvedModuleNames().Count(x => x == pair.Key) != bomb.GetSolvableModuleNames().Count(x => x == pair.Key)).Select(pair => pair.Key).ToArray());
        Log("You are allowed to solve any of the following: " + currentlySolvable + ".");
        result.text = "" + currentSelection;
    }

    private IEnumerator OrgUpdateList()
    {
        List<string> orgOrderCopy = new List<string>(orgOrder);
        if (bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x)) > 0)
        {
            yield return new WaitWhile(() => orgOrder[0] == orgOrderCopy[0]);
        }
        if (allModules.ContainsKey(orgOrder[0]))
        {
            currentSelection = allModules[orgOrder[0]];
            Log("Current selection: " + currentSelection);
            Log("Organization is requiring " + orgOrder[0] + " to be solved.");
        }
        result.text = "" + currentSelection;

    }

    void Update()
    {
        if (!doneWithCategorization && allSMKs.Count > 0 && this == allSMKs[0])
        {
            CandidateSync();
        }
        if (modulesSolved != bomb.GetSolvedModuleNames().Count(x => !ignoredModules.Contains(x)) && doneWithCategorization)
        {
            modulesSolved++;
            newSolved = bomb.GetSolvedModuleNames().Where(x => !ignoredModules.Contains(x)).ToList();
            newSolvedCopy = newSolved.ToArray();
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
                Log("---");
                if (newSolved.Count(x => !ignoredModules.Contains(x)) != 0)
                {
                    lastSolvedModule = newSolved[0];
                    Log("The last solved module is " + lastSolvedModule + ".");
                }
                Log("Smash, Marry, Kill is now ready to be solved.");
                candidates[0].gameObject.SetActive(false);
                candidates[1].gameObject.SetActive(true);
                candidates[2].gameObject.SetActive(false);
                Get<KMSelectable>().Children = new[] { candidates[1] };
                Get<KMSelectable>().UpdateChildrenProperly();
                candidates[1].Highlight.transform.localScale = new Vector3(8, 1, 8);
                result.text = "SOLVE";
                candidates[1].Set(onInteract: () =>
                {
                    Solve("Done!");
                    result.text = "DONE";
                    candidates[1].gameObject.SetActive(false);
                });

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
            newSolved = bomb.GetSolvedModuleNames().Where(x => !ignoredModules.Contains(x)).ToList();
            newSolvedCopy = newSolved.ToArray();
            foreach (string module in oldSolved)
            {
                if (newSolved.Contains(module))
                {
                    newSolved.Remove(module);
                }
            }
            oldSolved = newSolvedCopy;
            if (newSolved.Count(x => !ignoredModules.Contains(x)) != 0)
            {
                lastSolvedModule = newSolved[0];
            }
            if (modulesSolved != 0)
            {
                if (mmHidingThis)
                {
                    Log(lastSolvedModule + " was solved while Smash, Marry, Kill was hidden by Mystery Module.");
                }
                else if (this == allSMKs.First(x => !x.mmHidingThis) && !(allHidden.Contains(lastSolvedModule) && bomb.GetSolvedModuleNames().Count(x => x == lastSolvedModule) == bomb.GetSolvableModuleNames().Count(x => x == lastSolvedModule)))
                {
                    Strike("STRIKE! Solved " + lastSolvedModule + " before categorization was finished.");
                }
            }
        }
    }
}
