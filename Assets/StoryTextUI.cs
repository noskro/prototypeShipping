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
        if (isActive)
        {
            if (Input.GetKeyUp(KeyCode.Escape) ||
                Input.GetKeyUp(KeyCode.Space) ||
                Input.GetKeyUp(KeyCode.Return))
            {
                ShowNextStoryText();
            }
        }
    }

    public void SetStoryText(StoryTextEventSO story)
    {
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
