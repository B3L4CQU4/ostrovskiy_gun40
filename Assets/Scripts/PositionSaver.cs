using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		[Serializable]
		private class SaveData
		{
			public List<Data> Records;
		}

		[SerializeField, ReadOnly]
		[Tooltip("Для заполнения этого поля воспользуйтесь контекстным меню в инспекторе и командой \"Create File\".")]
		private TextAsset _json;

		[field: SerializeField, HideInInspector]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			// Ответ: Метод продолжит выполняться и при обращении к _json.text ниже возникнет NullReferenceException.
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			if (!string.IsNullOrWhiteSpace(_json.text))
			{
				var data = JsonUtility.FromJson<SaveData>(_json.text);
				Records = data?.Records;
			}
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			// Ответ: Она создаёт пустой список, если данные из json не заполнили Records, и позволяет избежать обращений к null.
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			// Ответ: Они не дают рисовать путь без данных и предотвращают ошибки при обращении к Records и data[0].
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			// Ответ: Нулевой элемент уже взят как предыдущая точка, поэтому дальше нужно рисовать отрезки от неё к следующим точкам.
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			// Ответ: В папке Assets создаётся файл Path.txt, а в переменную stream сохраняется открытый поток этого файла.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			// Ответ: Она закрывает поток и освобождает файл, чтобы Unity могла импортировать его как ассет и чтобы файл не оставался заблокированным.
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				// Ответ: Они убеждаются, что ассет успешно загружен и что это именно созданный файл Path, а не любой другой TextAsset.
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					// Ответ: Нужный ассет уже найден и записан в _json, поэтому дальнейший поиск только лишний и может перезаписать результат.
					return;
				}
			}
		}

		private void OnDestroy()
		{
			if (_json == null) return;

			var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
			if (string.IsNullOrEmpty(path)) return;

			var projectPath = Directory.GetParent(Application.dataPath)?.FullName;
			if (string.IsNullOrEmpty(projectPath)) return;

			var data = new SaveData { Records = Records };
			File.WriteAllText(Path.Combine(projectPath, path), JsonUtility.ToJson(data, true));
			UnityEditor.AssetDatabase.Refresh();
		}
#endif
	}
}
