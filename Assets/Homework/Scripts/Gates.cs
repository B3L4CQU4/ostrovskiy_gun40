using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Collider))]
	public class Gates : MonoBehaviour
	{
		[SerializeField]
		private bool _useTagFilter;
		[SerializeField]
		private string _ballTag = "Ball";

		private static int _score;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetScore()
		{
			_score = 0;
		}

		private void Reset()
		{
			EnableTrigger();
		}

		private void Awake()
		{
			EnableTrigger();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!TryGetBall(other, out var ball)) return;

			_score++;
			Debug.Log($"\u0422\u0435\u043a\u0443\u0449\u0438\u0439 \u0438\u0433\u0440\u043e\u0432\u043e\u0439 \u0441\u0447\u0435\u0442: {_score}");
			Destroy(ball.gameObject);
		}

		private bool TryGetBall(Collider other, out Rigidbody ball)
		{
			ball = other.attachedRigidbody;

			if (ball == null) return false;
			if (!_useTagFilter) return true;

			return ball.gameObject.tag == _ballTag || other.gameObject.tag == _ballTag;
		}

		private void EnableTrigger()
		{
			var collider = GetComponent<Collider>();
			collider.isTrigger = true;
		}
	}
}
