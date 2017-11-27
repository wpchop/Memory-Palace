using System;
using System.Collections.Generic;

namespace Palace {
	public class Room
	{
		public int centerX;
		public int centerY;
		public int width;
		public int height;

		List<Room> nearestNeighbors;

		public Room (int x, int y, int width, int height)
		{
			this.centerX = x;
			this.centerY = y;
			this.width = width;
			this.height = height;
			nearestNeighbors = new List<Room> ();
		}

		public void addNearestNeighbors(Room neighbor) {
			nearestNeighbors.Add (neighbor);
		}

		public double getDistanceFrom(Room neighbor) {
			double dx = centerX - neighbor.centerX;
			double dy = centerY - neighbor.centerY;
			return Math.Sqrt (dx * dx + dy * dy);
		}

		void onDrawGizmos() {

		}
	}
}

