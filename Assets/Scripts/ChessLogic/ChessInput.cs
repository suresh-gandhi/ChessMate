using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChessInput : MonoBehaviour {

    public LayerMask pieceMask;
    Camera viewCamera;

    PieceUI pieceHeld;
    bool holdingPiece;
    bool isThereAPiece;

    List<HumanPlayer> players = new List<HumanPlayer>();
    bool active;

    void Start()
    {
        viewCamera = Camera.main;
        FindObjectOfType<MoveManager>().OnGameOver += HandleOnGameOver;
    }
    
    void HandleOnGameOver(int result, Definitions.ResultType type)
    {
        active = false;
    }

    public void AddPlayer(HumanPlayer player)
    {
        players.Add(player);
        active = true;
    }

    void Update()
    {
        if (!active)
        { // input is active if one or more players have been assigned to it (or game is over)
            return;
        }

        Vector3 endPointerPosition = GvrPointerInputModule.CurrentRaycastResult.worldPosition;
        // Vector2 mousePosition = (Vector2)viewCamera.ScreenToWorldPoint (Input.mousePosition);

        // Pick up piece
        // If not holding any Piece and clicked anywhere
        if( !holdingPiece && Input.GetMouseButtonDown(0) )
        // if (/*Input.GetKeyDown(KeyCode.Mouse0)*/ Input.GetMouseButtonDown(0) && !holdingPiece)
        {
            ChessUI.instance.ResetHighlights();
            // isPlayerMove just checks if any HumanPlayer has his chance
            // If so then we will highlight the regions
            bool isPlayerMove = false;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].isMyMove)
                {
                    isPlayerMove = true;
                    break;
                }
            }

            holdingPiece = TryGetPieceUIAtPoint(endPointerPosition, out pieceHeld);
            // This line improves efficiency
            if (!isThereAPiece)
                holdingPiece = false;
            // Debug.Log("holding piece: " + holdingPiece + " isPlayerMove: " + isP);
            if (holdingPiece && isPlayerMove)
            {
                // highlight legal moves for held piece
                Heap legalMoveHeap = HumanPlayer.legalMoves;
                for (int i = 0; i < legalMoveHeap.Count; i++)
                {
                    HighlightSquare(legalMoveHeap.GetMove(i), pieceHeld.algebraicCoordinate);
                }
            }
        }
        // Let go of piece
        // If holding anything and clicked somewhere
        else if(Input.GetMouseButtonDown(0) && holdingPiece)
        // else if (/*Input.GetKeyUp(KeyCode.Mouse0)*/Input.GetMouseButtonUp(0) && holdingPiece)
        {
            PieceUI dropSquare;
            ChessUI.instance.ResetHighlights();
            if (TryGetPieceUIAtPoint(endPointerPosition, out dropSquare))
            {
                string algebraicMove = pieceHeld.algebraicCoordinate + dropSquare.algebraicCoordinate;
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].TryMakeMove(algebraicMove);
                }
            }
            pieceHeld.Release();
            holdingPiece = false;
        }
        // Drag piece
        // else if (/*Input.GetKey(KeyCode.Mouse0)*/ Input.GetMouseButton(0) && holdingPiece)
        // {
        //    pieceHeld.Move(endPointerPosition);
        // }
    }

    void HighlightSquare(ushort move, string pieceAlgebraic)
    {
        int moveFromIndex = move & 127;
        int moveToIndex = (move >> 7) & 127;

        int moveFromX = Board.Convert128to64(moveFromIndex) % 8;
        int moveFromY = Board.Convert128to64(moveFromIndex) / 8;
        int moveToX = Board.Convert128to64(moveToIndex) % 8;
        int moveToY = Board.Convert128to64(moveToIndex) / 8;


        string fromAlgebraic = Definitions.fileNames[moveFromX].ToString() + Definitions.rankNames[moveFromY].ToString();
        string toAlgebraic = Definitions.fileNames[moveToX].ToString() + Definitions.rankNames[moveToY].ToString();

        if (fromAlgebraic == pieceHeld.algebraicCoordinate)
        {
            ChessUI.instance.HighlightSquare(toAlgebraic);
        }
    }

    // What is it's output when we click on a blank poimt on the board
    // Who sets HumanPlayer.legalMoves
    bool TryGetPieceUIAtPoint(Vector3 point, out PieceUI piece)
    {
        // RaycastHit hit;
        // Ray ray = viewCamera.ScreenPointToRay(point);
        // bool isColliding = Physics.Raycast(ray, out hit , pieceMask);
        // Collider2 pieceCollider = Physics2D.OverlapPoint(point, pieceMask);
        bool isColliding = GvrPointerInputModule.CurrentRaycastResult.isValid;
        if (isColliding)
        {
            if (GvrPointerInputModule.CurrentRaycastResult.gameObject.GetComponent<PieceUI>() != null)
            {
                piece = GvrPointerInputModule.CurrentRaycastResult.gameObject.GetComponent<PieceUI>();
                if (piece.GetComponent<MeshCollider>().enabled)
                    isThereAPiece = true;
                else
                    isThereAPiece = false;
                return true;
            }
        }
        piece = null;
        return false;
    }
}
