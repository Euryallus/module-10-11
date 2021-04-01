using UnityEngine;

[CreateAssetMenu(fileName = "Consumable Item", menuName = "Item/Consumable Item")]
public class ConsumableItem : Item
{
    public float HungerIncrease { get { return m_hungerIncrease; } }


    [Space]
    [Header("Consumable Item Options")]

    [SerializeField] [Tooltip("How much the player's food level will increase by when this item is eaten")]
    private float m_hungerIncrease;
}
