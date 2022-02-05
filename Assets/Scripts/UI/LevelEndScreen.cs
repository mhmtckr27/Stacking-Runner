using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelEndScreen : ScreenBase, IPointerClickHandler
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject levelCompleteBanner;
    [SerializeField] private GameObject scoreHorizontalLayout;
    [SerializeField] private TextMeshProUGUI levelCompletedText;
    [SerializeField] private TextMeshProUGUI highScoreText; 
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI bestScoreText; 
    [SerializeField] private TextMeshProUGUI gainedGoldText;
    [SerializeField] private GameObject tapToContinue;
    [SerializeField] private ParticleSystem coinBurstVFX;
    [SerializeField] private ParticleSystem coinScoreVFX;
    [SerializeField] private List<ParticleSystem> colorfulVFXs;

    public event Action OnTapToContinue;

    public static LevelEndScreen Instance { get; private set; }

    protected override void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        FinishLine.OnFinishLine += UpdateEndLevelScreen;
        GameManager.Instance.IsNewHighScore += GameManager_IsNewHighScore;
        CoinVFXStateMachineBehaviour.OnPlayCoinVFX += LevelEndScreen_OnScorePanelSlidedFinished;
        ScoreBlendInStateMachineBehaviour.OnScoreBlendInStart += ScoreBlendInStateMachineBehaviour_OnScoreBlendInStart;
        ShowLevelCompleteAndTapToContinue.OnReadyToShow += ShowLevelCompleteAndTapToContinue_OnReadyToShow;
    }

    private void ShowLevelCompleteAndTapToContinue_OnReadyToShow()
    {
        levelCompleteBanner.gameObject.SetActive(true);
        tapToContinue.SetActive(true);
        InGameScreen.Instance.currentLevelPanel.SetActive(false);
        levelsPanel.SetActive(true);
        StartCoroutine(UpdateLevelsProgressBarRoutine());
        foreach (ParticleSystem colorfulVFX in colorfulVFXs)
        {
            colorfulVFX.Play();
        }
    }

    private void ScoreBlendInStateMachineBehaviour_OnScoreBlendInStart()
    {
        scoreHorizontalLayout.SetActive(true);
    }

    private void LevelEndScreen_OnScorePanelSlidedFinished(CoinVFXType vfxType)
    {
        (vfxType == CoinVFXType.Burst ? coinBurstVFX : coinScoreVFX).Play();
    }

    private void OnDestroy()
    {
        FinishLine.OnFinishLine -= UpdateEndLevelScreen;
        GameManager.Instance.IsNewHighScore -= GameManager_IsNewHighScore;
        CoinVFXStateMachineBehaviour.OnPlayCoinVFX -= LevelEndScreen_OnScorePanelSlidedFinished;
        ScoreBlendInStateMachineBehaviour.OnScoreBlendInStart -= ScoreBlendInStateMachineBehaviour_OnScoreBlendInStart;
        ShowLevelCompleteAndTapToContinue.OnReadyToShow -= ShowLevelCompleteAndTapToContinue_OnReadyToShow;
    }

    private void GameManager_IsNewHighScore(bool isNewHighScore, int newScore, int highScore)
    {
        highScoreText.text = isNewHighScore ? "NEW HIGH SCORE!" : "SCORE";
        scoreText.text = newScore.ToString();
        gainedGoldText.text = "+" + scoreText.text;
        bestScoreText.text = "Best Score - " + highScore;
        bestScoreText.gameObject.SetActive(!isNewHighScore);
    }

    private void UpdateEndLevelScreen()
    {
        levelCompletedText.text = "Level " + GameManager.Instance.CurrentLevel + " Completed!";
        gameObject.SetActive(true);
        UpdateLevelsBar(GameManager.Instance.CurrentLevel);
        animator.SetTrigger("LevelEndTrigger");
    }

    private IEnumerator UpdateLevelsProgressBarRoutine() 
    {
        yield return new WaitForSeconds(0.25f);
        UpdateLevelsBar(GameManager.Instance.CurrentLevel + 1, 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        scoreHorizontalLayout.SetActive(false);
        foreach (ParticleSystem colorfulVFX in colorfulVFXs)
        {
            colorfulVFX.Stop();
        }
        InGameScreen.Instance.currentLevelPanel.SetActive(true);
        UpdateLevelsBar(GameManager.Instance.CurrentLevel + 1);
        OnTapToContinue?.Invoke();
    }
}

public enum CoinVFXType
{
    Burst,
    MoveToScore
}