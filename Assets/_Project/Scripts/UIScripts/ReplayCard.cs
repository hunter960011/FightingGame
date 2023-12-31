using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReplayCard : MonoBehaviour
{
    [SerializeField] private Image _playerOneImage = default;
    [SerializeField] private Image _playerTwoImage = default;
    [SerializeField] private TextMeshProUGUI _versionText = default;
    [SerializeField] private TextMeshProUGUI _dateText = default;
    [SerializeField] private Sprite[] _characterPortraits = default;
    public ReplayCardData ReplayCardData { get; private set; }


    public void SetData(ReplayCardData replayData)
    {
        ReplayCardData = replayData;
        _playerOneImage.sprite = GetCharacterPortrait(replayData.characterOne);
        _playerTwoImage.sprite = GetCharacterPortrait(replayData.characterTwo);
        _versionText.text = $"Ver {replayData.versionNumber}";
        _dateText.text = replayData.date;
    }

    private Sprite GetCharacterPortrait(int index)
    {
        return _characterPortraits[index];
    }
}

public struct ReplayCardData
{
    public string versionNumber;
    public string date;
    public int characterOne;
    public int colorOne;
    public int assistOne;
    public int characterTwo;
    public int colorTwo;
    public int assistTwo;
    public int stage;
    public string musicName;
    public bool bit1;
    public ReplayInput[] playerOneInputs;
    public ReplayInput[] playerTwoInputs;
    public float skip;
}

public struct ReplayInput
{
    public InputEnum input;
    public InputDirectionEnum direction;
    public int time;
}