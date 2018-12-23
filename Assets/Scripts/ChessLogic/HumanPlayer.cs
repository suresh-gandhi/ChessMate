using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HumanPlayer : Player {

	public static Stack<ushort> movesMade = new Stack<ushort>();
	public static Heap legalMoves;

	public override void Init (bool white) {
		base.Init (white);
	}

	/// <summary>
	/// Make a move that is known to be legal
	/// </summary>
	protected override void MakeMove (ushort move)
	{
		//UnityEngine.Debug.Log (move);
		//UnityEngine.Debug.Log (PGNReader.NotationFromMove (move));
		base.MakeMove (move);
		movesMade.Push (move);
	}



	/// <summary>
	/// Makes the move after confirming that it is legal
	/// </summary>
	public void TryMakeMove(string algebraicMove) {
		if (isWhite == Board.IsWhiteToPlay()) {
			legalMoves = moveGenerator.GetMoves(false,false);
			for (int i = 0; i < legalMoves.Count; i ++) {
				int moveFromIndex = legalMoves.GetMove(i) & 127;
				int moveToIndex = (legalMoves.GetMove(i) >> 7) & 127;
				
				int moveFromX = Board.Convert128to64(moveFromIndex) % 8;
				int moveFromY = Board.Convert128to64(moveFromIndex) / 8;
				int moveToX = Board.Convert128to64(moveToIndex) % 8;
				int moveToY = Board.Convert128to64(moveToIndex) / 8;
				
				
				string fromAlgebraic = Definitions.fileNames[moveFromX].ToString() + Definitions.rankNames[moveFromY].ToString();
				string toAlgebraic = Definitions.fileNames[moveToX].ToString() + Definitions.rankNames[moveToY].ToString();


				string moveCoords = fromAlgebraic + toAlgebraic;
				if (moveCoords == algebraicMove) { // move confirmed as legal
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
