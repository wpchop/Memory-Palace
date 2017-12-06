using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Palace;
using System;

/**
 * Script for generating a random memory palace based on input parameters
 * and filepath to folder containing photos.
 * */
public class MapGeneratorScript : MonoBehaviour {

	public int numPhotos;

	private int gridSize;

	public string seed;
	public bool useRandom;

	public string photoFilePath;

	[Range(0,100)]
	public int randomFillPercent;

	public Transform wall;
	public Transform hallCorner;
	public Transform hallStraight;
	public Transform hallCross;
	public Transform hallT;
	public GameObject photo;

	List<Room> rooms;
	Hall[,] map;
	// List<Memory> memories;

	Memory [] memories;

	private double radius;

	private enum Hall {e, r, h, v, c_tr, c_br, t_r, t_l, t_d, t_u, x};

	void Start() {
		createMemories ();
		GenerateRooms ();
	}

	/**
	 * Loads photos from {photoFilepath} folder into textures and 
	 * adds them to a collection of memories.
	 * */
	void createMemories() {
		PhotoCreator photoCreator = new PhotoCreator (photoFilePath);
		List<string> photoFiles = photoCreator.getPhotoFiles ();
		int numMemories = photoFiles.Count;
		memories = new Memory[numMemories];

		for (int i = 0; i < numMemories; i++) {
			Texture2D pic = photoCreator.getTexture (photoFiles[i]);
			Memory memory = new Memory (pic);
			memories [i] = memory;
		}
		Debug.Log ("Number of photos: " + numMemories);
	}

	/**
	 * Setting up parameters and grid for palace construction.
	 * */
	void GenerateRooms() {
		gridSize = 10 * numPhotos;
		radius = 1.0/numPhotos * 35.0;
		rooms = new List<Room> ();
		map = new Hall[numPhotos, numPhotos];
		RandomFillMap ();
		PlaceMemories ();
		drawRooms ();
	}

	void PlaceMemories() {
		int memcount = 0;
		int totalMem = memories.Length;
		foreach (Room r in rooms) {
			for (int i = 0; i < 4; i++ ) {
				r.addMemory (i, memories [memcount % totalMem]);
				memcount++;
			}
		}
	}

	/**
	 * Randomly places room onto grid of the palace
	 * */
	void RandomFillMap() {
		if (useRandom) {
			seed = Time.time.ToString();
		}

		System.Random pseudo = new System.Random (seed.GetHashCode ());

		for (int x = 0; x < numPhotos; x++) {
			for (int y = 0; y < numPhotos; y++) {
				if (pseudo.Next (0, 100) < randomFillPercent) {
					map [x, y] = Hall.r;
					Room newRoom = new Room (x, y, 10, 10);
					rooms.Add (newRoom);
				} else {
					map [x, y] = Hall.e;
				}
			}
		}
	}

	/**
	 * Starting point for drawing the palace
	 * */
	void drawRooms() {
	  if (rooms != null) {
			foreach (Room room in rooms) {
				foreach (Room room2 in rooms) {
					if (room2 != room) {
						if (room.getDistanceFrom (room2) < radius) {
							makeNeighbors(room, room2);
						}
					}
				}
			}
			foreach (Room room in rooms) {
				drawRoom (room);
			}
		}

		for (int x = 0; x < numPhotos; x++) {
			for (int y = 0; y < numPhotos; y++) {
					float xPos = -gridSize/2 + 10 * x;
					float yPos = -gridSize/2 + 10 * y;
					Vector3 pos = new Vector3(xPos, 0, yPos);
					drawHallUnit(map[x,y], pos);
			}
		}
	}

	/**
	 * Function that draws the different types of hallway components given a 
	 * type of Hall and position. 
	 **/
	void drawHallUnit(Hall type, Vector3 pos) {
		switch (type) {
		case Hall.e:
			return;
		case Hall.r:
			return;
		case Hall.h:
			Instantiate(hallStraight, pos, Quaternion.AngleAxis(90, new Vector3(0,1,0)));
			return;
		case Hall.v:
			Instantiate(hallStraight, pos, Quaternion.identity);
			return;
		case Hall.c_br:
			Instantiate (hallCorner, pos, Quaternion.AngleAxis(180, new Vector3(0,1,0)));
			return;
		case Hall.c_tr:
			Instantiate (hallCorner, pos, Quaternion.AngleAxis(90, new Vector3(0,1,0)));
			return;
		case Hall.t_d:
			Instantiate (hallT, pos, Quaternion.AngleAxis(-90, new Vector3(0,1,0)));
			return;
		case Hall.t_l:
			Instantiate (hallT, pos, Quaternion.identity);
			return;
		case Hall.t_r:
			Instantiate (hallT, pos, Quaternion.AngleAxis(180, new Vector3(0,1,0)));
			return;
		case Hall.t_u:
			Instantiate (hallT, pos, Quaternion.AngleAxis(90, new Vector3(0,1,0)));
			return;
		case Hall.x:
			Instantiate (hallCross, pos, Quaternion.identity);
			return;
		}
	}
		
	/**
	 * Giant convoluted function that handles logic for adding hallway
	 * components. Based on the previous and new hallway component, a new
	 * component will be needed. For example Hall.h + Hall.v = Hall.x
	 * Read here
	 * https://stackoverflow.com/questions/17605603/enum-case-handling-better-to-use-a-switch-or-a-dictionary
	 * that dictionaries are slower than switch statements for handling enum cases in C#
	 * Hence, the really long nested switch statements. Apologies for readability.
	 * */
	void modifyHallUnit(int x, int y, Hall add) {
		Hall prev = map [x, y];
		if (add == prev) {
			return;
		}
		switch (prev) {
		case Hall.e:
			map [x, y] = add;
			return;
		case Hall.r:
			return;
		case Hall.x:
			return;
		case Hall.c_tr:
			switch (add) {
			case Hall.c_tr:
				return;
			case Hall.c_br:
				map [x, y] = Hall.t_r;
				return;
			case Hall.t_d:
				map [x, y] = Hall.x;
				return;
			case Hall.t_l:
				map [x, y] = Hall.x;
				return;
			case Hall.t_r:
				map [x, y] = Hall.t_r;
				return;
			case Hall.t_u:
				map [x, y] = Hall.t_u;
				return;
			case Hall.h:
				map [x, y] = Hall.t_u;
				return;
			case Hall.v:
				map [x, y] = Hall.t_r;
				return;
			}
			return;
		case Hall.c_br:
			switch (add) {
			case Hall.c_tr:
				map [x, y] = Hall.t_r;
				return;
			case Hall.c_br:
				return;
			case Hall.t_d:
				map [x, y] = Hall.t_d;
				return;
			case Hall.t_l:
				map [x, y] = Hall.x;
				return;
			case Hall.t_r:
				map [x, y] = Hall.t_r;
				return;
			case Hall.t_u:
				map [x, y] = Hall.x;
				return;
			case Hall.h:
				map [x, y] = Hall.t_u;
				return;
			case Hall.v:
				map [x, y] = Hall.t_r;
				return;
			}
			return;
		case Hall.t_d:
			switch (add) {
			case Hall.c_br:
				return;
			case Hall.h:
				return;
			}
			map [x, y] = Hall.x;
			return;
		case Hall.t_l:
			if (add == Hall.t_l || add == Hall.v) {
				return;
			}
			map [x, y] = Hall.x;
			return;
		case Hall.t_r:
			if (add == Hall.c_tr || add == Hall.c_br || add == Hall.v) {
				return;
			}
			map [x, y] = Hall.x;
			return;
		case Hall.t_u:
			if (add == Hall.c_tr || add == Hall.h) {
				return;
			}
			map [x, y] = Hall.x;
			return;
		case Hall.h:
			switch (add) {
			case Hall.c_tr:
				map [x, y] = Hall.t_u;
				return;
			case Hall.c_br:
				map [x, y] = Hall.t_d;
				return;
			case Hall.t_d:
				return;
			case Hall.t_l:
				map [x, y] = Hall.x;
				return;
			case Hall.t_r:
				map [x, y] = Hall.x;
				return;
			case Hall.t_u:
				return;
			case Hall.h:
				return;
			case Hall.v:
				map [x, y] = Hall.x;
				return;
			}
			return;
		case Hall.v:
			switch (add) {
			case Hall.c_tr:
				map [x, y] = Hall.t_r;
				return;
			case Hall.c_br:
				map [x, y] = Hall.t_r;
				return;
			case Hall.t_d:
				map [x, y] = Hall.x;
				return;
			case Hall.t_l:
				return;
			case Hall.t_r:
				return;
			case Hall.t_u:
				map [x, y] = Hall.x;
				return;
			case Hall.h:
				map [x, y] = Hall.x;
				return;
			case Hall.v:
				return;
			}
			return;
		}
	}

	/**
	 * Sets up the hallway components between two rooms.
	 * */
	void makeNeighbors(Room room1, Room room2) {
		int x1 = (int)room1.centerX;
		int x2 = (int)room2.centerX;
		int y1 = (int)room1.centerY;
		int y2 = (int)room2.centerY;

		int xmin = Math.Min (x1, x2);
		int xmax = Math.Max (x1, x2);

		int ymin = Math.Min (y1, y2);
		int ymax = Math.Max (y1, y2);

		if (x1 == x2) {
			for (int j = ymin; j < ymax; j++) {
				modifyHallUnit (x1, j, Hall.v);
			}
			return;
		}

		if (y1 == y2) {
			for (int i = xmin; i < xmax; i++) {
				modifyHallUnit (i, y1, Hall.h);
			}
			return;
		}
			
		if (x1 < x2) {
			for (int i = x1; i < x2 - 1; i++) {
				modifyHallUnit (i, y1, Hall.h);
			}
			if (y1 > y2) {
				modifyHallUnit (x2, y1, Hall.c_tr);
				for (int i = ymin + 1; i < ymax; i++) {
					modifyHallUnit (x2, i, Hall.v);
				}
			} else {
				modifyHallUnit (x2, y1, Hall.c_br);
				for (int i = ymin; i < ymax - 1; i++) {
					modifyHallUnit (x2, i, Hall.v);
				}
			}

		}
	}

	/**
	 * Draws a single room and the photos placed within that room
	 * */
	void drawRoom(Room room) {
		float x = -gridSize / 2 + room.centerX * 10 + 0.5f;
		float y = -gridSize / 2 + room.centerY * 10 + 0.5f;
		int w = room.width;
		int h = room.height;

		Vector3 pos = new Vector3 (x, 0, y);

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
			
		// Adding photos to each room
		foreach (KeyValuePair<int, Memory> entry in room.getMemories()) {
			Vector3 photoPos = getPhotoPosition (x, y, entry.Key);

			GameObject frame = 
				(GameObject)Instantiate (photo, photoPos, Quaternion.AngleAxis (180, new Vector3 (0, 1, 0)));
			MeshRenderer mRenderer = (MeshRenderer) frame.GetComponents (typeof(MeshRenderer)) [0];
			// 1 is the picture, 0 is the frame;
			Material photoMaterial = mRenderer.materials [1];
			photoMaterial.mainTexture = entry.Value.getTexture ();
		}
			
		// Example of how to change the position/rotation of the picture:
		// pictureFrame.transform.rotation = Quaternion.AngleAxis (180, new Vector3 (0, 1, 0));

	}

	// Getting the position of each photo
	private Vector3 getPhotoPosition(float x, float y, int place) {
		switch (place) {
		case 1: 
			return new Vector3 (x - 3f, 2f, y + 3.5f);;
		case 2:
			return new Vector3 (x + 2f, 2f, y + 3.5f);
		case 3:
			return new Vector3 (x + 1f, 3f, y + 3.5f);
		case 4:
			return new Vector3 (x + 1f, 1f, y + 3.5f);
		}

		return new Vector3 (x + 1f, 0f, y + 3.5f);
	}

}
