using FlavorfulStory.Actions.Interactables;
using FlavorfulStory.InputSystem;
using UnityEngine;

public class RepairableBuilding : MonoBehaviour, IInteractable
{
    [Tooltip("Стадии ремонта.")] 
    [SerializeField] private BuildingRepairConfig _repairConfig;

    private BuildingRepairView _repairView;
    
    private void Awake()
    {
        _repairView = FindFirstObjectByType<BuildingRepairView>(FindObjectsInactive.Include);
    }

    private void Start()
    {
        IsInteractionAllowed = true;
        IsBlockingMovement = true;
    }

    private void Update()
    {
        if (_repairView.IsOpen && InputWrapper.GetButtonDown(InputButton.SwitchGameMenu, true))
        {
            _repairView.Close();
        }
    }

    #region Iteractable
    public string GetTooltipTitle() => "Repair building";

    public string GetTooltipDescription() => " Interact with me, handsome!";

    public Vector3 GetWorldPosition() => transform.position;

    public bool IsInteractionAllowed { get; set; }
    
    [field: SerializeField] public bool IsBlockingMovement { get; set; }

    public void Interact()
    {
        _repairView.Open();
    }

    public float GetDistanceTo(Transform otherTransform) => Vector3.Distance(otherTransform.position, transform.position);

    #endregion
}