using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject mainMenuCanvas;
    public CanvasGroup canvasGroup;
    public Button playButton;
    public Button optionsButton;
    public Button exitButton;

    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;

    public GameObject currentOptionsPanel;
    public CanvasGroup canvasGroupOptionsPanel;

    public GameObject currentExitPanel;
    public CanvasGroup canvasGroupExitPanel;

    public List<GameObject> openAfterPlayButton;

    void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonPressed);
        optionsButton.onClick.AddListener(OnOptionsButtonPressed);
        exitButton.onClick.AddListener(OnExitButtonPressed);
    }

    private void OnOptionsButtonPressed()
    {
        if (currentExitPanel != null && currentExitPanel.activeSelf)
        {
            HidePanel(currentExitPanel, canvasGroupExitPanel);
        }

        if (currentOptionsPanel.activeSelf)
        {
            HidePanel(currentOptionsPanel, canvasGroupOptionsPanel);
        }
        else
        {
            ShowPanel(currentOptionsPanel, canvasGroupOptionsPanel);
        }
    }

    private void OnExitButtonPressed()
    {
        if (currentOptionsPanel != null && currentOptionsPanel.activeSelf)
        {
            HidePanel(currentOptionsPanel, canvasGroupOptionsPanel);
        }

        if (currentExitPanel.activeSelf)
        {
            HidePanel(currentExitPanel, canvasGroupExitPanel);
        }
        else
        {
            ShowPanel(currentExitPanel, canvasGroupExitPanel);
        }
    }

    private void OnPlayButtonPressed()
    {
        foreach (var open in openAfterPlayButton)
        {
            open.SetActive(true);
        }
        mainMenu.SetActive(false);
    }

    private void ShowPanel(GameObject panel, CanvasGroup cg)
    {
        panel.SetActive(true);
        FadeInUI(cg, null);
    }

    private void HidePanel(GameObject panel, CanvasGroup cg)
    {
        FadeOutUI(cg, () =>
        {
            panel.SetActive(false);
        });
    }

    private void FadeInUI(CanvasGroup cg, Action onComplete = null)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        cg.DOFade(1, fadeInDuration)
          .SetEase(Ease.InOutQuad)
          .SetUpdate(true)
          .OnStart(() =>
          {
              cg.interactable = true;
              cg.blocksRaycasts = true;
          }).OnComplete(() =>
          {
              onComplete?.Invoke();
          });
    }

    private void FadeOutUI(CanvasGroup cg, Action onComplete = null)
    {
        cg.DOFade(0, fadeOutDuration)
          .SetEase(Ease.InOutQuad)
          .SetUpdate(true)
          .OnComplete(() =>
          {
              cg.interactable = false;
              cg.blocksRaycasts = false;
              onComplete?.Invoke();
          });
    }
}