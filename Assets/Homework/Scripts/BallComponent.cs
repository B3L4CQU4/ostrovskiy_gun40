using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Rigidbody))]
	public class BallComponent : MonoBehaviour
	{
		private Rigidbody _body;

		[SerializeField]
		private float _startVelocity;
		[SerializeField]
		private float _lifetime;

		private void Awake()
		{
			_body = GetComponent<Rigidbody>();
		}

		public void AttachTo(Transform parent)
		{
			transform.SetParent(parent);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

			_body.isKinematic = true;
		}

		public void Launch(Vector3 direction)
		{
			transform.SetParent(null);

			_body.isKinematic = false;
			_body.velocity = direction.normalized * _startVelocity;

			Destroy(gameObject, _lifetime);
		}
	}
}
