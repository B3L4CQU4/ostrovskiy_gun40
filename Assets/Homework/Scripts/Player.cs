using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
	public class Player : MonoBehaviour
	{
		private bool _ready;
		private BallComponent _ball;
		
		[SerializeField]
		private BallComponent _ballPrefab;
		[SerializeField]
		private Transform _ballSpawnPoint;
		[SerializeField]
		private float _respawnDelay;
		private WaitForSeconds _waitForRespawnDelay;

		private void Update()
		{
			if (!_ready) return;
			if (Input.GetKey(KeyCode.Space))
			{
				_ready = false;
				var direction = transform.forward;
				direction.y = 0;
				direction.Normalize();	
				_ball.Launch(direction);
				StartCoroutine(Reload());
			}
		}

		private IEnumerator Reload()
		{
			yield return _waitForRespawnDelay;
			Spawn();
		}

		private void Spawn()
		{
			_ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, _ballSpawnPoint.rotation);
			_ball.AttachTo(_ballSpawnPoint);
			_ready = true;
		}

		private void Start()
		{
			_waitForRespawnDelay = new WaitForSeconds(_respawnDelay);
			Spawn();
		}
	}
}
