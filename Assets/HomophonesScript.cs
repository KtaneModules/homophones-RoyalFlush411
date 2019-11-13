using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public static class Extensions
{
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class HomophonesScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable cycleButton;
    public KMSelectable[] mainButtons;
    public KMSelectable[] numbersUp;
    public KMSelectable[] numbersDown;
    public Material[] buttonColourOptions;
    public Renderer[] buttonRenderer;
    public ButtonNumber[] buttonInformation;

    private String[] iWords = new String[10] {"i", "I", "aye", "ay", "eye", "high", "hi", "aye-aye", "eye-eye", "ii"};
    private String[] lWords = new String[10] {"L", "l", "el", "ell", "hell", "lema", "lima", "leaner", "leemer", "lemur"};
    private String[] cWords = new String[10] {"C", "ce", "se", "see", "sea", "sees", "seas", "say", "she", "icy"};
    private String[] oneWords = new String[10] {"1", "One", "one", "won", "wun", "run", "on", "un", "win", "wan"};

    public String[] selectedWords = new String[4];

    private int[] wordIndices = new int[4];
    public TextMesh screenText;
    public Renderer[] screenIndicators;
    public Material[] indicatorMats;
    private int currentDisplay = 0;

    private int[] buttonNumbers = new int[4];
    public TextMesh[] buttonTextMesh;
    public String[] correctButtonLabel = new String[4];
    public KMSelectable[] correctButton = new KMSelectable[4];

    private List<int> selectedIndices = new List<int>();
    private bool correct = true;
    private int stage = 0;
    private bool numbersSet;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in mainButtons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
        }
        foreach (KMSelectable button in numbersUp)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { UpPress(pressedButton); return false; };
        }
        foreach (KMSelectable button in numbersDown)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { DownPress(pressedButton); return false; };
        }
        cycleButton.OnInteract += delegate () { CyclePress(); return false; };
    }


    void Start()
    {
        SelectWords();
        SetScreen();
        if(buttonNumbers[0].ToString() == correctButtonLabel[0] && buttonNumbers[1].ToString() == correctButtonLabel[1] && buttonNumbers[2].ToString() == correctButtonLabel[2] && buttonNumbers[3].ToString() == correctButtonLabel[3])
        {
            numbersSet = true;
            Debug.LogFormat("[Homophones #{0}] The button labels have been set correctly.", moduleId);
        }
    }

    void SelectWords()
    {
        for(int i = 0; i < 4; i++)
        {
            wordIndices[i] = UnityEngine.Random.Range(0,10);
        }
        selectedWords[0] = iWords[wordIndices[0]] + wordIndices[0].ToString();
        selectedWords[1] = lWords[wordIndices[1]] + wordIndices[1].ToString();
        selectedWords[2] = cWords[wordIndices[2]] + wordIndices[2].ToString();
        selectedWords[3] = oneWords[wordIndices[3]] + wordIndices[3].ToString();
        selectedWords.Shuffle();
        for(int i = 0; i < 4; i++)
        {
            correctButtonLabel[i] = selectedWords[i].Last().ToString();
            selectedWords[i] = selectedWords[i].Substring(0, selectedWords[i].Length - 1);
        }
        Debug.LogFormat("[Homophones #{0}] The selected words in stage order are: {1}.", moduleId, string.Join(", ", selectedWords.Select((x) => x).ToArray()));
    }

    void SetScreen()
    {
        foreach(Renderer indic in screenIndicators)
        {
            indic.material = indicatorMats[0];
        }
        screenIndicators[0].material = indicatorMats[1];
        screenText.text = selectedWords[0];
        currentDisplay = 0;
        for(int i = 0; i < 4; i++)
        {
            buttonNumbers[i] = 0;
            buttonTextMesh[i].text = buttonNumbers[i].ToString();
        }
        for(int i = 0; i < 4; i++)
        {
            int index = UnityEngine.Random.Range(0,4);
            while(selectedIndices.Contains(index))
            {
                index = UnityEngine.Random.Range(0,4);
            }
            selectedIndices.Add(index);
            buttonRenderer[i].material = buttonColourOptions[index];
            buttonInformation[i].buttonColour = buttonColourOptions[index].name;
        }
        selectedIndices.Clear();
        Debug.LogFormat("[Homophones #{0}] The button colours from left to right are: {1}.", moduleId, string.Join(", ", buttonInformation.Select((x) => x.buttonColour).ToArray()));
        Debug.LogFormat("[Homophones #{0}] The correct button labels from left to right are: {1}.", moduleId, string.Join(", ", correctButtonLabel.Select((x) => x).ToArray()));

        for(int i = 0; i < 4; i++)
        {
            if(selectedWords[i] == "i" || selectedWords[i] == "I" || selectedWords[i] == "aye" || selectedWords[i] == "ay" || selectedWords[i] == "eye" || selectedWords[i] == "high" || selectedWords[i] == "hi" || selectedWords[i] == "aye-aye" || selectedWords[i] == "eye-eye" || selectedWords[i] == "ii")
            {
                for(int j = 0; j < 4; j++)
                if(buttonInformation[j].buttonColour == "red")
                {
                    correctButton[i] = mainButtons[j];
                }
            }
            else if(selectedWords[i] == "L" || selectedWords[i] == "l" || selectedWords[i] == "el" || selectedWords[i] == "ell" || selectedWords[i] == "hell" || selectedWords[i] == "lema" || selectedWords[i] == "lima" || selectedWords[i] == "leaner" || selectedWords[i] == "leemer" || selectedWords[i] == "lemur")
            {
                for(int j = 0; j < 4; j++)
                if(buttonInformation[j].buttonColour == "green")
                {
                    correctButton[i] = mainButtons[j];
                }
            }
            else if(selectedWords[i] == "C" || selectedWords[i] == "ce" || selectedWords[i] == "se" || selectedWords[i] == "see" || selectedWords[i] == "sea" || selectedWords[i] == "sees" || selectedWords[i] == "seas" || selectedWords[i] == "say" || selectedWords[i] == "she" || selectedWords[i] == "icy")
            {
                for(int j = 0; j < 4; j++)
                if(buttonInformation[j].buttonColour == "blue")
                {
                    correctButton[i] = mainButtons[j];
                }
            }
            else
            {
                for(int j = 0; j < 4; j++)
                if(buttonInformation[j].buttonColour == "yellow")
                {
                    correctButton[i] = mainButtons[j];
                }
            }
        }
        Debug.LogFormat("[Homophones #{0}] After setting the numbers, press the buttons in this order: {1}.", moduleId, string.Join(", ", correctButton.Select((x) => x.GetComponent<ButtonNumber>().buttonColour).ToArray()));
    }

    public void CyclePress()
    {
        if(moduleSolved)
        {
            return;
        }
        cycleButton.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        currentDisplay++;
        currentDisplay = currentDisplay % 4;
        foreach(Renderer indic in screenIndicators)
        {
            indic.material = indicatorMats[0];
        }
        screenIndicators[currentDisplay].material = indicatorMats[1];
        screenText.text = selectedWords[currentDisplay];
    }

    public void ButtonPress(KMSelectable pressedButton)
    {
        if(moduleSolved || pressedButton.GetComponent<ButtonNumber>().pressed)
        {
            return;
        }
        pressedButton.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if(!numbersSet)
        {
            foreach(ButtonNumber button in buttonInformation)
            {
                button.GetComponent<ButtonNumber>().pressed = false;
            }
            correct = true;
            Debug.LogFormat("[Homophones #{0}] Strike! The button labels were set to {1}. The should have been set to {2}. Module reset.", moduleId, string.Join(", ", buttonNumbers.Select((x) => x.ToString()).ToArray()), string.Join(", ", correctButtonLabel.Select((x) => x).ToArray()));
            GetComponent<KMBombModule>().HandleStrike();
            Start();
            return;
        }

        pressedButton.GetComponent<ButtonNumber>().pressed = true;
        buttonTextMesh[pressedButton.GetComponent<ButtonNumber>().buttonNumber].text = "";
        if(pressedButton == correctButton[stage])
        {
            Debug.LogFormat("[Homophones #{0}] You pressed {1}. That is correct.", moduleId, pressedButton.GetComponent<ButtonNumber>().buttonColour);
        }
        else
        {
            Debug.LogFormat("[Homophones #{0}] You pressed {1}. That is incorrect. The module will strike upon completion of the sequence.", moduleId, pressedButton.GetComponent<ButtonNumber>().buttonColour);
            correct = false;
        }
        stage++;
        if(stage == 4)
        {
            stage = 0;
            if(correct)
            {
                Debug.LogFormat("[Homophones #{0}] Module disarmed.", moduleId);
                moduleSolved = true;
                GetComponent<KMBombModule>().HandlePass();
                for(int i = 0; i < 4; i++)
                {
                    screenIndicators[i].material = indicatorMats[1];
                    buttonTextMesh[i].text = "";
                }
                screenText.text = "";
            }
            else
            {
                foreach(ButtonNumber button in buttonInformation)
                {
                    button.GetComponent<ButtonNumber>().pressed = false;
                }
                Debug.LogFormat("[Homophones #{0}] Strike! You did not enter the sequence correctly. Module reset.", moduleId);
                GetComponent<KMBombModule>().HandleStrike();
                numbersSet = false;
                correct = true;
                Start();
            }
        }
    }

    public void UpPress(KMSelectable pressedButton)
    {
        if(moduleSolved)
        {
            return;
        }
        pressedButton.AddInteractionPunch(0.5f);
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        int pressedNumber = pressedButton.GetComponent<ButtonNumber>().buttonNumber - 1;
        buttonNumbers[pressedNumber] = (buttonNumbers[pressedNumber] + 1) % 10;
        buttonTextMesh[pressedNumber].text = buttonNumbers[pressedNumber].ToString();
        CheckNumbers();
    }

    public void DownPress(KMSelectable pressedButton)
    {
        if(moduleSolved)
        {
            return;
        }
        pressedButton.AddInteractionPunch(0.5f);
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        int pressedNumber = pressedButton.GetComponent<ButtonNumber>().buttonNumber - 1;
        buttonNumbers[pressedNumber] = (buttonNumbers[pressedNumber] + 9) % 10;
        buttonTextMesh[pressedNumber].text = buttonNumbers[pressedNumber].ToString();
        CheckNumbers();
    }

    void CheckNumbers()
    {
        if(buttonNumbers[0].ToString() == correctButtonLabel[0] && buttonNumbers[1].ToString() == correctButtonLabel[1] && buttonNumbers[2].ToString() == correctButtonLabel[2] && buttonNumbers[3].ToString() == correctButtonLabel[3])
        {
            numbersSet = true;
            Debug.LogFormat("[Homophones #{0}] The button labels have been set correctly.", moduleId);
        }
        else
        {
            if(numbersSet)
            {
                Debug.LogFormat("[Homophones #{0}] The button labels are no longer set correctly.", moduleId);
            }
            numbersSet = false;
        }
    }

    //twitch plays
    private bool setsAlright(string s1, string s2, string s3, string s4)
    {
        string[] numbers = {"0","1","2","3","4","5","6","7","8","9"};
        string[] strings = { s1, s2, s3, s4 };
        for(int i = 0; i < 4; i++)
        {
            if (!numbers.Contains(strings[i]))
            {
                return false;
            }
        }
        return true;
    }
    private bool pressAlright(string s1, string s2, string s3, string s4)
    {
        string[] numbers = { "1", "2", "3", "4" };
        string[] strings = { s1, s2, s3, s4 };
        for (int i = 0; i < 4; i++)
        {
            if (!numbers.Contains(strings[i]))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} cycle [Cycles through all 4 words] | !{0} set <btn> <#> [Sets the specified button's number to '#'] | !{0} set all <#1> <#2> <#3> <#4> [Sets all buttons numbers to '#1' through '#4' from left to right] | !{0} press <btn>... [Presses the specified button(s)] | Valid buttons are 1-4 with 1 leftmost and 4 rightmost";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*cycle\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 1)
            {
                yield return null;
                yield return new WaitForSeconds(0.5f);
                cycleButton.OnInteract();
                for(int i = 0; i < 3; i++)
                {
                    yield return "trycancel Word cycling cancelled due to a cancel request.";
                    yield return new WaitForSeconds(2f);
                    cycleButton.OnInteract();
                }
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 6)
            {
                if (Regex.IsMatch(parameters[1], @"^\s*all\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    if (setsAlright(parameters[2], parameters[3], parameters[4], parameters[5]))
                    {
                        yield return null;
                        while (!buttonTextMesh[0].text.Equals(parameters[2])) { yield return new WaitForSeconds(0.1f); numbersUp[0].OnInteract(); }
                        while (!buttonTextMesh[1].text.Equals(parameters[3])) { yield return new WaitForSeconds(0.1f); numbersUp[1].OnInteract(); }
                        while (!buttonTextMesh[2].text.Equals(parameters[4])) { yield return new WaitForSeconds(0.1f); numbersUp[2].OnInteract(); }
                        while (!buttonTextMesh[3].text.Equals(parameters[5])) { yield return new WaitForSeconds(0.1f); numbersUp[3].OnInteract(); }
                    }
                }
            }
            else if(parameters.Length == 3)
            {
                if(pressAlright(parameters[1], "1", "1", "1") && setsAlright(parameters[2], "0", "0", "0"))
                {
                    yield return null;
                    int button = 0;
                    int.TryParse(parameters[1], out button);
                    button -= 1;
                    while (!buttonTextMesh[button].text.Equals(parameters[2])) { yield return new WaitForSeconds(0.1f); numbersUp[button].OnInteract(); }
                }
            }
            yield break;
        }
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length == 5)
            {
                if (pressAlright(parameters[1], parameters[2], parameters[3], parameters[4]))
                {
                    yield return null;
                    for(int i = 1; i < 5; i++)
                    {
                        if (parameters[i].Equals("1"))
                        {
                            mainButtons[0].OnInteract();
                        }
                        else if (parameters[i].Equals("2"))
                        {
                            mainButtons[1].OnInteract();
                        }
                        else if (parameters[i].Equals("3"))
                        {
                            mainButtons[2].OnInteract();
                        }
                        else if (parameters[i].Equals("4"))
                        {
                            mainButtons[3].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (parameters.Length == 4)
            {
                if (pressAlright(parameters[1], parameters[2], parameters[3], "1"))
                {
                    yield return null;
                    for (int i = 1; i < 4; i++)
                    {
                        if (parameters[i].Equals("1"))
                        {
                            mainButtons[0].OnInteract();
                        }
                        else if (parameters[i].Equals("2"))
                        {
                            mainButtons[1].OnInteract();
                        }
                        else if (parameters[i].Equals("3"))
                        {
                            mainButtons[2].OnInteract();
                        }
                        else if (parameters[i].Equals("4"))
                        {
                            mainButtons[3].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (parameters.Length == 3)
            {
                if (pressAlright(parameters[1], parameters[2], "1", "1"))
                {
                    yield return null;
                    for (int i = 1; i < 3; i++)
                    {
                        if (parameters[i].Equals("1"))
                        {
                            mainButtons[0].OnInteract();
                        }
                        else if (parameters[i].Equals("2"))
                        {
                            mainButtons[1].OnInteract();
                        }
                        else if (parameters[i].Equals("3"))
                        {
                            mainButtons[2].OnInteract();
                        }
                        else if (parameters[i].Equals("4"))
                        {
                            mainButtons[3].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (parameters.Length == 2)
            {
                if (pressAlright(parameters[1], "1", "1", "1"))
                {
                    yield return null;
                    for (int i = 1; i < 2; i++)
                    {
                        if (parameters[i].Equals("1"))
                        {
                            mainButtons[0].OnInteract();
                        }
                        else if (parameters[i].Equals("2"))
                        {
                            mainButtons[1].OnInteract();
                        }
                        else if (parameters[i].Equals("3"))
                        {
                            mainButtons[2].OnInteract();
                        }
                        else if (parameters[i].Equals("4"))
                        {
                            mainButtons[3].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            yield break;
        }
    }
}
