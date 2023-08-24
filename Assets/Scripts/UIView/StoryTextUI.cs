using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoryTextUI : MonoBehaviour, IPointerUpHandler
{
    public delegate void StoryTextCompleted();
    public static event StoryTextCompleted OnStoryTextCompleted;

    public TextMeshProUGUI textStory;

    private StoryTextEventSO storyText;
    private bool isActive;
    private int currentTextShown;

    private float preventMouseClickCooldown; // prevent mouse click in the first x (0.5) seconds

    // Start is called before the first frame update
    void Start()
    {
        if (!isActive)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (preventMouseClickCooldown > 0)
        {
            preventMouseClickCooldown -= Time.deltaTime; 
        }

        if (isActive)
        {
            if (Input.GetKeyUp(KeyCode.Escape) ||
                Input.GetKeyUp(KeyCode.Space) ||
                Input.GetKeyUp(KeyCode.Return) ||
                (preventMouseClickCooldown <= 0 && Input.GetMouseButtonUp(0))) // "Up" ist hier wichtig, da bei Down oder nur Button, der Dialog zu geht und der ShipController instant den Button nimmt und das schiff bewegt. Bei "Up" ist der Button druck danach vorbei
            {
                ShowNextStoryText();
            }
        }
    }

    public void SetStoryText(StoryTextEventSO story)
    {
        preventMouseClickCooldown = 0.5f;
        DemoController.Instance.IsStoryTextShown = true;
        this.storyText = story;
        currentTextShown = 0;

        isActive = true;
        this.gameObject.SetActive(true);

        this.ShowNextStoryText();
    }

    public void ShowNextStoryText()
    {
        if (currentTextShown < storyText.Texts.Count)
        {
            textStory.text = storyText.Texts[currentTextShown];
            currentTextShown++;
        }
        else
        {
            CloseStoryText();
        }
    }

    public void CloseStoryText()
    {
        textStory.text = "";
        isActive = false;
        DemoController.Instance.IsStoryTextShown = false;
        this.gameObject.SetActive(false);
        OnStoryTextCompleted?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ShowNextStoryText();
    }
}
