using UnityEngine;

[CreateAssetMenu(fileName = "New Table", menuName = "Inventory/Table")]
public class Table : ScriptableObject
{
    /**
     * Table is a ScriptableObject for storing game tables info as an in-project file.
     */

    [SerializeField] private string tableID; // * This field must be equivalent to the Tables column in the database.
    [SerializeField] private Sprite tableImage;
    [SerializeField] private int tableValue;
    [SerializeField] private bool bought;

    public string TableID { get { return tableID; } }
    public Sprite TableImage { get { return tableImage; } }
    public int TableValue { get { return tableValue; } }
    public bool Bought { get { return bought; } }

    public void SetAsBought() => bought = true;
}
