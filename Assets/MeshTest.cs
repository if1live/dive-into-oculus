using UnityEngine;
using System.Collections;

public class MeshTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		BaseGridGenerator generator = new UniformGridGenerator ();
		Mesh mesh = generator.CreateBase (4, 3);
		MeshFilter filter = GetComponent<MeshFilter> ();
		filter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
