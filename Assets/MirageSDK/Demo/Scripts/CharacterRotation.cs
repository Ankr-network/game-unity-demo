using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
	private readonly float _rotatespeed = 100f;

	private float _startingPosition;

	private void Update()
	{
	#if UNITY_ANDROID
		RotateTransformOnFingerDrag();
	#endif
	#if UNITY_STANDALONE_WIN
		RotateTransformOnMouseDrag();
	#endif
	}

	private void RotateTransformOnFingerDrag()
	{
		if (Input.touchCount > 0)
		{
			var touch = Input.GetTouch(0);

			switch (touch.phase)
			{
				case TouchPhase.Began:
					_startingPosition = touch.position.x;
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					var angleRotation = _rotatespeed * Time.deltaTime;

					if (_startingPosition > touch.position.x)
					{
						transform.Rotate(Vector3.up, angleRotation);
					}
					else if (_startingPosition < touch.position.x)
					{
						transform.Rotate(Vector3.up, -angleRotation);
					}

					break;
			}
		}
	}

	private void RotateTransformOnMouseDrag()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_startingPosition = Input.mousePosition.x;
		}

		if (Input.GetMouseButton(0))
		{
			var posDelta = _startingPosition - Input.mousePosition.x;
			var angleRotation = _rotatespeed * Time.deltaTime;

			if (posDelta > 0)
			{
				transform.Rotate(Vector3.up, angleRotation);
			}
			else
			{
				transform.Rotate(Vector3.up, -angleRotation);
			}
		}
	}
}