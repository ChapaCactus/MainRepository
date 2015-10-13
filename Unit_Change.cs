using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Unit_Change : MonoBehaviour
{
	private int selectWindowSize = 0;

	List<GameObject> selectWindowList  = new List<GameObject>();
	List<GameObject> teamAWindowList = new List<GameObject>();

	private bool changeUnitIsRunning = false;
	private bool isPush = false;

	private int numberFrom, numberTo;

	UnitData[] unitData, teamA, keepData;

	void Start ()
	{
		EditList();
	}

	public int GetSelectWindowCount ()
	{
		GameObject[] selWindowCount = GameObject.FindGameObjectsWithTag( "SelectWindow" );
		Debug.Log ( selWindowCount.Length );

		return selWindowCount.Length;
	}

	public void EditList ()
	{
		int maxCount = GetSelectWindowCount();
		unitData = new UnitData[maxCount];
		teamA = new UnitData[6];
		keepData = new UnitData[1];

		for ( int i = 0; i < maxCount; i++ )
		{
			//int randomA = Random.Range( 0, 6 );

			string charName = GetCharName ( i );

			unitData[i] = new UnitData( ( i ), charName, ( i ) );
			teamA[i] = new UnitData( ( i ), charName, ( i ) );

			GameObject selWindow = GameObject.Find ( "CharSelect_List (" + i.ToString() + ")" );
			selectWindowList.Add( selWindow );
			GameObject teamAWindow = GameObject.Find ( "UnitSlot (" + i.ToString() + ")" );
			teamAWindowList.Add( teamAWindow );

			Debug.Log ( selectWindowList[ i ] );
			Debug.Log ( selectWindowList.Count );
			// 部隊外分
			GameObject childSprite = selectWindowList[i].gameObject.transform.FindChild("CharSprite_Panel/CharSprite_List (" + i.ToString() + ")"  ).gameObject;
			childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );
			GameObject childText = GameObject.Find ( "CharText_List (" + i + ")" );
			charName = GetCharName ( unitData[i].char_no );
			childText.GetComponent<Text>().text = charName;
			// 部隊分
			childSprite = teamAWindowList[i].gameObject.transform.FindChild("SpritePanel/SpriteSlot (" + i.ToString() + ")"  ).gameObject;
			childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );
			//GameObject childText = GameObject.Find ( "CharText_List (" + i + ")" );
			//childText.GetComponent<Text>().text = charName;
		}
	}

	public void RefreshListOnly ()
	{
		Debug.Log ( " RefreshListOnly RUNNED " );
		int maxCount = GetSelectWindowCount();

		for ( int i = 0; i < maxCount; i++ )
		{
			/*
			GameObject selWindow = GameObject.Find ( "CharSelect_List (" + i.ToString() + ")" );
			selectWindowList.Add( selWindow );
			GameObject teamAWindow = GameObject.Find ( "UnitSlot (" + i.ToString() + ")" );
			teamAWindowList.Add( teamAWindow );
			*/

			GameObject childSprite = GameObject.Find( "CharSprite_List (" + i.ToString() + ")"  ).gameObject;
			childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );
			GameObject childText = GameObject.Find ( "CharText_List (" + i + ")" );
			string charName = GetCharName ( unitData[i].char_no );
			childText.GetComponent<Text>().text = charName.ToString();
			// teamAの入れ替え
			GameObject teamASprite = GameObject.Find( "SpriteSlot (" + i.ToString() + ")"  ).gameObject;
			teamASprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + teamA[i].char_no.ToString() );

			Debug.Log ( i.ToString() + ": " + unitData[i].char_no + "::  " + teamA[i].char_no );
		}
	}

	public string GetCharName ( int num )
	{
		string name = "";

		switch ( num )
		{
			case 0 :
				name = "まどか";
				break;

			case 1 :
				name = "ほむら";
				break;

			case 2 :
				name = "さやか";
				break;
			case 3 :
				name = "きょうこ";
				break;
			case 4 :
				name = "マミ";
				break;
			case 5 :
				name = "キュウべえ";
				break;

			default :
				name = "けつばん";
				break;

		}

		return name;
	}

	public void ChangeUnitFrom ( int slotNumber )
	{
		StopCoroutine ( "WaitIsPush" );
		numberFrom = slotNumber;
		Debug.Log ( numberFrom );
		StartCoroutine ( "WaitIsPush" );
	}
	// この関数で
	public void ChangeUnitTo ( int slotNumber )
	{
		numberTo = slotNumber;
		Debug.Log ( numberTo );

		isPush = true;
	}
	// この関数で数値の受け渡しを行います
	public void ChangeUnitFromTo ()
	{
		int keepNum = 0;
		//Debug.Log ( "Keep" + keepNum + "From" +numberFrom + "To: " + numberTo );
/*
		keepNum = numberFrom;
		Debug.Log ( "(1/3)...keepNum に numberFrom を 代入しました" );
		numberFrom = numberTo;
		Debug.Log ( "(2/3)...numberFrom に numberTo を 代入しました" );
		numberTo = keepNum;
		Debug.Log ( "(3/3)...numberTo に keepNum を 代入しました" );

		Debug.Log ( "Keep" + keepNum + "From" +numberFrom + "To: " + numberTo );
*/
		keepData[0] = teamA[numberFrom];
		teamA[numberFrom] = unitData[numberTo];
		unitData[numberTo] = keepData[0];

		Debug.Log ( "bbb" + teamA[numberFrom].char_no );
		Debug.Log ( "aaa" + unitData[numberTo].char_no );
		/*// 部隊外
		GameObject childSprite = GameObject.Find("CharSprite_List (" + unitData[numberFrom].char_no.ToString() + ")"  ).gameObject;
		childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[numberFrom].char_no.ToString() );
		// 部隊分
		childSprite = GameObject.Find("SpriteSlot (" + unitData[numberTo].char_no.ToString() + ")"  ).gameObject;
		childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + teamA[numberTo].char_no.ToString() );
*/
		changeUnitIsRunning = false;
		RefreshListOnly();
		Debug.Log ( "From" +numberFrom + "← Keep" + keepNum +  "← To: " + numberTo + "← Keep" + keepNum );
		Debug.Log ( "UnitData[1] "+ unitData[1].char_no );
		Debug.Log ( "teamA[0] "+ teamA[0].char_no );

		return;
	}

	IEnumerator WaitIsPush ()
	{
		if ( changeUnitIsRunning ) { yield break; }
		changeUnitIsRunning = true;

		while( !isPush )
		{
			Debug.Log ( "入力待ちです。" );
			yield return 0;
		}

		ChangeUnitFromTo();

		yield break;
	}
}

public class UnitData
{
// ----------- 各ユニット固有の値
	public int char_no = 0;
	public string char_name = "";
	public int groups;
// ----------- コスト等として増減する値
	public int useSlot, maxSlot = 1;
	public int hp, maxHp;
	public int fuel, maxFuel;
	public int bullet, maxBullet;
// ----------- 戦闘処理に関わる値
	public int long_power, middle_power, short_power, shield_power;
	public int defense, avoidance, speed;
	public int sphit, searchEnemy, luck, exp;
// -----------
	public UnitData ( int char_no, string char_name, int defense )
	{
		this.char_no = char_no;
		this.char_name = char_name;
		this.defense = defense;

		/*int char_no, string char_name, int groups, int useSlot, int maxSlot, int hp, int maxHp, int fuel
		, int maxFuel, int bullet, int maxBullet, int long_power, int middle_power, int short_power, int shield_power
		, int defense, int avoidance, int speed, int sphit, int searchEnemy, int luck, int exp*/ // 引数たち
	}
}