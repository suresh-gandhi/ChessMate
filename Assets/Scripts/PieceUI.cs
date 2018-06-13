using UnityEngine;
using System.Collections;

/*
 * The PieceUI script is attached to all squares of the board
 * Calling the Move method will position the piece graphic of this square at the specified position
 * Calling the Release method will reset the piece graphic back to its original position
 * In order to actually make a move on the board, the ChessUI class must be used.
 * 
 * The objects to which the PieceUI scripts are attached all have the 'Piece' layer assigned.
 * The 'Piece' layer will be disabled while being moves so as not to interfere with input.
 */

public class PieceUI : MonoBehaviour {

	public string algebraicCoordinate { get; private set; }
	Vector3 defaultLocalPosition;

	public void Init(string _algebraicCoordinate, Vector3 position) {
		BoxCollider bCollider = gameObject.AddComponent<BoxCollider> ();

        bCollider.size = new Vector3(3, 0.1f, 3);

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;

		algebraicCoordinate = _algebraicCoordinate;
		defaultLocalPosition = position;

		gameObject.layer = LayerMask.NameToLayer ("Piece");
	}

	// public void Move(Vector3 newPosition) {
        // Vector3 newLocalPosition = newPosition - transform.parent.transform.parent.transform.parent.transform.parent.position;
		// gameObject.layer = 0;
		// transform.localPosition = new Vector3(newLocalPosition.x, defaultLocalPosition.y + 0.5f, newLocalPosition.z);
	// }

	public void Release() {
		gameObject.layer = LayerMask.NameToLayer ("Piece");
		transform.localPosition = defaultLocalPosition;
	}
	
}
