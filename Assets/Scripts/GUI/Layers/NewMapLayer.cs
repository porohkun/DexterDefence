using UnityEngine;
using UnityEngine.UI;

namespace Layers
{
    public class NewMapLayer : LayerBase
    {
        [SerializeField]
        private InputField _filenameField;
        [SerializeField]
        private InputField _widthField;
        [SerializeField]
        private InputField _heightField;

        public string Filename { get { return _filenameField.text; } }
        public int Width { get { return int.Parse(_widthField.text); } }
        public int Height { get { return int.Parse(_heightField.text); } }
        public bool OkPressed { get; private set; }

        public void OnOk()
        {
            OkPressed = true;
            LayersManager.Pop();
        }

        public override void OnQuit()
        {
            OkPressed = false;
            base.OnQuit();
        }
    }
}
