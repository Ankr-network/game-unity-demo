using UnityEngine;

namespace AnkrDemo
{
	public class CharacterRotation : MonoBehaviour
	{
		private readonly float _rotatespeed = 100f;

		private float _startingPosition;
		private bool _canRotate = false;

		private void Update()
		{
			RotateTransformOnFingerDrag();
		
			if(Application.isEditor)
			{
				RotateTransformOnMouseDrag();
			}
		}

		private void RotateTransformOnFingerDrag()
		{
			if (Input.touchCount > 0)
			{
				var touch = Input.GetTouch(0);
				
				switch (touch.phase)
				{
					case TouchPhase.Began:
						RaycastHit2D hit;
						hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
						if (hit)
						{
							_canRotate = true;
							_startingPosition = touch.position.x;
						}
						break;
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						if (_canRotate)
						{
							var angleRotation = _rotatespeed * Time.deltaTime;

							if (_startingPosition > touch.position.x)
							{
								transform.Rotate(Vector3.up, angleRotation);
							}
							else if (_startingPosition < touch.position.x)
							{
								transform.Rotate(Vector3.up, -angleRotation);
							}
						}
						break;
					case TouchPhase.Ended:
						_canRotate = false;
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
}