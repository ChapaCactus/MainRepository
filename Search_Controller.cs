using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
// 未解決の変更点：気配察知に両方失敗・両方成功・味方のみ成功・敵のみ成功の４パターンが付く。
// 気配察知のアイコンを共通の物にするか？を考える

public class Search_Controller : MonoBehaviour
{
	private string whoLastAct = ""; // 最後に行動した者(索敵に負けた者)

	GameObject _debugText; // デバッグ時のみ使用する

	void Awake()
	{
		_debugText = GameObject.Find ( "Debug_Text" );
	}

	void Start()
	{
		StartCoroutine ( PhaseCtrlCoroutine() );// 進行管理コルーチンを起動する
	}

	// 進行管理を任意のタイミングで始めたい場合は、関数で起動する
	IEnumerator PhaseCtrlCoroutine ()
	{
		yield return new WaitForSeconds ( 1f );
		// 1.必要なデータの取得
		yield return StartCoroutine ( GetData() );// GetDataコルーチンの終了( yield return 0 が帰ってくるの) を待機

		// 2.戦闘開始、気配察知演出(絵コンテ①・②・③)
		yield return StartCoroutine ( PlaySearchAnimation() );

		// 3.ランダムで演出が入り、先に行動した者が勝利。(絵コンテ④・⑤)
		yield return StartCoroutine ( RandomStartPlayerOrEnemy() );

		// 4.結果送信。
		FinishSearchEnemy();

		yield break;

	}

	IEnumerator GetData()
	{
		// 絵コンテ①の前の段階にあたります。データを取得します(索敵するキャラの取得など？)
		yield return 0;// このコルーチンを終わって進行管理再開
	}

	// 一連のアニメーションは、索敵キャラ取得以外は毎回同じ処理なので完了を待たずに、
	// 実数値の時間経過を待って次の処理以降で良さそうです。
	IEnumerator PlaySearchAnimation ()
	{
		////部隊別索敵前のアニメーション(スライド)を表示します。

		DebugLogMaker ( "気配察知①" );
		// 電信が走るアニメーションはここ (絵コンテ①)

		//
		yield return new WaitForSeconds ( 3f );// 3秒待つ 要調整

		DebugLogMaker ( "気配察知②" );
		// 目元が流れるアニメーションはここ (絵コンテ②)

		//
		yield return new WaitForSeconds ( 3f );

		DebugLogMaker ( "気配察知③" );
		// 左右から文字が流れるアニメーションはここ (絵コンテ③)

		//
		yield return new WaitForSeconds ( 3f );


		yield return 0;// このコルーチンを終わって進行管理再開
	}

	IEnumerator RandomStartPlayerOrEnemy ()
	{
		// whichValueが０ならPlayerから、1ならエネミーから索敵が始まります。
		int whichValue = Random.Range ( 0, 2 );
		yield return new WaitForSeconds ( 0.5f );

		switch ( whichValue )
		{
			case 0:
				Debug.Log ( "whichValueは " + whichValue + "でした。PlayerSeachPhase コルーチンを起動します！" );

				yield return StartCoroutine ( PlayerSeachPhase() );
				yield return StartCoroutine ( EnemySeachPhase() );
				yield return 0;// このコルーチンを終わって進行管理再開

				break;

			case 1:
				Debug.Log ( "whichValueは " + whichValue + " でした。EnemySeachPhase コルーチンを起動します！" );

				yield return StartCoroutine ( EnemySeachPhase() );
				yield return StartCoroutine ( PlayerSeachPhase() );
				yield return 0;

				break;

			default:
				yield return StartCoroutine ( PlayerSeachPhase() );
				yield return StartCoroutine ( EnemySeachPhase() );
				yield return 0;// このコルーチンを終わって進行管理再開

				break;
		}

	}

	IEnumerator PlayerSeachPhase ()
	{
		whoLastAct = "Player";// 上書き
		DebugLogMaker ( "Playerの索敵開始です。" );
		// プレイヤーの索敵アニメーションはここ (絵コンテ④)

		//
		yield return new WaitForSeconds ( 3f );

		yield return 0;// このコルーチンを終わって進行管理再開
	}

	IEnumerator EnemySeachPhase ()
	{
		whoLastAct = "Enemy";// 上書き
		DebugLogMaker ( "Enemyの索敵開始です。" );
		// プレイヤーの索敵アニメーションはここ (絵コンテ⑤)

		//
		yield return new WaitForSeconds ( 3f );

		yield return 0;// このコルーチンを終わって進行管理再開
	}

	public void FinishSearchEnemy ()
	{
		if ( whoLastAct != "Player" )// 最後に行動した者がプレイヤーでなければ
		{
			DebugLogMaker ( "Playerの先行です。結果を送信します。（送信先は検討中。)" );
			// プレイヤーの勝利を送信
			// GameController又は陣形選択スクリプト自体に「終了」を伝えます。
			// 送り先Object.SendMessage ( "受け取る関数名",  "Player" );
		}
		else
		if ( whoLastAct == "Player" )// 最後に行動した者がプレイヤーなら
		{
			DebugLogMaker ( "Enemyの先行です。結果を送信します。（送信先は検討中。)" );
			// エネミーの勝利を送信
			// GameController又は陣形選択スクリプト自体に「終了」を伝えます。
			// 送り先Object.SendMessage ( "受け取る関数名",  "Enemy" );
		}

		return;
	}

///////////////// デバッグ用関数 /////////////////
	public void DebugLogMaker ( string log )
	{
		_debugText.GetComponent<Text>().text = log.ToString();

		Sound_Manager.Instance.PlaySE(1);
		Debug.Log ( log );

		return;
	}

}