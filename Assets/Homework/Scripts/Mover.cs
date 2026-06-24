using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
	[SerializeField]
	private Vector3 _start;
	[SerializeField]
	private Vector3 _end;
	[SerializeField, Min(0.01f)]
	private float _speed = 1f;
	[SerializeField, Min(0f)]
	private float _delay = 1f;
	private WaitForSeconds _waitForDelay;

	private IEnumerator Start()
	{
		var body = GetComponent<Rigidbody>();
		body.isKinematic = true;
		body.position = _start;

		var target = _end;
		var waitForFixedUpdate = new WaitForFixedUpdate();

		_waitForDelay = new WaitForSeconds(_delay);

		while (true)
		{
			while (Vector3.Distance(body.position, target) > Mathf.Epsilon)
			{
				var position = Vector3.MoveTowards(body.position, target, _speed * Time.fixedDeltaTime);
				body.MovePosition(position);

				yield return waitForFixedUpdate;
			}

			body.MovePosition(target);
			yield return _waitForDelay;

			target = target == _end ? _start : _end;
		}
	}
}
