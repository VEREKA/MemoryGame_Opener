using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private TextMeshProUGUI pairsText;
    
    private int pairsMatched;
    private int totalPairs;
    private float elapsedTime;
    private bool isGameOver;
    private bool isLevelFinished;
    [SerializeField] private float maxTime = 120f;
    [SerializeField] private float revealDuration = 1f;
    private int lastDisplayedMs = -1;
    private bool isInitialized = false;

    public Sprite[] CardFaces => cardFaces;
    public Sprite CardBack => cardBack;
    public float MaxTime => maxTime;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private string gameSceneName = "Game scene";
    [SerializeField] private string endSceneName = "End";

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
        if (!string.IsNullOrEmpty(gameSceneName) && SceneManager.GetActiveScene().name != gameSceneName)
        {
            Debug.Log("GameManagerCard: skipping initialization (not in game scene): " + SceneManager.GetActiveScene().name, this);
            return;
        }

        cards = new List<Card>();
        cardIDs = new List<int>();
        pairsMatched = 0;
        totalPairs = cardFaces.Length;

        elapsedTime = 0f;
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

        UpdatePairsText();

        if (finalUI != null) finalUI.gameObject.SetActive(false);
        isInitialized = true;
    }

    void Update()
    {
    if (!isInitialized) return;

    if (!isGameOver && !isLevelFinished)
        {
            if (elapsedTime < maxTime)
            {
                elapsedTime += Time.deltaTime;
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

            UpdatePairsText();

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
        LoadEndScene();
    }

    void LevelFinished()
    {
        isLevelFinished = true;
        LoadEndScene();
    }

    public void FinalPanel()
    {
        if (finalUI != null) finalUI.gameObject.SetActive(true);
        if (isLevelFinished)
        {
            if (finalText != null) finalText.text = "Czas " + FormatTimeMs(elapsedTime);
        }
        else if (isGameOver)
        {
            if (finalText != null) finalText.text = "Time's up! Try again!";
        }
    }

    public void RestartGame()
    {

        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMenuScene()
    {
        if (!string.IsNullOrEmpty(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }

    public void LoadGameScene()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }

    public void LoadEndScene()
    {
        if (!string.IsNullOrEmpty(endSceneName))
            SceneManager.LoadScene(endSceneName);
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;
        int displayMs = Mathf.FloorToInt(elapsedTime * 1000f);
        if (displayMs != lastDisplayedMs)
        {
            timerText.text = "Czas " + FormatTimeMs(elapsedTime);
            lastDisplayedMs = displayMs;
        }
    }

    void UpdatePairsText()
    {
        if (pairsText == null) return;
        pairsText.text = pairsMatched.ToString() + "/" + totalPairs.ToString();
    }

    string FormatTimeMs(float seconds)
    {
        int totalMs = Mathf.FloorToInt(Mathf.Clamp(seconds, 0f, maxTime) * 1000f);
        int mins = totalMs / 60000;
        int secs = (totalMs % 60000) / 1000;
        int ms = totalMs % 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", mins, secs, ms);
    }


}
