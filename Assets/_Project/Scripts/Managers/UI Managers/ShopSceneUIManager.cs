using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSceneUIManager : MonoBehaviour
{
    /**
     * This is a UI-Manager script.
     * UI-Manager scripts get and set the main elements of a scene UI that should be altered in code.
     * UI-Manager scripts contain UI elements field, their properties, and some methods as their setters.
     * ShopSceneUIManager:
     * This script will manage the Scene_Shop UI.
    */

    [Header("UI Elements")]
    [SerializeField] private Image itemRacketImage;
    [SerializeField] private Image powerIndicatorBar;
    [SerializeField] private TMP_Text powerIndicatorText;
    [SerializeField] private TMP_Text racketValueText;
    [SerializeField] private Image itemTableImage;
    [SerializeField] private TMP_Text tableValueText;
    [SerializeField] private Button racketBuyButton;
    [SerializeField] private Button tableBuyButton;
    [SerializeField] private TMP_Text racketBoughtText;
    [SerializeField] private TMP_Text tableBoughtText;

    public Image ItemRacketImage { get { return itemRacketImage; } }
    public Image PowerIndicatorBar { get { return powerIndicatorBar; } }
    public TMP_Text PowerIndicatorText { get { return powerIndicatorText; } }
    public TMP_Text RacketValueText { get { return racketValueText; } }
    public Image ItemTableImage { get { return itemTableImage; } }
    public TMP_Text TableValueText { get { return tableValueText; } }
    public Button RacketBuyButton { get { return racketBuyButton; } }
    public Button TableBuyButton { get { return tableBuyButton; } }
    public TMP_Text RacketBoughtText { get { return racketBoughtText; } }
    public TMP_Text TableBoughtText { get { return tableBoughtText; } }

    public void AssignRacketImage(Sprite image) => itemRacketImage.sprite = image;

    public void AssignTableImage(Sprite image) => itemTableImage.sprite = image;

    public void SetRacketValueText(int value) => racketValueText.text = $"Required Point: {value}";

    public void SetTableValueText(int value) => tableValueText.text = $"Required Point: {value}";

    public void SetPowerBarAndText(float powerAmount)
    {
        powerIndicatorBar.fillAmount = powerAmount;
        powerIndicatorText.text = $"Power: {powerAmount * 100f}%";
    }

    public void SetRacketAsBought()
    {
        powerIndicatorBar.transform.parent.gameObject.SetActive(false);
        powerIndicatorText.enabled = false;
        racketValueText.enabled = false;
        racketBuyButton.gameObject.SetActive(false);

        racketBoughtText.gameObject.SetActive(true);
    }

    public void DisableRacketBoughtPhase()
    {
        racketBoughtText.gameObject.SetActive(false);

        powerIndicatorBar.transform.parent.gameObject.SetActive(true);
        powerIndicatorText.enabled = true;
        racketValueText.enabled = true;
        racketBuyButton.gameObject.SetActive(true);
    }

    public void SetTableAsBought()
    {
        tableValueText.enabled = false;
        tableBuyButton.gameObject.SetActive(false);

        tableBoughtText.gameObject.SetActive(true);
    }

    public void DisableTableBoughtPhase()
    {
        tableBoughtText.gameObject.SetActive(false);

        tableValueText.enabled = true;
        tableBuyButton.gameObject.SetActive(true);
    }
}
