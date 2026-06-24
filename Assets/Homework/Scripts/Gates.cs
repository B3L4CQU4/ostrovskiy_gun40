using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Collider))]
	public class Gates : MonoBehaviour
	{
		private static int _score;

		private void Awake()
		{
			var collider = GetComponent<Collider>();
			collider.isTrigger = true;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!TryGetBall(other, out var ball)) return;

			_score++;
			Debug.Log($"Текущий игровой счёт: {_score}");
			Destroy(ball.gameObject);
		}

		private bool TryGetBall(Collider other, out Ball ball)
		{
			return other.TryGetComponent(out ball);
		}

	}
}
