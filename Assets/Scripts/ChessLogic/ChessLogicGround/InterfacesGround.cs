using System;

public interface IMoveGeneratorGround {
//	MoveOld[] GetAllLegalMoves(Position position);

	void PrintTimes();
}

public interface ISearchGround {
//	event Action<MoveOld> OnNewMoveFound;
	//event Action TestEvent;
	//void StartSearch(Position position);
	void Update();
}