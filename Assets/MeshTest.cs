using UnityEngine;
using System.Collections;

public class MeshTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//BaseCoordConverter converter = new UniformCoordConverter ();
		BaseCoordConverter converter = new NonUniformCoordConverter (NonUniformEaseType.Quad);
		GridGenerator generator = new GridGenerator (converter);
		Mesh mesh = generator.CreateBase (40, 40);
		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
