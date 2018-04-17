using UnityEngine;
using UnityEngine.UI;

namespace Layers
{
    public class EndGameLayer : LayerBase
    {
        [SerializeField]
        private Text _message;

        public void Initialize(bool win)
        {
            _message.text = win ? "You win" : "You lose";
        }

        public void OnRestart()
        {
            LayersManager.FadeOut(0.5f, () =>
            {
                LayersManager.PopTill<GameLayer>();
                LayersManager.FadeIn(0.5f, null);
                LayersManager.GetLayer<GameLayer>().Restart();
            });
        }

        public void OnMenu()
        {
            LayersManager.FadeOut(0.5f, () =>
            {
                LayersManager.GetLayer<GameLayer>().Clear();
                LayersManager.PopTill<MainMenuLayer>();
                LayersManager.FadeIn(0.5f, null);
            });
        }
    }
}
