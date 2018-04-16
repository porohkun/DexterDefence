using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using MimiJson;

namespace Game
{
    public class MultiShotAI : SingleShotAI
    {
        private float _queueDelay;
        private int _shots;

        private float _lastDeltaTime;
        private IEnumerator<bool> _shootingRoutine = null;

        public MultiShotAI(JsonArray turretData) : base(turretData)
        {
        }

        protected override void UpgradeToLevel(JsonValue data)
        {
            base.UpgradeToLevel(data);
            _queueDelay = data["queue_delay"];
            _shots = data["shots"];
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            _lastDeltaTime = deltaTime;
            if (_shootingRoutine != null)
            {
                _shootingRoutine.MoveNext();
                    if (_shootingRoutine.Current)
                    _shootingRoutine = null;
            }
        }

        protected override void Shot()
        {
            _shootingRoutine = ShotRoutine();
        }

        private IEnumerator<bool> ShotRoutine()
        {
            var time = 0f;
            var shots = 0;
            while (true)
            {
                time += _lastDeltaTime;
                if (time >= _queueDelay)
                {
                    base.Shot();
                    shots++;
                    time -= _queueDelay;
                }
                if (shots >= _shots)
                    break;
                yield return false;
            }
            yield return true;
        }
    }
}
