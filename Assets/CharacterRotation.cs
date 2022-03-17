using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotation : MonoBehaviour
{
	private float _rotatespeed = 100f;

	private float _startingPosition;

	private void Update()
	{
		if (Input.touchCount > 0)
		{
			var touch = Input.GetTouch(0);
			var id = touch.fingerId;
			
				switch (touch.phase)
				{
					case TouchPhase.Began:
						_startingPosition = touch.position.x;
						break;
					case TouchPhase.Moved:
						if (_startingPosition > touch.position.x)
						{
							transform.Rotate(Vector3.up, _rotatespeed * Time.deltaTime);
						}
						else if (_startingPosition < touch.position.x)
						{
							transform.Rotate(Vector3.up, -_rotatespeed * Time.deltaTime);
						}

						break;
				}
			
		}
	}
}