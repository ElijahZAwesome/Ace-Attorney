using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    bool coroutineRunning = false;
    bool justSwitched = false;
    bool richTextSecondaryActive;
    bool richTextActive;
    string richTextCloseString;
    string richTextSecondaryCloseString;
    string richTextSecondaryString;
    string preDialogueString;
    string richTextString;
    int blipDelay;
    int currentBlip;
    public Text currentDlgText;
    DialogueParser parser;
    public GameObject Player;
    public GameObject Client;
    public GameObject ScreenFade;
    bool silentText;
    string currentDialogue;
    public Animator currentDlgBoxAnim;
    public AudioSource source;
    public AudioClip currentDlgBlip;
    public AudioClip[] sounds;
    bool centeredText;

    public string dialogue, characterName;
    public int lineNum;
    int pose;
    string position;
    string[] options;
    public bool playerTalking;
    List<Button> buttons = new List<Button>();

    public Text dialogueBox;
    public Text nameBox;
    public GameObject choiceBox;

    // Use this for initialization
    void Start()
    {
        dialogue = "";
        characterName = "";
        pose = 0;
        position = "L";
        playerTalking = false;
        parser = GameObject.Find("DialogueParser").GetComponent<DialogueParser>();
        lineNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentDialogue = dialogue;
        if (Input.GetMouseButtonDown(0) && playerTalking == false)
        {
            ShowDialogue();

            lineNum++;
        }

        UpdateUI();
    }

    public void ShowDialogue()
    {
        ResetImages();
        ParseLine();
    }

    void UpdateUI()
    {
        if (!playerTalking)
        {
            ClearButtons();
        }
        dialogueBox.text = dialogue;
        nameBox.text = characterName;
    }

    void ClearButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            print("Clearing buttons");
            Button b = buttons[i];
            buttons.Remove(b);
            Destroy(b.gameObject);
        }
    }

    void ParseLine()
    {
        if (parser.GetName(lineNum) != "Player")
        {
            playerTalking = false;
            characterName = parser.GetName(lineNum);
            dialogue = parser.GetContent(lineNum);
            pose = parser.GetPose(lineNum);
            position = parser.GetPosition(lineNum);
            DisplayImages();
        }
        else
        {
            playerTalking = true;
            characterName = "";
            dialogue = "";
            pose = 0;
            position = "";
            options = parser.GetOptions(lineNum);
            CreateButtons();
        }
    }

    void CreateButtons()
    {
        for (int i = 0; i < options.Length; i++)
        {
            GameObject button = (GameObject)Instantiate(choiceBox);
            Button b = button.GetComponent<Button>();
            ChoiceButton cb = button.GetComponent<ChoiceButton>();
            cb.SetText(options[i].Split(':')[0]);
            cb.option = options[i].Split(':')[1];
            cb.box = this;
            b.transform.SetParent(this.transform);
            b.transform.localPosition = new Vector3(0, -25 + (i * 50));
            b.transform.localScale = new Vector3(1, 1, 1);
            buttons.Add(b);
        }
    }

    void ResetImages()
    {
        if (characterName != "")
        {
            GameObject character = GameObject.Find(characterName);
            SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
            currSprite.sprite = null;
        }
    }

    void DisplayImages()
    {
        if (characterName != "")
        {
            GameObject character = GameObject.Find(characterName);

            SetSpritePositions(character);

            SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
            currSprite.sprite = character.GetComponent<Character>().characterPoses[pose];
        }
    }


    void SetSpritePositions(GameObject spriteObj)
    {
        if (position == "L")
        {
            spriteObj.transform.position = new Vector3(-6, 0);
        }
        else if (position == "R")
        {
            spriteObj.transform.position = new Vector3(6, 0);
        }
        spriteObj.transform.position = new Vector3(spriteObj.transform.position.x, spriteObj.transform.position.y, 0);
    }
    /*
    private bool doesNextWordWrap(string textParagraph, string entireTextParagraph, int index)
    {
        textParagraph = RemoveBetween(textParagraph, "&&%%@@<>");
        string nextWord = (getNextWord(entireTextParagraph, index)) + "a";
        TextGenerationSettings generationSettings = currentDlgText.GetGenerationSettings(currentDlgText.rectTransform.rect.size);
        float originalTextHeight = currentDlgText.cachedTextGeneratorForLayout.GetPreferredHeight(textParagraph, generationSettings);
        float newTextHeight = currentDlgText.cachedTextGeneratorForLayout.GetPreferredHeight(textParagraph + nextWord, generationSettings);
        if (newTextHeight > originalTextHeight)
        {
            return true;
        }
        return false;
    }

    private string getNextWord(string entireTextParagraph, int index)
    {
        string textAfterParagraph = entireTextParagraph.Substring(index + 1);
        string[] nextWords = textAfterParagraph.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
        if (nextWords.Length > 0)
        {
            nextWords[0] = RemoveBetween(nextWords[0], "&&%%@@<>");
            return nextWords[0];
        }
        return "";
    }

    string RemoveBetween(string s, string delimiters)
    {
        for (int i = 0; i < delimiters.Length; i++)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", delimiters[i], delimiters[i + 1]));
            s = regex.Replace(s, string.Empty);
            i++;
        }
        return s;
    }


    IEnumerator TextUpdate()
    {
        float timeInSeconds = .02f;
        coroutineRunning = true;
        if (justSwitched)
        {
            yield return new WaitForSeconds(.2f);
            justSwitched = false;
        }
        for (int i = 0; i < dialogue.Length; i++)
        {
            switch (dialogue[i])
            {
                case '&':
                    string timeDelayString = processDlgCommand(ref i, '&');
                    timeInSeconds = float.Parse(timeDelayString);
                    if (timeInSeconds <= .1)
                    {
                        blipDelay = 2;
                        currentBlip = 2;
                    }
                    else
                    {
                        blipDelay = 0;
                        currentBlip = 0;
                    }
                    continue;
                case '<':
                    string comparisonString = processDlgCommand(ref i, '>');
                    if (richTextActive == true)
                    {
                        if (richTextSecondaryActive == true)
                        {
                            preDialogueString = richTextSecondaryCloseString;
                            richTextSecondaryString = string.Empty;
                            richTextSecondaryActive = false;
                        }
                        else if ("<" + comparisonString + ">" == richTextCloseString)
                        {
                            preDialogueString = richTextCloseString;
                            richTextString = string.Empty;
                            richTextCloseString = string.Empty;
                            richTextActive = false;
                        }
                        else
                        {
                            richTextSecondaryString = comparisonString;
                            processRichText(ref richTextSecondaryString, ref richTextSecondaryCloseString);
                            preDialogueString = richTextSecondaryString;
                            richTextSecondaryActive = true;
                        }
                    }
                    else
                    {
                        richTextString = comparisonString;
                        processRichText(ref richTextString, ref richTextCloseString);
                        preDialogueString = richTextString;
                        richTextSecondaryActive = true;
                    }
                    continue;
                case '%':
                    Animator animator = null;
                    string animString = processDlgCommand(ref i, '%');
                    string animActorString = animString.Split(':')[0];
                    switch (animActorString)
                    {
                        case "DialogueBox":
                            animator = currentDlgBoxAnim;
                            break;
                        case "Client":
                            animator = Client.GetComponent<Animator>();
                            break;
                        case "Player":
                            animator = Player.GetComponent<Animator>();
                            break;
                        case "ScreenFade":
                            animator = ScreenFade.GetComponent<Animator>();
                            ScreenFade.SetActive(true);
                            StartCoroutine(AnimationDelay(1f, ScreenFade));
                            break;
                    }
                    if (animator != null)
                    {
                        animator.SetTrigger(animString.Split(':')[1]);
                    }
                    continue;
                case '@':
                    string SFXString = processDlgCommand(ref i, '@');
                    int soundIndex = int.Parse(SFXString);
                    source.PlayOneShot(sounds[soundIndex]);
                    continue;
                case ' ':
                    if (doesNextWordWrap(currentDialogue, dialogue, i))
                    {
                        currentDialogue += "\n";
                        yield return new WaitForSeconds(timeInSeconds);
                        continue;
                    }
                    break;
                case '^':
                    currentDialogue += "\n";
                    continue;
                case '~':
                    silentText = !silentText;
                    continue;
                case '`':
                    if (centeredText == false)
                    {
                        centeredText = true;
                        currentDlgText.alignment = TextAnchor.UpperCenter;
                    }
                    else
                    {
                        centeredText = false;
                        currentDlgText.alignment = TextAnchor.UpperCenter;
                    }
                    continue;
            }
            currentDialogue += preDialogueString + dialogue[i];
            yield return new WaitForSeconds(timeInSeconds);
            currentDlgText.text = currentDialogue + richTextSecondaryCloseString + richTextCloseString;
            preDialogueString = string.Empty;
            if (!silentText)
            {
                if (currentBlip == 0)
                {
                    source.PlayOneShot(currentDlgBlip);
                    currentBlip = blipDelay;
                }
                else
                {
                    currentBlip--;
                }
            }
        }
        coroutineRunning = false;
        currentDlgBoxAnim.SetTrigger("ArrowFlash");
    }*/
}