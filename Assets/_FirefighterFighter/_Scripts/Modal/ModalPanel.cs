using UnityEngine;

namespace FirefighterFighter.Modal
{
    public class ModalPanel : MonoBehaviour
    {
        [SerializeField] private GameObject[] _panelsToClose;

        private void OnEnable()
        {
            foreach (var panel in _panelsToClose)
            {
                if (panel != gameObject) 
                {
                    panel.SetActive(false);
                }
            }
        }
    }
}