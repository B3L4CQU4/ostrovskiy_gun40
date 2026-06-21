using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Rigidbody))]
	public class Rotator : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _rotate;

		private IEnumerator Start()
		{
			var body = GetComponent<Rigidbody>();
			body.isKinematic = true;
			body.interpolation = RigidbodyInterpolation.Interpolate;

			var waitForFixedUpdate = new WaitForFixedUpdate();

			while (true)
			{
				var rotation = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
				body.MoveRotation(body.rotation * rotation);

				yield return waitForFixedUpdate;
			}
		}
	}
}
