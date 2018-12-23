using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Engine communication protocol:
 * Initial position set from fen string
 * Move format: (start square, end square) + (promotion piece type)
 * 
 * Examples:
 * e2e4 (no extra notation denoting capture, check, etc)
 * e1g1 (castling)
 * e7e8Q (pawn promotion)
 * e5f6 (pawn capture - en passant remains the same)
 * 
 */

public class ChessUIOldGround : MonoBehaviour {               // CHANGED IN ORIGINAL

	public bool hide;

	public int themeIndex;
	public Theme[] themes;

	[Range(0,1)]
	public float frameSize;
	
	public Transform lightSquare;
	public Transform darkSquare;
	public Transform frame;

	public Sprite blackRook, blackKnight, blackBishop, blackQueen, blackKing, blackPawn;
	public Sprite whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing, whitePawn;
	public Sprite none;

	public bool boardOrientationWhite = true;
	bool highlightLegalMoves = true;


	// board (0,0) = a1; (7,7) = h8
	char[,] board;
	Dictionary<char,Sprite> spriteDictionary;
	SpriteRenderer[,] pieceSquares;
	Renderer[,] squares;
	GameObject boardVisibilityController;
	GameObject pieceVisibilityController;

	static ChessUIOldGround myInstance;                       // CHANGED IN OLD

	void Awake() {
		CreateBoardUI ();
		if (hide) {
			SetBoardVisibility(false);
		}
	}

	/// <summary>
	/// Gets the instance.
	/// </summary>
	public static ChessUIOldGround instance                   // CHANGED IN OLD
    {       
        get {
			if (myInstance == null) {
				myInstance = FindObjectOfType<ChessUIOldGround> ();       // CHANGED IN OLD
            }
			return myInstance;
		}
	}

	public void SetHightlightLegalMoves(bool highlight) {
		highlightLegalMoves = highlight;
	}

	public void SetBoardVisibility(bool visible) {
		boardVisibilityController.SetActive (visible);
	}

	public void SetPieceVisiblity(bool visible) {
		for (int i = 0; i < 8; i ++) {
			for (int j = 0; j < 8; j ++) {
				pieceSquares [i, j].color = (visible) ? Color.white : Color.clear;
			}
		}

	}

	public void FlipBoard() {
		boardOrientationWhite = !boardOrientationWhite;
		SetBoardOrientation (boardOrientationWhite);
	}

	public void SetBoardOrientation(bool white) {
		boardOrientationWhite = white;
		CreateBoardUI ();
		AutoUpdate ();
	}

	/// <summary>
	/// Highlight the given square (algebraic coordinate)
	/// </summary>
	public void HighlightSquare(string squaresToHighlight) {
		if (highlightLegalMoves) {
			int squareX = GetIndex (DefinitionsGround.fileNames.IndexOf (squaresToHighlight [0]));
			int squareY = GetIndex (DefinitionsGround.rankNames.IndexOf (squaresToHighlight [1]));

			squares [squareX, squareY].material.color = themes [themeIndex].legalMoveHighlight;
		}
	}

	public void HighlightMove(int fromIndex, int toIndex) {
		int fromX = GetIndex(BoardGround.FileFrom128 (fromIndex) - 1);
		int fromY = GetIndex(BoardGround.RankFrom128 (fromIndex) - 1);
		int toX = GetIndex(BoardGround.FileFrom128 (toIndex) - 1);
		int toY = GetIndex(BoardGround.RankFrom128 (toIndex) - 1);
		// highlight move squares;
		squares [fromX, fromY].material.color = themes[themeIndex].moveFromHighlightColour;
		squares [toX, toY].material.color = themes[themeIndex].moveToHighlightColour;
	}

	public void ResetHighlights() {
		PaintBoard (themes[themeIndex].lightColour, themes[themeIndex].darkColour);
	}
	
	/// Updates the board UI to match the board array
	public void AutoUpdate() {
		ResetHighlights();
		for (int i = 0; i < 8; i ++) {
			for (int j = 0; j < 8; j ++) {
				int x = GetIndex(j);
				int y = GetIndex(i);
	
				int index = BoardGround.Convert64to128(i*8 + j);
				char pieceCode = BoardGround.pieceNameDictionary[BoardGround.boardArray[index]];
				board[x,y] = pieceCode;
				pieceSquares[x,y].sprite = spriteDictionary[pieceCode];
			}
		}
	}

	public void CreateBoardUI () {
		board = new char[8,8];
		// Create new holder objects for easy organisation/deletion of board UI elements
		string holderName = "UI Holder";

		if (transform.Find (holderName)) {
			GameObject holderOld = transform.Find(holderName).gameObject;
			DestroyImmediate(holderOld);
		}

		Transform uiHolder = new GameObject (holderName).transform;
		boardVisibilityController = uiHolder.gameObject;
		uiHolder.parent = transform;

		Transform boardHolder = new GameObject ("BoardGround Holder").transform;
		boardHolder.parent = uiHolder;

		Transform pieceHolder = new GameObject ("Piece Holder").transform;
		pieceVisibilityController = pieceHolder.gameObject;
		pieceHolder.parent = uiHolder;


		// Generate board and piece squares
		squares = new Renderer[8,8];
		pieceSquares = new SpriteRenderer[8,8];


		for (int i = 0; i < 8; i ++) {
			for (int j = 0; j < 8; j ++) {
				int x = GetIndex(j);
				int y = GetIndex(i);
				bool isLightSquare = SquareIsWhite(x,y);

				Vector2 position = new Vector2(-4.5f + x+1, -4.5f + y+1);
				string algebraicCoordinate = DefinitionsGround.fileNames[j].ToString() + DefinitionsGround.rankNames[i].ToString();


				// squares
				Transform newSquare = Instantiate((isLightSquare)?lightSquare:darkSquare, position, Quaternion.identity) as Transform;
				newSquare.parent = boardHolder;
				newSquare.name =  algebraicCoordinate;
				squares[x,y] = newSquare.GetComponent<Renderer>();

				// pieces
				PieceUIGround pieceUI = new GameObject(algebraicCoordinate).AddComponent<PieceUIGround>();
				pieceUI.Init(algebraicCoordinate, position);
				SpriteRenderer sprite = pieceUI.gameObject.AddComponent<SpriteRenderer>();
				sprite.transform.position = position;
				sprite.transform.parent = pieceHolder;
				pieceSquares[x,y] = sprite;
			}
		}

		// Create frame
		Transform newFrame = Instantiate(frame, Vector3.forward * 0.1f, Quaternion.identity) as Transform;
		newFrame.localScale = Vector3.one * (8 + frameSize);
		newFrame.parent = uiHolder;
		newFrame.GetComponent<Renderer> ().sharedMaterial.color = themes[themeIndex].frameColour;

		PaintBoard (themes[themeIndex].lightColour, themes[themeIndex].darkColour);
		InitializeSpriteDictionary ();
	}

	int GetIndex(int i) {
		if (!boardOrientationWhite) {
			return 7-i;
		}
		return i;
	}

	bool SquareIsWhite(int x, int y) {
		int a = (boardOrientationWhite) ? 1 : 0;
		return ((y + x) % 2) != a;
	}

	void PaintBoard(Color light, Color dark) {
		if (squares != null) {
			for (int i = 0; i < 8; i ++) {
				for (int j = 0; j < 8; j ++) {
					int x = GetIndex(j);
					int y = GetIndex(i);
					bool isLightSquare = SquareIsWhite(i,x);
					squares[x,y].sharedMaterial.color = (isLightSquare)?light:dark;
				}
			}
		}
	}

	void InitializeSpriteDictionary() {
		spriteDictionary = new Dictionary<char, Sprite> ();
		spriteDictionary.Add (' ', none);

		spriteDictionary.Add ('r', blackRook);
		spriteDictionary.Add ('n', blackKnight);
		spriteDictionary.Add ('b', blackBishop);
		spriteDictionary.Add ('q', blackQueen);
		spriteDictionary.Add ('k', blackKing);
		spriteDictionary.Add ('p', blackPawn);

		spriteDictionary.Add ('R', whiteRook);
		spriteDictionary.Add ('N', whiteKnight);
		spriteDictionary.Add ('B', whiteBishop);
		spriteDictionary.Add ('Q', whiteQueen);
		spriteDictionary.Add ('K', whiteKing);
		spriteDictionary.Add ('P', whitePawn);
	}

	[System.Serializable]
	public class Theme {
		public string themeName;
		public Color lightColour;
		public Color darkColour;
		public Color moveFromHighlightColour;
		public Color moveToHighlightColour;
		public Color legalMoveHighlight;
		public Color frameColour;
	}

}
