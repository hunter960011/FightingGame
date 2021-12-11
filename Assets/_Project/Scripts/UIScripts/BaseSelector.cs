using Demonics.Sounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Audio))]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Animator))]
public class BaseSelector : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private Transform _values = default;
    [SerializeField] private Image _rightArrowBackgroundImage = default;
    [SerializeField] private Image _rightArrowImage = default;
    [SerializeField] private Image _leftArrowBackgroundImage = default;
    [SerializeField] private Image _leftArrowImage = default;
    [SerializeField] private UnityEventInt _eventInt = default;
    [SerializeField] private RectTransform _scrollView = default;
    [SerializeField] private float _scrollUpAmount = default;
    [SerializeField] private float _scrollDownAmount = default;
    [SerializeField] private int _initialValue = default;
    [SerializeField] private bool _ignoreFirstSelectSound = default;
    protected Audio _audio;
    protected Button _button;
    protected Animator _animator;
    private int _currentSelectedIndex;
    private bool _isIgnoringFirstSelectSound;


    void Awake()
    {
        _audio = GetComponent<Audio>();
        _button = GetComponent<Button>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        SetValue(_initialValue);
    }

    public void SetValue(int valueIndex)
    {
        if (valueIndex >= 0 && valueIndex < _values.childCount)
        {
            for (int i = 0; i < _values.childCount; i++)
            {
                _values.GetChild(i).gameObject.SetActive(false);
            }
            _currentSelectedIndex = valueIndex;
            _values.GetChild(_currentSelectedIndex).gameObject.SetActive(true);

            _rightArrowBackgroundImage.color = Color.white;
            _leftArrowBackgroundImage.color = Color.white;
            _rightArrowImage.color = Color.black;
            _leftArrowImage.color = Color.black;
            _rightArrowBackgroundImage.raycastTarget = true;
            _leftArrowBackgroundImage.raycastTarget = true;
            if (_currentSelectedIndex == _values.childCount - 1)
            {
                _rightArrowBackgroundImage.color = Color.black;
                _rightArrowImage.color = Color.white;
                _rightArrowBackgroundImage.raycastTarget = false;
            }
            if (_currentSelectedIndex == 0)
            {
                _leftArrowBackgroundImage.color = Color.black;
                _leftArrowImage.color = Color.white;
                _leftArrowBackgroundImage.raycastTarget = false;
            }
            _eventInt.Invoke(_currentSelectedIndex); 
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!_isIgnoringFirstSelectSound)
        {
            _audio.Sound("Selected").Play();
        }
        _animator.SetBool("IsSelected", true);
        _isIgnoringFirstSelectSound = false;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _animator.SetBool("IsSelected", false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _button.Select();
    }

    public void SelectValue(BaseEventData eventData)
    {
		AxisEventData axisEventData = eventData as AxisEventData;
        if (axisEventData.moveDir == MoveDirection.Right)
        {
            NextValue();
        }
        if (axisEventData.moveDir == MoveDirection.Left)
        {
            PreviousValue();
        }
    }

    public void MoveScrollDown(BaseEventData eventData)
    {
        AxisEventData axisEventData = eventData as AxisEventData;
        if (axisEventData.moveDir == MoveDirection.Down)
        {
            _scrollView.anchoredPosition += new Vector2(0.0f, _scrollDownAmount);
        }
    }

    public void MoveScrollUp(BaseEventData eventData)
    {
        AxisEventData axisEventData = eventData as AxisEventData;
        if (axisEventData.moveDir == MoveDirection.Up)
        {
            _scrollView.anchoredPosition += new Vector2(0.0f, _scrollUpAmount);
        }
    }

    public void NextValue()
    {
        _audio.Sound("Selected").Play();
        SetValue(_currentSelectedIndex + 1);
    }

    public void PreviousValue()
    {
        _audio.Sound("Selected").Play();
        SetValue(_currentSelectedIndex - 1);
    }

    void OnEnable()
    {
        _isIgnoringFirstSelectSound = _ignoreFirstSelectSound;
    }

    void OnDisable()
    {
        if (_ignoreFirstSelectSound)
        {
            _isIgnoringFirstSelectSound = true;
        }
        _animator.Rebind();
    }
}
