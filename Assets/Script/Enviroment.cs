using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enviroment : MonoBehaviour
{
	public static int GamePhase = 0;	//GamePhase: 0->init(双方各抽一张), 1->self, 2->none, 3->enemy, 4->none
										//normally : 0-1-2-3-4-1-2-3...0-1-2-3...
    public static int SelfCardOnDeskNum = 0;
    public static int CardOnSelfHand = 0;
	public static int AnotherCardOnDeskNum = 0;
	public static int CardOnAnotherHand = 0;
	const int CardSum = 32;
	const int CardStart = 10;
    int TimeCon;
    static int[] Deck;
    int DeckLeft;
	public static bool InitReady, InitSelfReady, InitAnotherReady;

    static int[] DeckSelf = new int[CardSum / 2], DeckAnother = new int[CardSum / 2];
	static GameObject DeckSelfobj, DeckAnotherobj, PlayedSelfobj;
	static GameObject HandSelfobj, HandAnotherobj, PlayedAnotherobj;
	static int SelfPoint, AnotherPoint;

	GameObject PrefabCard;
	static Sprite[] cardSprite;
	public static Sprite GetSprite(int value)
	{
		return cardSprite[value];
	}

	void SendCard()
    {
        for(int i = 0; i < CardStart; i++)
        {
            GameObject newCard = Instantiate(PrefabCard);
			newCard.transform.parent = HandSelfobj.transform;
            newCard.transform.position = new Vector3(-3.0f + 0.5f * i, -3.2f, -i / 10.0f);
			newCard.GetComponent<CardEvent>().Set(DeckSelf[i] - 1);
			newCard.GetComponent<CardEvent>().SetImage(true);
			newCard.GetComponent<CardEvent>().SetPlayer(true);
			newCard.GetComponent<SpriteRenderer>().sortingOrder = i;

			newCard = Instantiate(PrefabCard);
			newCard.transform.parent = HandAnotherobj.transform;
			newCard.transform.position = new Vector3(3.0f - 0.5f * i, 3.2f, -i / 10.0f);
			newCard.GetComponent<CardEvent>().Set(DeckAnother[i] - 1);
			newCard.GetComponent<CardEvent>().SetImage(true);
			newCard.GetComponent<CardEvent>().SetPlayer(false);
			newCard.GetComponent<SpriteRenderer>().sortingOrder = 10 + i;
		}
		for (int i = CardStart; i < CardSum/2; i++)
		{
			GameObject newCard = Instantiate(PrefabCard);
			newCard.transform.parent = DeckSelfobj.transform;
			newCard.transform.position = new Vector3(-7.0f, -3.2f, -i / 10.0f);
			newCard.GetComponent<CardEvent>().Set(DeckSelf[i] - 1);
			newCard.GetComponent<CardEvent>().SetImage(false);
			newCard.GetComponent<CardEvent>().SetPlayer(true);
			newCard.GetComponent<SpriteRenderer>().sortingOrder = i;

			newCard = Instantiate(PrefabCard);
			newCard.transform.parent = DeckAnotherobj.transform;
			newCard.transform.position = new Vector3(7.0f, 3.2f, -i / 10.0f);
			newCard.GetComponent<CardEvent>().Set(DeckAnother[i] - 1);
			newCard.GetComponent<CardEvent>().SetImage(false);
			newCard.GetComponent<CardEvent>().SetPlayer(false);
			newCard.GetComponent<SpriteRenderer>().sortingOrder = 10 + i;
		}
	}

	void SortCard(GameObject cardset, bool sa)
	{
		List<GameObject> card = new List<GameObject>();
		foreach(Transform child in cardset.transform)
		{
			card.Add(child.gameObject);
		}
		card.Sort((left, right) =>
		{
			if (left.GetComponent<CardEvent>().Value > right.GetComponent<CardEvent>().Value)
				return 1;
			else
				return -1;
		});
		for (int i = 0; i < CardStart; i++)
		{
			if (sa)
				card[i].transform.position = new Vector3(-3.0f + i * 0.6f, -3.2f, -i / 10.0f);
			else
				card[i].transform.position = new Vector3(3.0f - i * 0.6f, 3.2f, -i / 10.0f);
			card[i].GetComponent<SpriteRenderer>().sortingOrder = i;
		}
	}

    void Shuffle()
    {
        System.Random x = new System.Random();
        for (int i=0; i< CardSum; i++)
        {
            int j = x.Next(i);
            int k = Deck[i];
            Deck[i] = Deck[j];
            Deck[j] = k;
        }
        for(int i = 0; i < CardSum; i++)
        {
            if (i % 2 == 0)
            {
                DeckSelf[i / 2] = Deck[i];
            }
            else
            {
                DeckAnother[i / 2] = Deck[i];
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PrefabCard			= Resources.Load("Prefabs/card", typeof(GameObject)) as GameObject;
		cardSprite			= Resources.LoadAll<Sprite>("Card");
		DeckSelfobj			= GameObject.Find("deckself");
		HandSelfobj			= GameObject.Find("handself");
		PlayedSelfobj		= GameObject.Find("playedcardself");
		DeckAnotherobj		= GameObject.Find("deckanother");
		HandAnotherobj		= GameObject.Find("handanother");
		PlayedAnotherobj	= GameObject.Find("playedcardanother");
		SelfPoint			= 0;
		AnotherPoint		= 0;
		TimeCon				= 0;
		InitReady = false; InitSelfReady = false; InitAnotherReady = false;
		GamePhase = 0;
        DeckLeft = CardSum;
        Deck = new int[]{ 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 7, 7, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9 };
        Shuffle();
        Debug.Log(string.Join(", ", DeckSelf));
        Debug.Log(string.Join(", ", DeckAnother));
        SendCard();
		SortCard(HandSelfobj, true);
		SortCard(HandAnotherobj, false);
		return;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCon++;
		
	}

	static public void AddPoint(int num, bool player)
	{
		int score = num;
		if (num > 7) score = 1;
		if (num == -1)
		{
			if (player) score = -SelfPoint;
			else		score = -AnotherPoint;
		}
		if (player)
		{
			SelfPoint += score;
			GameObject.Find("scoreself").GetComponent<Text>().text = SelfPoint.ToString();
		}
		else
		{
			AnotherPoint += score;
			GameObject.Find("scoreanother").GetComponent<Text>().text = AnotherPoint.ToString();
		}
	}

	static public void Draw(bool player, GameObject obj)
	{
		if (player == true) { InitSelfReady = true; obj.transform.parent = PlayedSelfobj.transform; }
		if (player == false) { InitAnotherReady = true; obj.transform.parent = PlayedAnotherobj.transform; }
		if(InitAnotherReady==true && InitSelfReady == true)
		{
			if (SelfPoint > AnotherPoint) GamePhase = 3;
			if (SelfPoint < AnotherPoint) GamePhase = 1;
			if (SelfPoint == AnotherPoint)
				SoftRestart();
		}
	}

	static public void Lead(GameObject obj)
	{
		if (GamePhase == 1)
		{
			GamePhase = 3;
			obj.transform.parent = PlayedSelfobj.transform;
			return;
		}
		if (GamePhase == 3)
		{
			GamePhase = 1;
			obj.transform.parent = PlayedAnotherobj.transform;
			return;
		}
	}

	static public void CheckWinner()
	{
		if (GamePhase == 0) return;
		if (GamePhase == 1 && SelfPoint < AnotherPoint) { return; }
		if (GamePhase == 3 && SelfPoint > AnotherPoint) { return; }
		if (SelfPoint < AnotherPoint) { return; }
		if (SelfPoint > AnotherPoint) { return; }
		if (SelfPoint == AnotherPoint)
			SoftRestart();
	}

	static void DeleteDeskCard()
	{
		foreach (Transform child in PlayedSelfobj.transform)
		{
			child.gameObject.GetComponent<CardEvent>().StartMove(10, new Vector2((8.0f - child.position.x) / 10,
					  (-1.0f - child.position.y) / 10));
		}
		foreach (Transform child in PlayedAnotherobj.transform)
		{
			child.gameObject.GetComponent<CardEvent>().StartMove(10, new Vector2((-8.0f - child.position.x) / 10,
					  (1.0f - child.position.y) / 10));
		}
	}

	static void SoftRestart()
	{
		DeleteDeskCard();
		SelfPoint = 0;
		AnotherPoint = 0;
		AddPoint(-1, true);
		AddPoint(-1, false);
		GamePhase = 0;
		InitSelfReady = false;
		InitAnotherReady = false;
	}
}
