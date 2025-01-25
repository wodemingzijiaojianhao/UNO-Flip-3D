using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Player> players = new List<Player>();
    [SerializeField] Deck deck;

    [SerializeField] Transform playerHandTransform; //HOLDS PLAYER HAND
    [SerializeField] List<Transform> aiHandTransform = new List<Transform>(); //HOLDS AI HANDS
    [SerializeField] GameObject cardPrefab;
    [SerializeField] int numberOfAiPlayers = 3;
    [SerializeField] int startingHandSize = 7;
    int currentPlayer = 0;
    [Header("Game Play")]
    [SerializeField] Transform discardPileTransform;
    [SerializeField] CardDisplay topCard;


    void Awake()
    {
        instance = this;

    }

    void Start()
    {
        //INITIALIZE DECK
        deck.InitializeDeck();

        //INITIALIZE PLAYERS
        InitializePlayers();

        //DEAL CARDS
        StartCoroutine(DealStartingCards());

        //START GAME
    }

    void InitializePlayers()
    {
        players.Clear();
        players.Add(new Player("Player",true));

        for (int i = 0; i < numberOfAiPlayers; i++)
        {
            players.Add(new AiPlayer("AI" + (i+1)));
        }
    }

    void DealCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                player.DrawCard(deck.DrawCard());
            }
        }

        //DISPLAY CARDS
        StartCoroutine(DealStartingCards());

        //DISPLAY AI HAND  
    }

    IEnumerator DealStartingCards()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            foreach (Player player in players)
            {
                Card drawnCard = deck.DrawCard();
                player.DrawCard(drawnCard);

                //VISUALISE CARDS
                Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
                GameObject card = Instantiate(cardPrefab, hand, false);

                //DRAW CARDS
                CardDisplay cardDisplay =  card.GetComponentInChildren<CardDisplay>();
                cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN

                //FOR AI PLAYERS
                if (player.IsHuman)
                {
                    //SHOW BACK OF CARD
                    cardDisplay.ShowCard();
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
        //START GAME
        Debug.Log("Game Started");
    }

    public void PlayCard(CardDisplay cardDisplay)
    {
        Card cardToPlay = cardDisplay.MyCard;
        //REMOVE CARD FROM PLAYER HAND
        players[currentPlayer].PlayCard(cardToPlay);
        //UNHIDE THE CARD IF AI PLAYER

        //MOVE THE CARD TO DISCARD PILE
        MoveCardToPile(cardDisplay.transform.parent.gameObject);
        //UPDATE TOP CARD
        topCard = cardDisplay;
        //IMPLEMENT WHAT SHOULD HAPPEN WHEN CARD PLAYED
        OnCardPlayed();
    }

    void MoveCardToPile(GameObject currentCard)
    {
        currentCard.transform.SetParent(discardPileTransform);
        currentCard.transform.localPosition = Vector3.zero;
        //currentCard.transform.localScale = Vector3.one;

        //UNHIDE CARD
    }
    void OnCardPlayed()
    {

    }

    public void DrawCardFromDeck()
    {
        Card drawnCard = deck.DrawCard();
        Player player = players[currentPlayer];

        if (drawnCard!=null)
        {
            player.DrawCard(drawnCard);

            //VISUALISE CARDS
            Transform hand = player.IsHuman ? playerHandTransform : aiHandTransform[players.IndexOf(player)-1];
            GameObject card = Instantiate(cardPrefab, hand, false);

            //DRAW CARDS
            CardDisplay cardDisplay =  card.GetComponentInChildren<CardDisplay>();
            cardDisplay.SetCard(drawnCard, player); //ONLY FOR HUMAN

            //FOR AI PLAYERS
            if (player.IsHuman)
            {
                //SHOW BACK OF CARD
                cardDisplay.ShowCard();
            }
        }
        
    }

}