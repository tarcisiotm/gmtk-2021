using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField] HUDManager _hud;
    [SerializeField] int _energyAmount = 0;

    //private Items _currentlyHoldingItem = Items.None;

    public delegate void EnergyAcquired(int energy);
    public event EnergyAcquired OnEnergyAcquired;

    //public Items HoldingItem => _currentlyHoldingItem;

    private void Start()
    {
    }

    public void SetEnergyAmount(int energy)
    {
        _energyAmount += energy;
        //_hud.SetStarFragment(_energyAmount);

        OnEnergyAcquired?.Invoke(_energyAmount);
    }

    public int GetEnergyAmount()
    {
        return _energyAmount;
    }

    public void SetItem(Items item)
    {
        //_currentlyHoldingItem = item;
        // _hud.SetItem(item);
    }

    private void Update()
    {
    }
}