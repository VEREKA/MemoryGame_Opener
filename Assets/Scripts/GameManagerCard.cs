using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameManagerCard : MonoBehaviour
{
    public static GameManagerCard Instance { get; private set; }
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] cardFaces;
    private List<Card> cards;
    private List<int> cardIDs;
    private Card firstCard, secondCard;
    public Card FirstCard => firstCard;
    public Card SecondCard => secondCard;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject finalUI;
    [SerializeField] private TextMeshProUGUI finalText;
    [SerializeField] private TextMeshProUGUI timerText;
    private int pairsMatched;
    private int totalPairs;
    private float timer;
    private bool isGameOver;
    private bool isLevelFinished;
    [SerializeField] private float maxTime = 240f;
    [SerializeField] private float revealDuration = 1f;
    private int lastDisplayedTime = -1;

    public Sprite[] CardFaces => cardFaces;
    public Sprite CardBack => cardBack;
    public float MaxTime => maxTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cards = new List<Card>();
        cardIDs = new List<int>();
        pairsMatched = 0;
        totalPairs = cardFaces.Length;

        timer = maxTime;
        isGameOver = false;
        isLevelFinished = false;

        if (cardPrefab == null) Debug.LogError("cardPrefab not assigned on GameManagerCard", this);
        if (cardHolder == null) Debug.LogError("cardHolder not assigned on GameManagerCard", this);
        if (finalUI == null) Debug.LogError("finalUI not assigned on GameManagerCard", this);
        if (finalText == null) Debug.LogError("finalText not assigned on GameManagerCard", this);
        if (timerText == null) Debug.LogError("timerText not assigned on GameManagerCard", this);
        if (cardFaces == null || cardFaces.Length == 0) Debug.LogError("cardFaces not assigned or empty on GameManagerCard", this);

        CreateCards();
        ShuffleCards();

        if (finalUI != null) finalUI.gameObject.SetActive(false);
    }

    void Update()
    {
       if (!isGameOver && !isLevelFinished)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                GameOver();
            }
        }
    }

    void CreateCards()
    {
        for (int i = 0; i < cardFaces.Length; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        foreach (int id in cardIDs)
        {
            Card newCard = Instantiate(cardPrefab, cardHolder);
            newCard.gameManager = this;
            newCard.CardID = id;
            newCard.ResetCard();
            cards.Add(newCard);
        }
    }

    void ShuffleCards()
    {
        int n = cardIDs.Count;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(i, n);
            int temp = cardIDs[i];
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Count && i < cardIDs.Count; i++)
        {
            cards[i].CardID = cardIDs[i];
            cards[i].ResetCard();
        }
    }

    public void CardFlipped(Card flippedCard)
    {
        if (firstCard == null)
        {
            firstCard = flippedCard;
        }
        else if (secondCard == null)
        {
            secondCard = flippedCard;
            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstCard.CardID == secondCard.CardID)
        {
            pairsMatched++;

            firstCard.IsMatched = true;
            secondCard.IsMatched = true;
            firstCard.SetInteractable(false);
            secondCard.SetInteractable(false);

            if (pairsMatched == totalPairs)
            {
                LevelFinished();
            }

            firstCard = null;
            secondCard = null;
        }
        else
        {
            StartCoroutine(FlipBackCards());
        }
    }

    IEnumerator FlipBackCards()
    {
        yield return new WaitForSeconds(revealDuration);
        if (firstCard != null) firstCard.HideCard();
        if (secondCard != null) secondCard.HideCard();
        firstCard = null;
        secondCard = null;
    }

    void GameOver()
    {
        isGameOver = true;
        FinalPanel();
    }

    void LevelFinished()
    {
        isLevelFinished = true;
        FinalPanel();
    }

    public void FinalPanel()
    {
        if (finalUI != null) finalUI.gameObject.SetActive(true);
        if (isLevelFinished)
        {
            if (finalText != null) finalText.text = "Congratulations! Time Taken: " + Mathf.Round(maxTime - timer) + "s";
        }
        else if (isGameOver)
        {
            if (finalText != null) finalText.text = "Time's up! Try again!";
        }
    }

    public void RestartGame()
    {
        pairsMatched = 0;
        timer = maxTime;
        isGameOver = false;
        isLevelFinished = false;
        if (finalUI != null) finalUI.gameObject.SetActive(false);

        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
        cardIDs.Clear();

        lastDisplayedTime = -1;

        CreateCards();
        ShuffleCards();
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;
        int display = Mathf.RoundToInt(timer);
        if (display != lastDisplayedTime)
        {
            timerText.text = "Time Left: " + display + "s";
            lastDisplayedTime = display;
        }
    }


}
