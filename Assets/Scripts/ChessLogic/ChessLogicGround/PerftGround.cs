using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

public class PerftGround : MonoBehaviour {

	public bool usePerft;
	public bool runAllTests;
	public int searchDepth;
	public string fen;

	public PerftResults[] tests;

	int nodeSearchCount;
	MoveGeneratorGround moveGenerator;


	void Start() {
		if (usePerft) {
			moveGenerator = new MoveGeneratorGround();

			if (runAllTests) {
				bool allCorrect = true;
				for (int i =0; i < tests.Length; i ++) {
					searchDepth = tests[i].depth;
					fen = tests[i].fen;
					int correctResult = tests[i].correctResult;
					int result = RunTest();
					if (correctResult == result) {
						print ("Results match");
					}
					else {
						allCorrect = false;
						print ("Error at test " + i + " Result: " + result + "; expected: " + correctResult);
					}
				}
				if (allCorrect) {
					print ("Test suite passed");
				}
				else {
					print ("Test suite failed");
				}
			}
			else {
				RunTest();
			}
		
		}
	}

	int RunTest() {
		nodeSearchCount = 0;


		BoardGround.SetPositionFromFen(fen);
		TimerGround.Start("PerftGround");
		PerfTest(searchDepth);
		TimerGround.Stop("PerftGround");
		TimerGround.Print("PerftGround");
		print("Leaf nodes at depth " + searchDepth + ": " + nodeSearchCount);
		return nodeSearchCount;
	}

	void PerfTest(int depth) {
		if (depth == 0) {
			nodeSearchCount ++;
			return;
		}
	
		IList<ushort> moves = moveGenerator.GetMoves (false, false) as IList<ushort>;
		for (int i =0; i < moves.Count; i ++) {
			BoardGround.MakeMove(moves[i]);
			PerfTest(depth-1);
			BoardGround.UnmakeMove(moves[i]);
		}
	}

	[System.Serializable]
	public class PerftResults {
		public string name;
		public string fen;
		public int depth;
		public int correctResult;
		public bool myResultsMatch = true;
	}


}
