using System;
using UnityEngine;

namespace Palace
{
	public class Memory
	{
		Texture2D photoTexture;
		public Memory (Texture2D tex)
		{
			photoTexture = tex;
		}

		public Texture2D getTexture() {
			return photoTexture;
		}
	}
}

