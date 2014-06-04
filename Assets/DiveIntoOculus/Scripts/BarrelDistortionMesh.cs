using UnityEngine;
using System.Collections;

public class BarrelDistortionMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//ICoordConverter converter = new UniformCoordConverter ();
		ICoordConverter converter = new NonUniformCoordConverter (NonUniformEaseType.Expo);
		GridGenerator generator = new GridGenerator (converter);
		Mesh mesh = generator.CreateBase (128, 128);
		//Mesh mesh = generator.CreateBase (2, 2);
		//Mesh mesh = generator.CreateBase (4, 4);
		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
