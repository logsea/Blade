using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEvent : MonoBehaviour
{

    public bool CardOnHand;
	public bool CardOnDeck;
    bool MoveState;
    int MoveTime;
    Vector2 Speed;
	public int Value;
	bool player;

    public void Set(int num)
    {
		Value = num;
    }

	public void SetPlayer(bool b)
	{
		player = b;
	}

	public void SetImage(bool ud)
	{
		if (ud)
		{
			gameObject.GetComponent<SpriteRenderer>().sprite = Enviroment.GetSprite(Value);
			CardOnDeck = false;
		}
		else
		{
			gameObject.GetComponent<SpriteRenderer>().sprite = Enviroment.GetSprite(9);
			CardOnDeck = true;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		CardOnHand = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveState)
        {
            MoveTime--;
            gameObject.transform.Translate(Speed);
            if (MoveTime == 0)
                MoveState = false;
        }
    }

    void OnMouseEnter()
    {
        
    }

    void OnMouseDown()
    {
        if (CardOnHand)
        {
			if (Enviroment.GamePhase == 0 && CardOnDeck == false) return; //deal while need draw
			if (Enviroment.GamePhase != 0 && CardOnDeck != false) return; //draw while need deal
			switch (player)
			{
				case true:
					if ((Enviroment.GamePhase != 1 && Enviroment.GamePhase != 0) || (Enviroment.GamePhase == 0 && Enviroment.InitSelfReady == true)) return;
					break;
				case false:
					if ((Enviroment.GamePhase != 3 && Enviroment.GamePhase != 0) || (Enviroment.GamePhase == 0 && Enviroment.InitAnotherReady == true)) return;
					break;
			}
            MoveState = true;
            MoveTime = 10;
			if (player)
			{
				Speed = new Vector2((2.0f + Enviroment.SelfCardOnDeskNum * 0.5f - gameObject.transform.position.x) / MoveTime,
					  (-1.0f - gameObject.transform.position.y) / MoveTime);
				Enviroment.SelfCardOnDeskNum++;
				Enviroment.AddPoint(Value + 1, player);
			}
			else
			{
				Speed = new Vector2((-2.0f - Enviroment.AnotherCardOnDeskNum * 0.5f - gameObject.transform.position.x) / MoveTime,
					  (1.0f - gameObject.transform.position.y) / MoveTime);
				Enviroment.AnotherCardOnDeskNum++;
				Enviroment.AddPoint(Value + 1, player);
			}
			CardOnHand = false;
			gameObject.transform.position.Set(gameObject.transform.position.x, gameObject.transform.position.y,
				Enviroment.SelfCardOnDeskNum / 10f);
			gameObject.GetComponent<SpriteRenderer>().sortingOrder = Enviroment.SelfCardOnDeskNum;

			if (CardOnDeck == true) Enviroment.Draw(player, gameObject);
			else Enviroment.Lead(gameObject);

			Enviroment.CheckWinner();

			if (CardOnDeck)
			{
				SetImage(true);
			}
		}
    }

	public void StartMove(int time, Vector2 speed)
	{
		MoveState = true;
		MoveTime = time;
		Speed = speed;
	}
}
