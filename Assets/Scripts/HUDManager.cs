using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TG.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemUiState
{
    public Items Item;
    public bool On = false;
    public Button Button;
}

public class HUDManager : Singleton<HUDManager>
{
    [SerializeField] Player _player;
    private PlayerMovement _playermovement;

    [SerializeField] ItemUiState _legsUI;
    [SerializeField] ItemUiState _wingsUI;

    [SerializeField] ItemUiState _legsStrengthUI;
    [SerializeField] ItemUiState _wingsStrenghUI;

    private List<ItemUiState> _itemsUI = new List<ItemUiState>();

    [Header("HUD Text")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI m_text;

    [SerializeField] float _fadeDuration = .3f;
    [SerializeField] float _duration = 5f;

    bool legs, wings, str = false;

    // Start is called before the first frame update
    void Start()
    {
        _playermovement = _player.GetComponent<PlayerMovement>();

        _itemsUI.Add(_legsUI);
        _itemsUI.Add(_wingsUI);

        _legsUI.Button.onClick.AddListener(() =>
        {
            ToggleItem(_legsUI);
            _playermovement.SetCanJump(_legsUI.On);
        });

        _wingsUI.Button.onClick.AddListener(() =>
        {
            ToggleItem(_wingsUI);
            _playermovement.SetCanFly(_wingsUI.On);
        });

        _legsStrengthUI.Button.onClick.AddListener(() =>
        {
            ToggleItem(_legsStrengthUI);
            if (_legsStrengthUI.On && _wingsStrenghUI.On) ToggleItem(_wingsStrenghUI, _legsStrengthUI);

            _playermovement.SetLegsStrength(_legsStrengthUI.On);
            _playermovement.SetWingsStrength(_wingsStrenghUI.On);
        });

        _wingsStrenghUI.Button.onClick.AddListener(() =>
        {
            ToggleItem(_wingsStrenghUI);
            if (_legsStrengthUI.On && _wingsStrenghUI.On) ToggleItem(_legsStrengthUI, _wingsStrenghUI);

            _playermovement.SetLegsStrength(_legsStrengthUI.On);
            _playermovement.SetWingsStrength(_wingsStrenghUI.On);
        });



        foreach (var item in _itemsUI)
        {

        }
        //_legsUI = new StateItemsUI
        //{
        //    Item = Items.Legs,
        //    Button = _legsButton
        //};
    }

    // Update is called once per frame
    void Update()
    {
        if (legs && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))) { _legsUI.Button.onClick.Invoke(); }
        else if (str && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))) { _legsStrengthUI.Button.onClick.Invoke(); }
        else if (wings && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))) { _wingsUI.Button.onClick.Invoke(); }
        else if (str && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))) { _wingsStrenghUI.Button.onClick.Invoke(); }
    }

    private void ToggleItem(ItemUiState itemUI, ItemUiState alternativeToggle = null)
    {
        if (alternativeToggle != null)
        {
            itemUI.On = !alternativeToggle.On;
        }
        else itemUI.On = !itemUI.On;

        if (itemUI.On)
        {
            itemUI.Button.targetGraphic.color = Color.green;//new Color(.3f, .3f, .3f);
        }
        else
        {
            itemUI.Button.targetGraphic.color = Color.red;
        } // set bg

    }

    public void HandleItem(Items item)
    {
        var playerMovement = _player.GetComponent<PlayerMovement>();
        switch (item)
        {
            case Items.Legs:
                _legsUI.Button.gameObject.SetActive(true);
                legs = true;
                break;
            case Items.Wings:
                _wingsUI.Button.gameObject.SetActive(true);
                wings = true;
                break;
            case Items.Strength:
                _legsStrengthUI.Button.gameObject.SetActive(true);
                _wingsStrenghUI.Button.gameObject.SetActive(true);
                str = true;
                break;
                /*case Items.StarFragments: HandleStarEnergy(player); break;
                case Items.RedKey: HandleHUD(player, Items.RedKey); break;
                case Items.GreenKey: HandleHUD(player, Items.GreenKey); break;
                case Items.BlueKey: HandleHUD(player, Items.BlueKey); break;
                case Items.YellowKey: HandleHUD(player, Items.YellowKey); break;
                case Items.Light: HandleHUD(player, Items.Light); break;*/
        }

    }

    public void SetTexts(params string[] text)
    {
        if (text.Length == 1) SetText(text[0]);
        else StartCoroutine(Internal_SetTexts(text));
    }

    IEnumerator Internal_SetTexts(params string[] text)
    {
        var sec = 6f;
        for (int i = 0; i < text.Length; i++)
        {
            sec = 6f;

            SetText(text[i]);

            while (sec > 0)
            {
                sec -= Time.deltaTime;

                yield return null;
            }
        }
    }

    public void SetText(string text)
    {
        m_text.text = text;
        canvasGroup.DOFade(1, _fadeDuration).OnComplete(FadeOut);
    }

    private void FadeOut()
    {
        canvasGroup.DOFade(0, _fadeDuration).SetDelay(_duration);
    }

}