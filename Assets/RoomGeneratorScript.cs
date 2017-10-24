using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneratorScript : MonoBehaviour {

	public int numPhotos;

	public string seed;
	public bool useRandom;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;

	void Start() {
		GenerateRooms ();
	}

	void GenerateRooms() {
		map = new int[numPhotos, numPhotos];
		RandomFillMap ();

	}

	void RandomFillMap() {
		if (useRandom) {
			seed = Time.time.ToString();
		}

		System.Random pseudo = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < numPhotos; x++) {
			for (int y = 0; y < numPhotos; y++) {
				map[x,y] = (pseudo.Next(0,100) < randomFillPercent)?1: 0;
			}
		}
	}

	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < numPhotos; x++) {
				for (int y = 0; y < numPhotos; y++) {
					Gizmos.color = (map [x, y] == 1) ? Color.black : Color.white;
					Vector3 pos = new Vector3 (-numPhotos / 2 + x + 0.5f, 0, -numPhotos / 2 + y + 0.5f);
					Gizmos.DrawCube (pos, Vector3.one);
				}
			}
		}
	}
}
