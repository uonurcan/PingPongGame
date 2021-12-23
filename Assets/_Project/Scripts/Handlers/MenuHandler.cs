using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    /**
    * Each scene in the game contains several menu panels.
    * With MenuHanlder we can store menu objects and do available operations on them.
    */

    // Scene menus will be stored here so can be accessed by index.
    [SerializeField] private GameObject[] menusList;

    // Open menu if cloes and vice versa.
    public void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeInHierarchy);
    }

    // Just another overload of ToggleMenu that uses menuIndex instead of the actual object.
    public void ToggleMenu(int menuIndex)
    {
        menusList[menuIndex].SetActive(!menusList[menuIndex].activeInHierarchy);
    }
}
