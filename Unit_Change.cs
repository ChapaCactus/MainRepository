﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Unit_Change : MonoBehaviour
{
	// ユニットの基礎攻撃力・ユニットの現在合計攻撃力・武器の基礎攻撃力・武器切り替えた後の合計攻撃力
	List<GameObject> selectWindowList  = new List<GameObject>(), teamAWindowList = new List<GameObject>();

	private bool changeUnitIsRunning = false;// コルーチンの多重起動防止用
	private bool isApply = false;// 二つ目が押されたかどうか

	private int numberFrom, numberTo;// 入れ替え用、OnClick時の引数を元にする
	private int weaponNumFrom, weaponNumTo;

	UnitData[] unitData, teamA, keepData;
	WeaponData[] weaponData;

	GameObject _unitDataWindow, _weaponAWindow;

	void Start ()
	{
		EditList();

		_unitDataWindow = GameObject.Find ( "CharTeam_WIndow (0)" );
		_weaponAWindow = GameObject.Find ( "ChangeWeapon_WIndow (0)" );
		_unitDataWindow.transform.localPosition = new Vector2 ( -800, -15 );
		_weaponAWindow.transform.localPosition = new Vector2 ( 800, -15 );
		//_unitDataWindow.gameObject.SetActive ( false );
		//_weaponAWindow.gameObject.SetActive ( false );
	}

	public int GetSelectWindowCount ( string windowName )
	{
		// Scene内の同タグオブジェクトを全て検索。見つかったウィンドウの数を返す
		GameObject[] selWindowCount = GameObject.FindGameObjectsWithTag( windowName );
		Debug.Log ( "シーン内にある " + windowName + " の数は " + selWindowCount.Length + "です" );

		return selWindowCount.Length;
	}

	public void EditList ()
	{
		int maxCount = GetSelectWindowCount( "UnitSelectWindow");
		unitData = new UnitData[maxCount];
		teamA = new UnitData[6];
		keepData = new UnitData[1];

		//maxCount = GetSelectWindowCount( "WeaponSelectWindow");
		weaponData = new WeaponData[6];

		for ( int i = 0; i < maxCount; i++ )
		{
			string charName = GetCharName ( i );
			string weaponName = GetWeaponName ( i );

			// -----
			weaponData[i] = new WeaponData ( i,  weaponName, ( ( i + 1 )  * 2 ) );
			GameObject weaponText = GameObject.Find ( "WeaponText_List (" + i + ")" );
			weaponText.GetComponent<Text>().text = weaponData[i].weapon_name;
			// -----

			unitData[i] = new UnitData( ( i ), charName, ( i ) );
			teamA[i] = new UnitData( ( i ), charName, ( i ) );

			GameObject selWindow = GameObject.Find ( "CharSelect_List (" + i.ToString() + ")" );
			selectWindowList.Add( selWindow );
			GameObject teamAWindow = GameObject.Find ( "UnitSlot (" + i.ToString() + ")" );
			teamAWindowList.Add( teamAWindow );
			GameObject teamAPowerText = GameObject.Find ( "PowerInfo_Text (" + i + ")" );
			teamAPowerText.GetComponent<Text>().text = ( "合計攻撃力:" + teamA[i].attack.ToString() );
			GameObject teamACharText = GameObject.Find ( "TeamACharText_List (" + i + ")" );
			teamACharText.GetComponent<Text>().text = GetCharName (i);

			// 部隊外分
			GameObject childSprite = selectWindowList[i].gameObject.transform.FindChild("CharSprite_Panel/CharSprite_List (" + i.ToString() + ")"  ).gameObject;
			childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );
			GameObject childText = GameObject.Find ( "CharText_List (" + i + ")" );
			charName = GetCharName ( unitData[i].char_no );
			childText.GetComponent<Text>().text = charName;
			// 部隊分
			GameObject teamASprite = GameObject.Find ("SpriteSlot (" + i.ToString() + ")"  ).gameObject;
			teamASprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );
		}
	}

	public void RefreshListOnly ()
	{
		Debug.Log ( " RefreshListOnly was RUNNED " );
		int maxCount = GetSelectWindowCount( "UnitSelectWindow" );
		// weaponウィンドウとunitウィンドウ両方を個別に更新するべき？
		for ( int i = 0; i < maxCount; i++ )
		{
			GameObject childSprite = GameObject.Find( "CharSprite_List (" + i.ToString() + ")"  ).gameObject;
			childSprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + unitData[i].char_no.ToString() );

			GameObject childText = GameObject.Find ( "CharText_List (" + i + ")" );
			string charName = GetCharName ( unitData[i].char_no );
			childText.GetComponent<Text>().text = charName.ToString();
			GameObject teamAPowerText = GameObject.Find ( "PowerInfo_Text (" + i + ")" );
			teamAPowerText.GetComponent<Text>().text = ( "合計攻撃力:" + teamA[i].attack.ToString() );
			// teamAの入れ替え
			GameObject teamASprite = GameObject.Find( "SpriteSlot (" + i.ToString() + ")"  ).gameObject;
			teamASprite.GetComponent<Image>().sprite = Resources.Load<Sprite> ( "Sprites/Character/MiniChar_" + teamA[i].char_no.ToString() );

			isApply = false;
		}
	}

	public void ChangeUnitFrom ( int slotNumber )
	{
		Sound_Manager.Instance.PlaySE(1);
		_unitDataWindow.transform.localPosition = new Vector2 ( 0, -15 );
		//_unitDataWindow.gameObject.SetActive ( true );
		StopCoroutine ( "WaitForApply" );// 既にコルーチンが起動していた場合それを停止 ( 選択しなおす )

		numberFrom = slotNumber;
		Debug.Log ( "「" + numberFrom + "」" + " が選択されました…入れ替え先 ( 部隊外ウィンドウ ) を選択してください。" );

		StartCoroutine ( "WaitForApply" );
	}

	public void ChangeUnitTo ( int slotNumber )
	{
		Sound_Manager.Instance.PlaySE(1);
		_unitDataWindow.transform.localPosition = new Vector2 ( -800, -15 );
		numberTo = slotNumber;
		Debug.Log ( "「" + numberTo + "」" + " が選択されました…対応する配列の入れ替えを行います" );

		isApply = true;
	}
	// この関数で数値の受け渡しを行う
	public void ChangeUnitFromTo ()
	{
		// 変数の入れ替え。配列keepDataに一旦保存して受け渡しを行う
		keepData[0] = teamA[numberFrom];// AをXに
		teamA[numberFrom] = unitData[numberTo];// BをAに
		unitData[numberTo] = keepData[0];// XをAに

		changeUnitIsRunning = false;
		RefreshListOnly();// リストを更新する
		//_unitDataWindow.gameObject.SetActive ( false );

		return;
	}

	public void ChangeWeaponFrom ( int slotNumber )
	{
		Sound_Manager.Instance.PlaySE(1);
		_weaponAWindow.transform.localPosition = new Vector2 ( 0, -15 );
		//_weaponAWindow.gameObject.SetActive ( true );
		StopCoroutine ( "WaitForWeaponApply" );// 既にコルーチンが起動していた場合それを停止 ( 選択しなおす )

		numberFrom = slotNumber;
		Debug.Log ( "「" + numberFrom + "」" + " が選択されました…入れ替え先 ( 部隊外ウィンドウ ) を選択してください。" );

//		GameObject unitOriginText = GameObject.Find ("UnitOriginPower" );
//		unitOriginText.GetComponent<Text>().text = teamA[numberFrom].attack.ToString();
		GameObject unitCurrentText = GameObject.Find ("UnitCurrentPower" );
		unitCurrentText.GetComponent<Text>().text = teamA[numberFrom].attack.ToString();

		StartCoroutine ( "WaitForWeaponApply" );
	}

	public void ChangeWeaponTo ( int slotNumber )
	{
		Sound_Manager.Instance.PlaySE(1);
		numberTo = slotNumber;
		Debug.Log ( "「" + numberTo + "」" + " が選択されました…決定で対応する配列の入れ替えを行います" );

//		GameObject weaponOriginText = GameObject.Find ("WeaponOriginPower");
//		weaponOriginText.GetComponent<Text>().text = weaponData[numberTo].attack.ToString();
		GameObject unitAmountText = GameObject.Find ("UnitAmountPower");
		unitAmountText.GetComponent<Text>().text = (weaponData[numberTo].attack + teamA[numberFrom].attack ).ToString();
	}

	public void ChangeWeaponFromTo ()
	{
		// 変数の入れ替え。配列keepDataに一旦保存して受け渡しを行う
		//keepWaeponData[0].attack = teamA[numberFrom].attack;// AをXに
		teamA[numberFrom].attack += weaponData[numberTo].attack;// BをAに
		//weaponData[numberTo] = keepWaeponData[0];// XをAに

		changeUnitIsRunning = false;
		isApply = false;
		RefreshListOnly();// リストを更新する
//
		Debug.Log ( this.gameObject.name + "ユニットの装備を入れ替えました。" );
		//_weaponAWindow.gameObject.SetActive ( false );

		return;
	}
	// 最終決定 後で名称変える
	public void WeaponApply ()
	{
		Sound_Manager.Instance.PlaySE(1);
		_weaponAWindow.transform.localPosition = new Vector2 ( 800, -15 );
		Debug.Log ( "装備の入れ替えを確定します。" );
		isApply = true;
	}

	IEnumerator WaitForApply ()
	{
		if ( changeUnitIsRunning ) { yield break; }

		changeUnitIsRunning = true;

		while( !isApply )
		{
			Debug.Log ( "入力待ちです。" );
			yield return 1;
		}

		ChangeUnitFromTo();

		yield break;
	}

	IEnumerator WaitForWeaponApply ()
	{
		if ( changeUnitIsRunning ) { yield break; }

		changeUnitIsRunning = true;

		while( !isApply )
		{
			Debug.Log ( "入力待ちです。" );
			yield return 1;
		}

		ChangeWeaponFromTo();

		yield break;
	}

	// char_noにより名称を返す。仮データなので、そのうちデータベースからちゃんと引っ張るようにする。
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
				name = "けつばんくん";
				break;
		}

		return name;
	}

	public string GetWeaponName ( int num )
	{
		string name = "";

		switch ( num )
		{
			case 0 :
				name = "キラー";
				break;

			case 1 :
				name = "緑こうら";
				break;

			case 2 :
				name = "赤こうら";
				break;

			case 3 :
				name = "トリプル緑こうら";
				break;

			case 4 :
				name = "トリプル赤こうら";
				break;

			case 5 :
				name = "バナナ";
				break;

			default :
				name = "けつばんアイテム";
				break;
		}

		return name;
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
	public int attack;
	public int long_power, middle_power, short_power, shield_power;
	public int defense, avoidance, speed;
	public int sphit, searchEnemy, luck, exp;
// -----------



	public UnitData ( int char_no, string char_name, int attack )
	{
		this.char_no = char_no;
		this.char_name = char_name;
		this.attack = attack;

		/*int char_no, string char_name, int groups, int useSlot, int maxSlot, int hp, int maxHp, int fuel
		, int maxFuel, int bullet, int maxBullet, int long_power, int middle_power, int short_power, int shield_power
		, int defense, int avoidance, int speed, int sphit, int searchEnemy, int luck, int exp*/ // 引数たち
	}
}

public class WeaponData
{
// ----------- 各ユニット固有の値
	public int weapon_no = 0;
	public string weapon_name = "";
	public int attack = 0;
// -----------
	public WeaponData ( int weapon_no, string weapon_name, int attack )
	{
		this.weapon_no = weapon_no;
		this.weapon_name = weapon_name;
		this.attack = attack;
	}
}