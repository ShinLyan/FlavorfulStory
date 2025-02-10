using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FlavorfulStory.UI;

public class ResourceRequirementView : MonoBehaviour
{
    [SerializeField] private Image _resourceIcon;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private CustomButton _addResourceButton;
    [SerializeField] private CustomButton _returnResourceButton;

    public void SetQuantityText(int currentQuantity, int requiredQuantity)
    {
        _quantityText.text = $"{currentQuantity}/{requiredQuantity}";
    }

    public void AddResourceButtonSetListener()
    {
        throw new NotImplementedException();
    }

    public void ReturnResourceButtonSetListener()
    {
        throw new NotImplementedException();
    }

    public void RemoveButtonListeners()
    {
        throw new NotImplementedException();
    }
}