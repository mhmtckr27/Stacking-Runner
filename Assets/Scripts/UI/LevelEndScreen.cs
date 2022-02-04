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
    [SerializeField] private TextMeshProUGUI levelCompletedText;
    [SerializeField] private TextMeshProUGUI highScoreText; 
    [SerializeField] private Animator tapToContinueAnimator;
    [SerializeField] private ParticleSystem coinBurstVFX;
    [SerializeField] private ParticleSystem coinScoreVFX;

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

    void Start()
    {
        FinishLine.OnFinishLine += UpdateEndLevelScreen;
        GameManager.Instance.IsNewHighScore += GameManager_IsNewHighScore;
    }

    private void OnDestroy()
    {
        FinishLine.OnFinishLine -= UpdateEndLevelScreen;
        GameManager.Instance.IsNewHighScore -= GameManager_IsNewHighScore;
    }

    private void GameManager_IsNewHighScore(bool isNewHighScore)
    {
        highScoreText.text = isNewHighScore ? "NEW HIGH SCORE!" : "SCORE";
    }

    private void UpdateEndLevelScreen()
    {
        levelCompletedText.text = "Level " + GameManager.Instance.CurrentLevel + " Completed!";
        gameObject.SetActive(true);
        animator.SetTrigger("LevelEndTrigger");
       /* UpdateLevelsBar(GameManager.Instance.CurrentLevel);
        StartCoroutine(UpdateLevelsProgressBarRoutine());
        StartCoroutine(PlayVFXs());*/
    }

    private IEnumerator UpdateLevelsProgressBarRoutine() 
    {
        yield return new WaitForSeconds(0.25f);
        UpdateLevelsBar(GameManager.Instance.CurrentLevel + 1, 0.5f);
    }

    private IEnumerator PlayVFXs()
    {
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(PlayVFXRoutine(coinBurstVFX));
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(PlayVFXRoutine(coinScoreVFX));
        yield return new WaitForSeconds(1f);
        yield return UIManager.Instance.StartCoroutine(UIManager.Instance.UpdateGoldTextRoutine(GameManager.Instance.GoldAmount));
        tapToContinueAnimator.SetBool("shouldPlay", true);
    }


    private IEnumerator PlayVFXRoutine(ParticleSystem toPlay)
    {
        toPlay.gameObject.SetActive(true);
        toPlay.Play();
        while (toPlay.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        toPlay.Stop();
        toPlay.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        OnTapToContinue?.Invoke();
    }
}
