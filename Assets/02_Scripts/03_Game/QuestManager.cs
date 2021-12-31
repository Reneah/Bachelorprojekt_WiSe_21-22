using DG.Tweening;
using Enemy.Controller;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    //Bela: Could be worth making it a Singelton and use DoNotDestroyOnLoad
    
    /// <summary>
    /// Variables for quest line
    /// </summary>

    private TextMeshProUGUI _quest1Text;
    private TextMeshProUGUI _quest2Text;
    private TextMeshProUGUI _quest3Text;
    private TextMeshProUGUI _quest4Text;
    private TextMeshProUGUI _quest5Text;
    private TextMeshProUGUI _quest6Text;
    private TextMeshProUGUI _quest7Text;

    [SerializeField] private int _provisionsQuestTarget;
    private GameObject _staircaseToCellarInteractionObjects;
    private CollectProvisions _collectProvisions;
    private int _currentProvisionsCount;
    private bool _provisionsQuestDone;
    private EnemyController[] _enemyController;

    public GameObject StaircaseToCellarInteractionObjects
    {
        get => _staircaseToCellarInteractionObjects;
        set => _staircaseToCellarInteractionObjects = value;
    }

    public bool ProvisionsQuestDone
    {
        get => _provisionsQuestDone;
        set => _provisionsQuestDone = value;
    }
    
    /// <summary>mm
    /// Variables for quest panel movement
    /// </summary>
    bool isDown = false;
    [SerializeField]
    RectTransform questPanel;
    bool isCompleted;
    [SerializeField]
    Image buttonImage;
    [SerializeField] float buttonAnimationDuration = 0.3f;
    [SerializeField] float panelAnimationDuration = 0.7f;
    [SerializeField] private float upPositionValue = 263;
    [SerializeField] private float downPositionValue = -22;
    [SerializeField]
    private bool firstScene;

    private bool _provisionsQuestPopUpHappened;
    private bool _provisionsKeyPopUpHappened;

    private void Awake()
    {
        DOTween.Sequence()
            .Append(questPanel.DOAnchorPosY(upPositionValue, 0)).Append(buttonImage.transform.DOScaleY(1f, 0)).AppendCallback(() =>
            {
                isCompleted = true;

                if (isCompleted)
                {
                    isCompleted = false;
                }
            });
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!firstScene)
        {
            MoveQuestPanelDown();
        }
        _collectProvisions = FindObjectOfType<CollectProvisions>();
        _staircaseToCellarInteractionObjects = GameObject.Find("StaircaseToCellarInteractionObjects");
        _staircaseToCellarInteractionObjects.SetActive(false);
        _provisionsQuestPopUpHappened = false;
        _provisionsKeyPopUpHappened = false;

        _quest1Text = GameObject.Find("Quest1Text").GetComponent<TextMeshProUGUI>();
        _quest2Text = GameObject.Find("Quest2Text").GetComponent<TextMeshProUGUI>();
        _quest3Text = GameObject.Find("Quest3Text").GetComponent<TextMeshProUGUI>();
        _quest4Text = GameObject.Find("Quest4Text").GetComponent<TextMeshProUGUI>();
        _quest5Text = GameObject.Find("Quest5Text").GetComponent<TextMeshProUGUI>();
        _quest6Text = GameObject.Find("Quest6Text").GetComponent<TextMeshProUGUI>();
        _quest7Text = GameObject.Find("Quest7Text").GetComponent<TextMeshProUGUI>();

        _quest3Text.enabled = false;
        _quest4Text.enabled = false;
        _quest5Text.enabled = false;
        _quest6Text.enabled = false;
        _quest7Text.enabled = false;
    
        // Reset QuestManager quest texts to regular font on New Game Start
        if (!CollectItem._backpackCollected)
        {
            _quest2Text.fontStyle = FontStyles.Normal;
        }
        if (!CollectItem._parchmentCollected)
        {
            _quest3Text.fontStyle = FontStyles.Normal;
        }
        if (_currentProvisionsCount <= _collectProvisions.ProvisionsCounter)
        {
            _quest4Text.fontStyle = FontStyles.Normal;
        }
        if (!CollectItem._keyCollected)
        {
            _quest5Text.fontStyle = FontStyles.Normal;
        }
        if (!CollectItem._enteredStaircase)
        {
            _quest6Text.fontStyle = FontStyles.Normal;
            _quest7Text.fontStyle = FontStyles.Underline;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CollectItem._backpackCollected)
        {
            // activate next quest text, "Meet up with Drustan back at the front gate."
            _quest3Text.enabled = true;
            // activate crossed out resolved quest text, "Find a backpack."
            _quest2Text.fontStyle = FontStyles.Strikethrough;
        }
        
        if (CollectItem._parchmentCollected)
        {
            // activate next quest text "Gather X provisions."
            _quest4Text.enabled = true;
            // activate next quest text "Find the hidden key under the throne."
            _quest5Text.enabled = true;
            // activate crossed out resolved quest text "Meet up with Drustan back at the front gate."
            _quest3Text.fontStyle = FontStyles.Strikethrough;
        }
        
        if (CollectProvisions._provisionsActive)
        {
            _currentProvisionsCount = _collectProvisions.ProvisionsCounter;

            if (_currentProvisionsCount >= _provisionsQuestTarget)
            {
                // activate crossed out resolved quest text "Gather at least X provisions."
                _quest4Text.fontStyle = FontStyles.Strikethrough;
                _provisionsQuestDone = true;

                if (!_provisionsQuestPopUpHappened)
                {
                    MoveQuestPanelDown();
                    _provisionsQuestPopUpHappened = true;
                }

                // Activate interaction visualisation objects at staircase asset
                if (CollectItem._keyCollected)
                {
                    _staircaseToCellarInteractionObjects.SetActive(true);
                }
            }
            else
            {
                // revoke crossed out resolved quest text "Gather at least X provisions."
                _quest4Text.fontStyle = FontStyles.Normal;
                _provisionsQuestDone = false;
                
                // Dectivate interaction visualisation objects at staircase asset
                if (CollectItem._keyCollected)
                {
                    _staircaseToCellarInteractionObjects.SetActive(false);
                }
            }
        }
        
        if (CollectItem._keyCollected)
        {
            // activate next quest text "Enter the staircase to the cellar, to find the secret passage."
            _quest6Text.enabled = true;
            _quest7Text.enabled = true;
            // activate crossed out resolved quest text "Find the hidden key under the throne."
            _quest5Text.fontStyle = FontStyles.Strikethrough;
            
            if (!_provisionsKeyPopUpHappened)
            {
                MoveQuestPanelDown();
                _provisionsKeyPopUpHappened = true;
            }
        }

        if (CollectItem._enteredStaircase)
        {
            // Doesn't get visualised as next scene loads instantly on entering the staircase. Left as is, so not to mess with the code
            
            // activate crossed out resolved quest text "Enter the staircase to the cellar, to find the secret passage."
            _quest6Text.fontStyle = FontStyles.Strikethrough;
            // activate crossed out resolved quest text "Escape the keep unharmed."
            _quest1Text.fontStyle = FontStyles.Strikethrough;
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
                    isCompleted = true;

                    if (isCompleted)
                    {

                        isCompleted = false;
                        isDown = false;
                    }
                });
        }
    }
}
