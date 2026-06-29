using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private int cardID;
    public GameManagerCard gameManager;
    private bool isFlipped;
    [SerializeField] private bool isMatched = false;
    [SerializeField] private Image cardImage;
    private Button cardButton;
    private Collider2D cardCollider;

    public int CardID { get => cardID; set => cardID = value; }
    public bool IsMatched { get => isMatched; set => isMatched = value; }

    void Awake()
    {
        cardButton = GetComponent<Button>();
        cardCollider = GetComponent<Collider2D>();
    }

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

        var currentGameManager = gameManager ?? GameManagerCard.Instance;
        if (currentGameManager != null && cardImage != null)
        {
            cardImage.sprite = currentGameManager.CardBack;
        }

        SetInteractable(true);
    }

    public void SetInteractable(bool enabled)
    {
        if (cardButton != null) cardButton.interactable = enabled;

        if (cardCollider != null) cardCollider.enabled = enabled;
    }

    public void FlipCard()
    {
        if (IsMatched) return;
        if (gameManager == null) gameManager = GameManagerCard.Instance;

        var currentGameManager = gameManager ?? GameManagerCard.Instance;
        if (!isFlipped && currentGameManager != null && (currentGameManager.FirstCard == null || currentGameManager.SecondCard == null))
        {
            isFlipped = true;
            if (cardImage != null && currentGameManager.CardFaces != null && cardID >= 0 && cardID < currentGameManager.CardFaces.Length)
                cardImage.sprite = currentGameManager.CardFaces[cardID];
            currentGameManager.CardFlipped(this);
        }
    }

    public void HideCard()
    {
        isFlipped = false;
        if ((gameManager ?? GameManagerCard.Instance) != null && cardImage != null)
            cardImage.sprite = (gameManager ?? GameManagerCard.Instance).CardBack;
    }
}
