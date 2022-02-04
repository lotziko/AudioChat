using System.Collections;
using TMPro;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class MessagePanelItem : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _group;
		[SerializeField] private TMP_Text _messageText;
		[SerializeField] private float _lifetime;

		public void Initialize(string text)
		{
			_messageText.text = text;
			StartCoroutine(LifetimeCoroutine(_lifetime));
		}

		private IEnumerator LifetimeCoroutine(float time)
		{
			yield return new WaitForSeconds(time);
			yield return CoroutineInterpolator.InterpolateUnmanaged(this, 1f, 0f, 1f,
				(float alpha) =>
				{
					_group.alpha = alpha;
				});
			Destroy(gameObject);
		}
	}
}
