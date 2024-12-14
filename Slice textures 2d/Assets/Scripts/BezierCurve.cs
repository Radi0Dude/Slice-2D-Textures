using System.Collections;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
	[Range(0, 1), SerializeField]
	float t = 0;

	[SerializeField]
	GameObject hello;
	[SerializeField]
	Transform[] points;

	void Start()
	{
		t = 0;
		
		SpawnObjectAlongCurve();
	}
	Vector3 LinearBezier(Vector3 a, Vector3 b, float t)
	{
		return a + (b - a) * t;
	}

	Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		Vector3 p0 = LinearBezier(a, b, t);
		Vector3 p1 = LinearBezier(b, c, t);
		return LinearBezier(p0, p1, t);
	}

	Vector3 CubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
	{
		Vector3 p0 = QuadraticBezier(a, b, c, t);
		Vector3 p1 = QuadraticBezier(b, c, d, t);
		return LinearBezier(p0, p1, t);
	}


	Vector3 RecursiveBezier(Vector3[] points, float t)
	{
		if (points.Length == 1)
		{
			return points[0];
		}

		Vector3[] newPoints = new Vector3[points.Length - 1];
		for (int i = 0; i < points.Length - 1; i++)
		{
			newPoints[i] = LinearBezier(points[i], points[i + 1], t);
		}

		return RecursiveBezier(newPoints, t);
	}

	Vector3 AutomaticallyGetCorrectBezier(int amountOfPoints)
	{
		if (amountOfPoints < 2)
		{
			Debug.LogError("You need to have at least 2 points to create a Bezier curve");
			return transform.position;
		}

		switch (amountOfPoints)
		{
			case 2:
				return LinearBezier(points[0].position, points[1].position, t);
			case 3:
				return QuadraticBezier(points[0].position, points[1].position, points[2].position, t);
			case 4:
				return CubicBezier(points[0].position, points[1].position, points[2].position, points[3].position, t);
			default:
				Vector3[] positions = new Vector3[amountOfPoints];
				for (int i = 0; i < amountOfPoints; i++)
				{
					positions[i] = points[i].position;
				}
				return RecursiveBezier(positions, t);
		}
	}

	void SpawnObjectAlongCurve()
	{
		if (points == null || points.Length == 0)
		{
			Debug.LogError("Points array is not initialized or empty.");
			return;
		}

		StartCoroutine(testing());
	}
	IEnumerator testing()
	{
		float initialT = t;
		for (int i = 0; i < 100; i++)
		{
			GameObject go = Instantiate(hello, Vector3.zero, Quaternion.identity);
			go.transform.position = AutomaticallyGetCorrectBezier(points.Length);
			t += 0.01f;
			if (t > 1)
			{
				t = 0;
			}
			yield return new WaitForEndOfFrame();
		}
		t = initialT;
	}
	void OnDrawGizmos()
	{
		for (int i = 0; i < points.Length - 1; i++)
		{
			Gizmos.DrawLine(points[i].position, points[i + 1].position);
		}

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(AutomaticallyGetCorrectBezier(points.Length), 0.5f);
	}

	private void Update()
	{
		//transform.position = AutomaticallyGetCorrectBezier(points.Length);


	}
}
