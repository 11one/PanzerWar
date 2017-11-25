using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Plugins.ButtonSoundsEditor
{
    public class ButtonClickSound : MonoBehaviour
    {
        public AudioSource AudioSource;
        public AudioClip ClickSound;

        public void Start()
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(PlayClickSound);
            }

            EventTrigger eventTrigger = GetComponent<EventTrigger>();
            if (eventTrigger != null)
            {
                EventTrigger.Entry clickEntry = eventTrigger.triggers.SingleOrDefault(_ => _.eventID == EventTriggerType.PointerClick);
                if (clickEntry != null)
                    clickEntry.callback.AddListener(_ => PlayClickSound());
            }
			if (AudioSource == null) {
				AudioSource =GameObject.FindObjectOfType<Camera>().gameObject.AddComponent<AudioSource> ();
			}
        }

        private void PlayClickSound()
        {
			if (AudioSource == null) {
				AudioSource =GameObject.FindObjectOfType<Camera>().gameObject.AddComponent<AudioSource> ();
			}

            AudioSource.PlayOneShot(ClickSound);
        }
    }

}
