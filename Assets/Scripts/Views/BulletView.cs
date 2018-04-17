using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game;
using UnityEngine;
using Managers;

namespace Views
{
    public class BulletView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] _renderersForDestroy;
        [SerializeField]
        private ParticleSystem _particleSystem;
        [SerializeField]
        private float _destroyDelay;
        [SerializeField]
        private Transform _stopPrefab;

        public float DestroyDelay { get { return _destroyDelay; } }

        private BulletModel _bullet;
        private bool _flying = true;

        public void AttachTo(BulletModel bullet)
        {
            _bullet = bullet;
            Update();
        }

        private void Update()
        {
            if (_flying)
            {
                transform.localPosition = GraphicsManager.Scale(_bullet.Position);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, _bullet.Direction);
            }
        }

        internal void Stop()
        {
            _flying = false;
            foreach (var renderer in _renderersForDestroy)
                renderer.enabled = false;
            if (_particleSystem != null)
                _particleSystem.Stop();
            if (_stopPrefab != null)
            {
                var stopObject = Instantiate(_stopPrefab, transform);
                stopObject.localPosition = Vector3.back * 5f;
                stopObject.localScale = Vector3.one * 1.5f;
            }
        }
    }
}
