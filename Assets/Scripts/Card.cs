using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private int cardID;
    public GameManagerCard gameManager;
    private bool isFlipped;
    [SerializeField] private bool isMatched = false;
    [SerializeField] private Image cardImage;

    public int CardID { get => cardID; set => cardID = value; }
    public bool IsMatched { get => isMatched; set => isMatched = value; }

    void Start()
    {
        ResetCard();
    }

    public void ResetCard()
    {
        isFlipped = false;
        isMatched = false;

        if (cardImage == null)
        {
            Debug.LogError("cardImage not assigned on Card", this);
        }

        if (gameManager == null)
        {
            gameManager = GameManagerCard.Instance;
        }

        if (GameManagerCard.Instance != null && cardImage != null)
        {
            cardImage.sprite = GameManagerCard.Instance.CardBack;
        }

        SetInteractable(true);
    }

    public void SetInteractable(bool enabled)
    {
        var btn = GetComponent<UnityEngine.UI.Button>();
        if (btn != null) btn.interactable = enabled;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = enabled;
    }

    public void FlipCard()
    {
        if (IsMatched) return;
        if (gameManager == null) gameManager = GameManagerCard.Instance;
        if (!isFlipped && gameManager != null && (gameManager.FirstCard == null || gameManager.SecondCard == null))
        {
            isFlipped = true;
            if (cardImage != null && gameManager.CardFaces != null && cardID >= 0 && cardID < gameManager.CardFaces.Length)
                cardImage.sprite = gameManager.CardFaces[cardID];
            gameManager.CardFlipped(this);
        }
    }

    public void HideCard()
    {
        isFlipped = false;
        if (gameManager != null && cardImage != null)
            cardImage.sprite = gameManager.CardBack;
    }
}
