using System.Collections;
using System.Collections.Generic;
using TG.GameJamTemplate;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] Items _itemType;
    [SerializeField] private AudioClip _itemSound;
    [SerializeField] private float _itemSoundVolume = .5f;

    private const int _starEnergy = 1;

    private bool _hasTouchedPlayer = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasTouchedPlayer) return;

        var player = collision.GetComponentInParent<Player>();
        if (player == null) return;

        HandleItems(player);
    }

    private void HandleItems(Player player)
    {
        HUDManager.I.HandleItem(_itemType);
        if (_itemSound != null) AudioManager.I.CreateOneShot(_itemSound, transform.position, _itemSoundVolume);
        switch (_itemType)
        {
            case Items.Legs:
                HUDManager.I.SetTexts(
                    "You got the bipede DNA! You can enable this by pressing the legs button below (shortcut: [1] key) [Space] to Jump.",
                    "This new DNA enables jumping and faster movement. It can be toggled at any time."
                    );
                break;
            case Items.Wings:
                HUDManager.I.SetText("You got some bird DNA! You can now fly! (shortcut: [3] key)");
                break;
            case Items.Strength:
                HUDManager.I.SetTexts(
                    "You got a Strength Modifier DNA. You can add this to legs or wings DNA (shortcuts: [2]/[4] keys).",
                    "Chaining order matters, so play around with it!"
                    );
                break;
                /*case Items.StarFragments: HandleStarEnergy(player); break;
                case Items.RedKey: HandleHUD(player, Items.RedKey); break;
                case Items.GreenKey: HandleHUD(player, Items.GreenKey); break;
                case Items.BlueKey: HandleHUD(player, Items.BlueKey); break;
                case Items.YellowKey: HandleHUD(player, Items.YellowKey); break;
                case Items.Light: HandleHUD(player, Items.Light); break;*/
        }

        _hasTouchedPlayer = true;

        Destroy(gameObject);
    }

    private void HandleHUD(Player player, Items item)
    {
        player.SetItem(item);
        //
    }

    private void HandleStarEnergy(Player player)
    {
        player.SetEnergyAmount(_starEnergy);
        //if (_itemSound != null) AudioManager.I.CreateOneShot(_itemSound, transform.position, _itemSoundVolume);
    }
}