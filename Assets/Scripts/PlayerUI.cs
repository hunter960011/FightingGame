using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _lostLives = default;
    [SerializeField] private Slider _healthSlider = default;
    [SerializeField] private TextMeshProUGUI _comboText = default;
    private int _currentLifeIndex;
    private int _currentComboCount;


    public void SetMaxHealth(float value)
    {
        _healthSlider.maxValue = value;
    }

    public void SetHealth(float value)
    {
        _healthSlider.value = value;
    }

    public void SetLives(int lives)
    {
        _lostLives[_currentLifeIndex].SetActive(true);
        _currentLifeIndex++;
    }

    public void ResetLives()
    {
        _lostLives[0].SetActive(false);
        _lostLives[1].SetActive(false);
        _currentLifeIndex = 0;
    }

    public void IncreaseCombo()
    {
        _currentComboCount++;
        _comboText.text = "Hits " + _currentComboCount.ToString();
        if (_currentComboCount > 1)
        {
            _comboText.gameObject.SetActive(true);
        }
    }

    public void ResetCombo()
    {
        StartCoroutine(ResetComboCoroutine());
    }

    IEnumerator ResetComboCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        _comboText.gameObject.SetActive(false);
        _currentComboCount = 0;
        _comboText.text = "Hits 0";
    }
}
