using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MeshFilter))]
public class Cone : MonoBehaviour {

	public int subdivisions = 5;
	public float radius = 0.3f;
	public float height = 2f;

	Mesh coneMesh;
	MeshCollider coneCollider;

	void Start () {
		coneMesh = Create(subdivisions, radius, height);
		coneMesh.name = "ConeMesh";
		this.GetComponent<MeshFilter>().sharedMesh = coneMesh;
		coneCollider = GetComponent<MeshCollider>();
		coneCollider.sharedMesh = coneMesh;

		//GetComponent<MeshFilter>().sharedMesh = Create(subdivisions, radius, height);
	}
	
	void Update () {
	}

	public Mesh Create (int subdivisions, float radius, float height) {
		Mesh mesh = new Mesh();

		Vector3[] vertices = new Vector3[subdivisions + 2];
		Vector2[] uv = new Vector2[vertices.Length];
		int[] triangles = new int[(subdivisions * 2) * 3];

		vertices[0] = Vector3.zero;
		uv[0] = new Vector2(0.5f, 0f);
		for(int i = 0, n = subdivisions - 1; i < subdivisions; i++) {
			float ratio = (float)i / n;
			float r = ratio * (Mathf.PI * 2f);
			float x = Mathf.Cos(r) * radius;
			float z = Mathf.Sin(r) * radius;
			vertices[i + 1] = new Vector3(x, 0f, z);

			// Debug.Log (ratio);
			uv[i + 1] = new Vector2(ratio, 0f);
		}
		vertices[subdivisions + 1] = new Vector3(0f, height, 0f);
		uv[subdivisions + 1] = new Vector2(0.5f, 1f);

		// construct bottom

		for(int i = 0, n = subdivisions - 1; i < n; i++) {
			int offset = i * 3;
			triangles[offset] = 0; 
			triangles[offset + 1] = i + 1; 
			triangles[offset + 2] = i + 2; 
		}

		// construct sides

		int bottomOffset = subdivisions * 3;
		for(int i = 0, n = subdivisions - 1; i < n; i++) {
			int offset = i * 3 + bottomOffset;
			triangles[offset] = i + 1; 
			triangles[offset + 1] = subdivisions + 1; 
			triangles[offset + 2] = i + 2; 
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		return mesh;
	}

	public void ModifyVertices(float newRadius, float newHeight)
    {
		radius = newRadius;
		height = newHeight;

		Vector3[] vertices = coneMesh.vertices;
		vertices[0] = Vector3.zero;
		for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
		{
			float ratio = (float)i / n;
			float r = ratio * (Mathf.PI * 2f);
			float x = Mathf.Cos(r) * radius;
			float z = Mathf.Sin(r) * radius;
			vertices[i + 1] = new Vector3(x, 0f, z);
		}
		vertices[subdivisions + 1] = new Vector3(0f, height, 0f);

		coneMesh.vertices = vertices;
		coneMesh.RecalculateBounds();
		coneMesh.RecalculateNormals();
	}

    private void OnTriggerEnter(Collider other)
    {
		Debug.Log("Hit " + other.name);
		Renderer r = other.GetComponent<Renderer>();
		Color oldColor = r.material.color;
		Debug.Log(oldColor);
		Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
		r.material.SetColor("_Color", newColor);
    }

    private void OnTriggerExit(Collider other)
    {
		Renderer r = other.GetComponent<Renderer>();
		Color oldColor = r.material.color;
		Debug.Log(oldColor);
		Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1f);
		r.material.SetColor("_Color", newColor);
	}
}