using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManagerInstance;
    public bool GameState;
    public GameObject uiElement;
    public TextMeshProUGUI TaptoPlayText;
    public Image scoreSlider;
    public List<GameObject> finishCubes;

    private Vector3 _originalTextScale, _scaleTo;

    private void Awake()
    {
        UIManagerInstance = this;
        GameState = false;
        initFinishCubes();
    }
    void Start()
    {
        _originalTextScale = TaptoPlayText.transform.localScale;
        _scaleTo = _originalTextScale * 1.5f;
        TaptoPlayText.rectTransform
        .DOScale(_scaleTo, .5f)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        float value = (ScoreManager.instance.score / 50f);


        if (ScoreManager.instance.score < 0)
        {
            UIManager.UIManagerInstance.GameState = false;
        }
    }
    public void StartTheGame()
    {
        GameState = true;
        uiElement.SetActive(false);
        GameObject.FindWithTag("airParticle").GetComponent<ParticleSystem>().Play();
        GameObject.FindWithTag("ballTrail").GetComponent<ParticleSystem>().Play();
    }

    public void UpdateSlider()
    {
        scoreSlider.fillAmount = ScoreManager.instance.score / 100;
    }

    private void initFinishCubes()
    {
        int i = 1;
        int numberOfCubes = finishCubes.Count;

        foreach (var finishCube in finishCubes)
        {
            finishCube.transform.localPosition = new Vector3(0, 0.01f, i);
            finishCube.GetComponent<Renderer>().material.color = Color.HSVToRGB(i * (1.0f / numberOfCubes), .67f, .94f);
            finishCube.GetComponentInChildren<TextMeshProUGUI>().text = "x" + i;
            i++;
        }
    }

}
