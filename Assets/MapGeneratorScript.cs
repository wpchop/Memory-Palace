using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Palace;
using System;

public class MapGeneratorScript : MonoBehaviour {

	public int numPhotos;

	private int gridSize;

	public string seed;
	public bool useRandom;

	[Range(0,100)]
	public int randomFillPercent;

	public Transform wall;

	public Transform hallCorner;

	public Transform hallStraight;

	public Transform hallCross;

	public Transform hallT;

	public Transform hall;

	public Transform photo;

	List<Room> rooms;
	int[,] map;


	private double radius = 4;

	void Start() {
		GenerateRooms ();
	}

	void GenerateRooms() {
		gridSize = numPhotos * numPhotos;
		rooms = new List<Room> ();
		map = new int[gridSize, gridSize];
		RandomFillMap ();
		drawRooms ();
	}

	void RandomFillMap() {
		if (useRandom) {
			seed = Time.time.ToString();
		}

		System.Random pseudo = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < numPhotos; x++) {
			for (int y = 0; y < numPhotos; y++) {
				map[x,y] = (pseudo.Next(0,100) < randomFillPercent)?1: 0;
				if (pseudo.Next (0, 100) < randomFillPercent) {
					Room newRoom = new Room (x, y, numPhotos, numPhotos);
					rooms.Add (newRoom);
				}
			}
		}
	}

	void drawRoom(Room room) {
		float x = -gridSize / 2 + room.centerX * 10 + 0.5f;
		float y = -gridSize / 2 + room.centerY * 10 + 0.5f;
		int w = room.width;
		int h = room.height;

		// for straight hallway ||
		// Vector3 gridPos = new Vector3 (x - 2f, 0, y - 0.5f);
		Vector3 gridPos = new Vector3(x - 0.5f, 0, y -0.5f);
		//Instantiate (hall, gridPos, Quaternion.identity);

		Vector3 pos = new Vector3 (x, 0, y);
		// for horizontal straight hallway =
		//gridPos = new Vector3 (x - 0.5f, 0, y + 1f);
		// Instantiate (hall, gridPos, Quaternion.AngleAxis (180, new Vector3 (0, 1, 0)));

		// If no top opening
		if (room.walls [(int)Room.Wall.top]) {
			// Top and bottom walls
			for (int i = -w / 2; i < w / 2; i++) {
				pos = new Vector3 (x + i, 0, y - h / 2);
				Instantiate (wall, pos, Quaternion.identity);
			}
		} else {
			for (int i = -w / 2; i < w / 2; i++) {
				if (i != -1 && i != 0) {
					pos = new Vector3 (x + i, 0, y - h / 2);
					Instantiate (wall, pos, Quaternion.identity);
				}
			}
		}

		// bottom
		if (room.walls [(int)Room.Wall.bottom]) {
			// Top and bottom walls
			for (int i = -w / 2; i < w / 2; i++) {
				pos = new Vector3 (x + i, 0, y + h / 2 - 1);
				Instantiate (wall, pos, Quaternion.identity);

			} 
		} else {
			for (int i = -w / 2; i < w / 2; i++) {
				if (i != 0 && i != -1) {
					pos = new Vector3 (x + i, 0, y + h / 2 - 1);
					Instantiate (wall, pos, Quaternion.identity);
				}
			}
		}

		//left
		if (room.walls [(int)Room.Wall.left]) {
			for (int j = -h / 2; j < h / 2; j++) {
				pos = new Vector3 (x - w / 2, 0, y + j);
				Instantiate (wall, pos, Quaternion.identity);
			}
		} else {
			for (int j = -h / 2; j < h / 2; j++) {
				if (j != 0 && j != -1) {
					pos = new Vector3 (x - w / 2, 0, y + j);
					Instantiate (wall, pos, Quaternion.identity);
				}
			}
		}

		//right
		if (room.walls [(int)Room.Wall.right]) {
			for (int j = -h / 2; j < h / 2; j++) {
				pos = new Vector3 (x + w / 2 - 1, 0, y + j);
				Instantiate (wall, pos, Quaternion.identity);
			}
		} else {
			for (int j = -h / 2; j < h / 2; j++) {
				if (j != 0 && j != -1) {
					pos = new Vector3 (x + w / 2 - 1, 0, y + j);
					Instantiate (wall, pos, Quaternion.identity);
				}
			}
		}
			
		// Left and right walls
//		for (int j = -h / 2; j < h / 2; j++) {
//			Vector3 pos = new Vector3 (x + w/2 - 1, 0, y + j);
//			Instantiate (wall, pos, Quaternion.identity);
//			pos = new Vector3 (x - w / 2, 0, y + j);
//			Instantiate (wall, pos, Quaternion.identity);
//		}

		// Add photos
		Vector3 position = new Vector3 (x, 2, y);
		Instantiate (photo, position, Quaternion.identity);

	}

	void drawRooms() {
	  if (rooms != null) {
			foreach (Room room in rooms) {
				drawRoom (room);
				foreach (Room room2 in rooms) {
					if (room2 != room) {
						if (room.getDistanceFrom (room2) < radius) {
							//DrawHallway (room, room2);
						}
					}
				}
			}


		}
	}

	void makeNeighbors(Room r1, Room r2) {
		
	}

	void DrawHallway(Room room1, Room room2) {
		int x1 = room1.centerX;
		int x2 = room2.centerX;
		int y1 = room1.centerY;
		int y2 = room2.centerY;

		// Same horizontal axis
		if (x1 == x2) {
			float x = -gridSize / 2 + x1 * 10 + 0.5f;
			float miny = -gridSize / 2 + Mathf.Min (y1, y2) * 10 + 0.5f;
			float maxy = -gridSize / 2 + Mathf.Max (y1, y2) * 10 + 0.5f;
			for (int i = (int)miny; i < (int)maxy; i++) {
				Vector3 pos = new Vector3 (x, 0, i);
				pos = new Vector3 (x - 1, 0, i);
			}
		} else if (y1 == y2) {
			float y = -gridSize / 2 + y1 * 10 + 0.5f;
			float minx = -gridSize / 2 + Mathf.Min (x1, x2) * 10 + 0.5f;
			float maxx = -gridSize / 2 + Mathf.Max (x1, x2) * 10 + 0.5f;
			for (int i = (int)minx; i < (int)maxx; i++) {
				Vector3 pos = new Vector3 (i, 0, y);
				pos = new Vector3 (i, 0, y-1);
			}
		} else {
			//only draw if room1 is to the left of room2
			if (x1 < x2) {
				float startx = -gridSize / 2 + x1 * 10 + 0.5f;
				float endx = -gridSize / 2 + x2 * 10 + 0.5f;
				float starty = -gridSize / 2 + y1 * 10 + 0.5f;
				float endy = -gridSize / 2 + y2 * 10 + 0.5f;
				for (int i = (int) Mathf.Floor(startx + room1.width/2); i <= (int)endx; i++) {
					Vector3 pos = new Vector3 (i, 0, starty + 1);
					Instantiate(hall, pos, Quaternion.identity);
					pos = new Vector3 (i, 0, starty - 2);
					Instantiate(hall, pos, Quaternion.identity);
				}
				if (starty > endy) {
					float temp = endy;
					endy = starty;
					starty = temp;
				}
				for (int i = (int)starty - 1; i < (int)endy; i++) {
					Vector3 pos = new Vector3 (endx + 1, 0, i);
					Instantiate(hall, pos, Quaternion.identity);
					pos = new Vector3 (endx - 2, 0, i);
					Instantiate(hall, pos, Quaternion.identity);
				}
			}
		}
	}

}
