﻿using System.Collections;
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
	public Transform photo;

	List<Room> rooms;
	Hall[,] map;

	private double radius;

	private enum Hall {e, r, h, v, c_tr, c_br, t_r, t_l, t_d, t_u, x};

	void Start() {
		GenerateRooms ();
	}

	void GenerateRooms() {
		gridSize = 10 * numPhotos;
		radius = 1.0/numPhotos * 35.0;
		rooms = new List<Room> ();
		map = new Hall[numPhotos, numPhotos];
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
				// map[x,y] = (pseudo.Next(0,100) < randomFillPercent)?1: 0;
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
	 * Function that draws the different types of hallway components
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
	 * that dictionaries are slower than switch statements for handling enum cases.
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

		// Add photos
		Vector3 position = new Vector3 (x - 3f, 2f, y + 3.5f);
		Instantiate (photo, position, Quaternion.AngleAxis (180, new Vector3 (0, 1, 0)));
	}

}
