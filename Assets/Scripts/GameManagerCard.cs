using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameManagerCard : MonoBehaviour
{
    public static GameManagerCard Instance;
    public Card cardPrefab;
    public Sprite cardBack;
    public Sprite[] cardFaces;
    private List<Card> cards;
    private List<int> cardIDs;
    public Card firstCard, secondCard;
    public Transform cardHolder;
    public GameObject finalUI;
    public TextMeshProUGUI finalText;
    public TextMeshProUGUI timerText;
    private int pairsMatched;
    private int totalPairs;
    private float timer;
    private bool isGameOver;
    private bool isLevelFinished;
    public float maxTime = 120f;

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

        CreateCards();
        ShuffleCards();

        finalUI.gameObject.SetActive(false);
    }

    void Update()
    {
       if (!isGameOver && !isLevelFinished)
        {
            if (timer > 0)
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
            newCard.cardID = id;
            cards.Add(newCard);
        }
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cardIDs.Count);
            int temp = cardIDs[i];
            cardIDs[i] = cardIDs[randomIndex];
            cardIDs[randomIndex] = temp;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].cardID = cardIDs[i];
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
        if (firstCard.cardID == secondCard.cardID)
        {
            pairsMatched++;

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
        yield return new WaitForSeconds(1f);
        firstCard.HideCard();
        secondCard.HideCard();
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
        finalUI.gameObject.SetActive(true);
        if (isLevelFinished)
        {
            finalText.text = "Congratulations! Time Taken: " + Mathf.Round(maxTime - timer) + "s";
        }
        else if (isGameOver)
        {
            finalText.text = "Time's up! Try again!";
        }
    }

    public void RestartGame()
    {
        pairsMatched = 0;
        timer = maxTime;
        isGameOver = false;
        isLevelFinished = false;
        finalUI.gameObject.SetActive(false);

        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
        cardIDs.Clear();

        CreateCards();
        ShuffleCards();
    }

    void UpdateTimerText()
    {
        timerText.text = "Time Left: " + Mathf.Round(timer) + "s";
    }


}
