using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Palace {

	/**
	 * Used for testing/debugging photo uploading feature.
	 * **/
	public class PhotoCreatorScript : MonoBehaviour {

		public string photoFolder;

		// /Users/Wenli/Desktop/Memory Palace/Assets/Photos
		public GameObject pictureFrame;


		void Start () {
			if (File.Exists (photoFolder)) {
				string[] photoNames = Directory.GetFiles (photoFolder);
				for (int s = 0; s < photoNames.Length; s++) {
					Debug.Log ("PHOTO NAME: " + photoNames [s]);
				}
			}
			getTexture ("/Users/Wenli/Desktop/Memory Palace/Assets/Photos/IMG_9253");
		}

		void getTexture(string filePath) {
			if (File.Exists(filePath))     {
				byte [] fileData = File.ReadAllBytes(filePath);
				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

				//return tex;

				GameObject pic = (GameObject)Instantiate(pictureFrame, new Vector3(0,1,0), Quaternion.AngleAxis(180, new Vector3(0,1,0)));

				Component [] shaders = pic.GetComponents(typeof(MeshRenderer));
				Component shader = shaders[0];
				MeshRenderer meshRender = (MeshRenderer) shader;

				// For now, 0 is art, 1 is picture
				Material pictMaterial = meshRender.materials[1];

				pictMaterial.mainTexture = tex;
			}
		}
	
		// Update is called once per frame
		void Update () {
			
		}

	}
}