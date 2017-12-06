using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotoCreatorScript : MonoBehaviour {

	public string filepath;

	public GameObject pictureFrame;

	private bool reached = false;

	// Use this for initialization
	void Start () {

		// Specify a file to read from and to create.
		string pathSource = "/Users/Wenli/Desktop/Memory Palace/Assets/Photos/IMG_9253.jpg";

		if (File.Exists (pathSource)) {
			string[] photoNames = Directory.GetFiles ("/Users/Wenli/Desktop/Memory Palace/Assets/Photos");
			for (int s = 0; s < photoNames.Length; s++) {
				Debug.Log ("PHOTO NAME: " + photoNames [s]);
			}
		}

		try
		{
			if (File.Exists(pathSource))     {
				byte [] fileData = File.ReadAllBytes(pathSource);

				Texture2D tex = new Texture2D(2, 2);
				tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

				GameObject pic = (GameObject)Instantiate(pictureFrame, new Vector3(0,1,0), Quaternion.AngleAxis(180, new Vector3(0,1,0)));

				Component [] shaders = pic.GetComponents(typeof(MeshRenderer));
				Component shader = shaders[0];
				MeshRenderer meshRender = (MeshRenderer) shader;

				// For now, 0 is art, 1 is picture. Unsure why.
				// Debug.Log("Mesh renderer material: " + meshRender.materials[1]);

				Material pictMaterial = meshRender.materials[1];

				pictMaterial.mainTexture = tex;

				//reached = true;
			}
		}
		catch (FileNotFoundException ioEx)
		{
			throw ioEx;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos() {
		if (reached) {
			Gizmos.DrawCube (new Vector3 (0, 0, 0), Vector3.one);
		}
	}

}
