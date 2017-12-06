using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Palace
{
	public class PhotoCreator
	{
		private string photoFolder;

		public PhotoCreator (string filePath)
		{
			photoFolder = filePath;
		}

		public List<string> getPhotoFiles () {
			List<string> photoFiles = new List<string> ();
			string[] photoNames = Directory.GetFiles (photoFolder);
			for (int s = 0; s < photoNames.Length; s++) {
				if (photoNames [s].EndsWith ("jpg")) {
					photoFiles.Add (photoNames [s]);
				}
			}
			return photoFiles;
		}

		public Texture2D getTexture(string filePath) {
			try
			{
				if (File.Exists(filePath))     {
					byte [] fileData = File.ReadAllBytes(filePath);
					Texture2D tex = new Texture2D(2, 2);
					tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

					return tex;
				} else {
					throw new FileNotFoundException();
				}
			}
			catch (FileNotFoundException ioEx)
			{
				throw ioEx;
			}
		}

	}
}

