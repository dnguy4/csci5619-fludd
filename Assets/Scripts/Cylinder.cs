using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cylinder : MonoBehaviour
{
	public Action<GameObject, GameObject> onTriggerEntered;
	public Action<GameObject, GameObject> onTriggerExited;
	public HashSet<GameObject> currentCollisions;
	public float radius;

	Collider myCollider; 
	// Start is called before the first frame update
	void Start()
	{
		myCollider = GetComponent<Collider>();
		currentCollisions = new HashSet<GameObject>();
		radius = transform.localScale.x;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ModifyRadius(float newRadius)
    {
		transform.localScale = new Vector3(newRadius, transform.localScale.y, newRadius);
		radius = newRadius;
    }

	private void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<Outline>()) return;
		currentCollisions.Add(other.gameObject);

		// If there are any listeners...
		if (onTriggerEntered != null)
		{
			// ...broadcast the callback.
			onTriggerEntered(gameObject, other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<Outline>()) return;
		currentCollisions.Remove(other.gameObject);
		if (onTriggerExited != null)
		{
			onTriggerExited(gameObject, other.gameObject);
		}
	}

    private void OnDisable()
    {
		foreach (GameObject g in currentCollisions)
        {
			onTriggerExited(gameObject, g);
		}
		currentCollisions.Clear();
    }
}
