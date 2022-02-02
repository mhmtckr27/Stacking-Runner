using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private Text levelText;
    [SerializeField] private Text levelTextShadow;
    [SerializeField] private Image levelPanelBackground;

    public void UpdateLevel(int level, LevelState levelState)
    {

        switch (levelState)
        {
            case LevelState.NotAchieved:
                levelText.text = level.ToString();
                levelTextShadow.text = level.ToString();
                levelText.color = Color.black;
                levelTextShadow.color = Color.gray;
                levelPanelBackground.color = new Color(.85f, .85f, .85f);
                rect.localScale = Vector2.one;
                break;
            case LevelState.Achieved:
                levelText.text = level.ToString();
                levelTextShadow.text = level.ToString();
                levelText.color = Color.white;
                levelTextShadow.color = Color.black;
                levelPanelBackground.color = new Color(0, 0.8f, 0);
                rect.localScale = Vector2.one;
                break;
            case LevelState.Current:
                levelText.text = level.ToString();
                levelTextShadow.text = level.ToString();
                levelText.color = Color.black;
                levelTextShadow.color = Color.gray;
                levelPanelBackground.color = Color.white;
                rect.localScale = 1.2f * Vector2.one;
                break;
            default:
                break;
        }
    }
}
