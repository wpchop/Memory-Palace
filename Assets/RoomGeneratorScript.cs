using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneratorScript : MonoBehaviour {

	public int numPhotos;

	private int gridSize;

	public string seed;
	public bool useRandom;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;

	void Start() {
		GenerateRooms ();
	}

	void GenerateRooms() {
		gridSize = numPhotos * numPhotos;
		map = new int[gridSize, gridSize];
		RandomFillMap ();
	}

	void RandomFillMap() {
		if (useRandom) {
			seed = Time.time.ToString();
		}

		System.Random pseudo = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < gridSize; x++) {
			for (int y = 0; y < gridSize; y++) {
				map[x,y] = (pseudo.Next(0,100) < randomFillPercent)?1: 0;
			}
		}
	}

	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < gridSize; x++) {
				for (int y = 0; y < gridSize; y++) {
					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
					Vector3 pos = new Vector3 (-gridSize / 2 + x + 0.5f, 0, -gridSize / 2 + y + 0.5f);
					Gizmos.DrawCube (pos, Vector3.one);
				}
			}
		}
	}
}
