using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HumanPlayerGround : PlayerGround {

	public static Stack<ushort> movesMade = new Stack<ushort>();
	public static HeapGround legalMoves;

	public override void Init (bool white) {
		base.Init (white);
	}

	/// <summary>
	/// Make a move that is known to be legal
	/// </summary>
	protected override void MakeMove (ushort move)
	{
		//UnityEngine.Debug.Log (move);
		//UnityEngine.Debug.Log (PGNReaderGround.NotationFromMove (move));
		base.MakeMove (move);
		movesMade.Push (move);
	}



	/// <summary>
	/// Makes the move after confirming that it is legal
	/// </summary>
	public void TryMakeMove(string algebraicMove) {
		if (isWhite == BoardGround.IsWhiteToPlay()) {
			legalMoves = moveGenerator.GetMoves(false,false);
			for (int i = 0; i < legalMoves.Count; i ++) {
				int moveFromIndex = legalMoves.GetMove(i) & 127;
				int moveToIndex = (legalMoves.GetMove(i) >> 7) & 127;
				
				int moveFromX = BoardGround.Convert128to64(moveFromIndex) % 8;
				int moveFromY = BoardGround.Convert128to64(moveFromIndex) / 8;
				int moveToX = BoardGround.Convert128to64(moveToIndex) % 8;
				int moveToY = BoardGround.Convert128to64(moveToIndex) / 8;
				
				
				string fromAlgebraic = DefinitionsGround.fileNames[moveFromX].ToString() + DefinitionsGround.rankNames[moveFromY].ToString();
				string toAlgebraic = DefinitionsGround.fileNames[moveToX].ToString() + DefinitionsGround.rankNames[moveToY].ToString();


				string moveCoords = fromAlgebraic + toAlgebraic;
				if (moveCoords == algebraicMove) { // move confirmed as legal
                    //Debug.Log(algebraicMove);
                    MakeMove(legalMoves.GetMove(i));
					break;
				}
			}
		}
	}

	public void TryMakeMove(ushort move) {
		MakeMove (move);
	}

	public override void RequestMove ()
	{
		base.RequestMove ();
		legalMoves = moveGenerator.GetMoves (false, false);
	}

}
