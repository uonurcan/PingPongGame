using UnityEngine;

[CreateAssetMenu(fileName = "New Racket", menuName = "Inventory/Racket")]
public class Racket : ScriptableObject
{
    /**
      * Racket is a ScriptableObject for storing game rackets info as an in-project file.
     */

    [SerializeField] private string racketID; // * This field must be equivalent to the Rackets column in the database.
    [SerializeField] private Sprite racketImage;
    [SerializeField] [Range(0, 1f)] private float racketPower;
    [SerializeField] private int racketValue;
    [SerializeField] private bool bought;

    public string RacketID { get { return racketID; } }
    public Sprite RacketImage { get { return racketImage; } }
    public float RacketPower { get { return racketPower; } }
    public int RacketValue { get { return racketValue; } }
    public bool Bought { get { return bought; }}

    public void SetAsBought() => bought = true;
}
