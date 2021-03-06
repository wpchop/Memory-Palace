﻿using System;
using System.Collections.Generic;
using UnityEngine;


namespace Palace {
	/**
	 * Represents a single room in the memory palace.
	 * */
	public class Room
	{
		public int centerX;
		public int centerY;
		public int width;
		public int height;

		public bool[] walls;

		List<Room> nearestNeighbors;

		Dictionary<int, Memory> memories;

		public enum Wall {top, bottom, left, right};

		public Room (int x, int y, int width, int height)
		{
			this.centerX = x;
			this.centerY = y;
			this.width = width;
			this.height = height;
			nearestNeighbors = new List<Room> ();
			walls = new bool[4];
			memories = new Dictionary<int, Memory>();
		}

		public void addNearestNeighbors(Room neighbor) {
			nearestNeighbors.Add (neighbor);
		}

		public double getDistanceFrom(Room neighbor) {
			double dx = centerX - neighbor.centerX;
			double dy = centerY - neighbor.centerY;
			return Math.Sqrt (dx * dx + dy * dy);
		}

		public void makeDoor(Wall wall) {
			walls [(int)wall] = true;
		}

		public void addMemory(int pos, Memory mem) {
			memories.Add (pos, mem);
		}

		public Dictionary<int, Memory> getMemories() {
			return memories;
		}
	}
}

