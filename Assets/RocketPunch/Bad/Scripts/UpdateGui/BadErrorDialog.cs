using UnityEngine;
using UnityEngine.UI;

namespace RocketPunch.Bad
{
    public class BadErrorDialog : MonoBehaviour
    {
        public Text txtMessage;
        
        public Button btnConfirm;

        public string message
        {
            get => this.txtMessage.text;
            set => this.txtMessage.text = value;
        }
        
        private void Awake()
        {
            btnConfirm.onClick.AddListener( this.OnConfirm );
        }
        
        private void OnDestroy()
        {
            btnConfirm.onClick.RemoveListener( this.OnConfirm );
        }

        private void OnConfirm()
        {
            this.gameObject.SetActive( false );
        }
    }
}