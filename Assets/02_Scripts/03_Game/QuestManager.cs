using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    private bool _questStage1complete;
    private bool _questStage2complete;
    private bool _questStage3complete;
    private bool _questStage4complete;

    private TextMeshProUGUI _quest1Text;
    private TextMeshProUGUI _quest2Text;
    private TextMeshProUGUI _quest3Text;
    private TextMeshProUGUI _quest4Text;
    private TextMeshProUGUI _quest5Text;

    //Bela: Could be worth making it a Singelton and use DoNotDestroyOnLoad
    
    /// <summary>
    /// Variables for quest panel movement
    /// </summary>
    bool isDown = true;
    [SerializeField]
    RectTransform questPanel;
    bool isCompleted;
    [SerializeField]
    Image buttonImage;
    [SerializeField] float buttonAnimationDuration = 0.3f;
    [SerializeField] float panelAnimationDuration = 0.7f;
    [SerializeField] private float upPositionValue = 263;
    [SerializeField] private float downPositionValue = -22;

    // Start is called before the first frame update
    void Start()
    {
        _quest1Text = GameObject.Find("Quest1Text").GetComponent<TextMeshProUGUI>();
        _quest2Text = GameObject.Find("Quest2Text").GetComponent<TextMeshProUGUI>();
        _quest3Text = GameObject.Find("Quest3Text").GetComponent<TextMeshProUGUI>();
        _quest4Text = GameObject.Find("Quest4Text").GetComponent<TextMeshProUGUI>();
        _quest5Text = GameObject.Find("Quest5Text").GetComponent<TextMeshProUGUI>();

        _quest3Text.enabled = false;
        _quest4Text.enabled = false;
        _quest5Text.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CollectItem._backpackCollected && !_questStage1complete)
        {
            _questStage1complete = true;
            
            // activate next quest text, "Meet up with Drustan back at the front gate."
            _quest3Text.enabled = true;
            // activate crossed out resolved quest text, "Find a backpack."
            _quest2Text.fontStyle = FontStyles.Strikethrough;
            _quest2Text.fontStyle = FontStyles.Bold;
        }
        else if (CollectItem._parchmentCollected && !_questStage2complete)
        {
            _questStage2complete = true;
            
            // activate next quest text "Find the hidden key under the throne."
            _quest4Text.enabled = true;
            // activate crossed out resolved quest text "Meet up with Drustan back at the front gate."
            _quest3Text.fontStyle = FontStyles.Strikethrough;
            _quest3Text.fontStyle = FontStyles.Bold;
        }
        else if (CollectItem._keyCollected && !_questStage3complete)
        {
            _questStage3complete = true;
            
            // activate next quest text "Find the secret passage."
            _quest5Text.enabled = true;
            // activate crossed out resolved quest text "Find the hidden key under the throne."
            _quest4Text.fontStyle = FontStyles.Strikethrough;
            _quest4Text.fontStyle = FontStyles.Bold;
        }
        else if (CollectItem._secretPassageOpened && !_questStage4complete)
        {
            _questStage4complete = true;
            
            // activate crossed out resolved quest text "Find the secret passage."
            _quest5Text.fontStyle = FontStyles.Strikethrough;
            _quest5Text.fontStyle = FontStyles.Bold;
            // activate crossed out resolved quest text "Escape the keep unharmed."
            _quest1Text.fontStyle = FontStyles.Strikethrough;
            _quest1Text.fontStyle = FontStyles.Bold;
        }
    }
    
    public void MoveQuestPanelDown()
    {
        if (!isDown)
        {
            DOTween.Sequence()
                .Append(questPanel.DOAnchorPosY(downPositionValue, panelAnimationDuration)).Append(buttonImage.transform.DOScaleY(-1f, buttonAnimationDuration)).AppendCallback(() =>
                {
                    isCompleted = true;
                    Debug.Log(isCompleted);


                    if (isCompleted)
                    {

                        isCompleted = false;
                        isDown = true;
                    }
                });
        }
    }
    
    public void MoveQuestPanelUp()
    {
        if (isDown)
        {

            DOTween.Sequence()
                .Append(questPanel.DOAnchorPosY(upPositionValue, panelAnimationDuration)).Append(buttonImage.transform.DOScaleY(1f, buttonAnimationDuration)).AppendCallback(() =>
                {
                    Debug.Log("OnComplete tween");
                    isCompleted = true;
                    Debug.Log(isCompleted);

                    if (isCompleted)
                    {

                        isCompleted = false;
                        isDown = false;
                    }
                });
        }
    }
}
