using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
    [AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Plume")]
    public class SmokePlume : TrailRenderer_Base
    {
        public float TimeBetweenPoints = 0.1f;
        public Vector3 ConstantForce = Vector3.up * 0.5f;
        public float RandomForceScale = 0.05f;
        private float _timeSincePoint;

        protected override void Start()
        {
            base.Start();
            _timeSincePoint = 0;
        }

        protected override void OnStartEmit()
        {
            _timeSincePoint = 0;
        }

        protected override void Reset()
        {
            base.Reset();
            TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.2f), new Keyframe(1, 0.2f));
            TrailData.Lifetime = 6f;
            ConstantForce = Vector3.up*0.5f;
            TimeBetweenPoints = 0.1f;
            RandomForceScale = 0.05f;
        }

        protected override void Update()
        {
			#if ClientCode
            if (_emit)
            {
                _timeSincePoint += Time.deltaTime;

                if (_timeSincePoint >= TimeBetweenPoints)
                {
                    AddPoint(new SmokeTrailPoint(), _t.position);
                    _timeSincePoint = 0;
                }
            }

            base.Update();
			#endif
        }

        protected override void InitialiseNewPoint(PCTrailPoint newPoint)
        {
            ((SmokeTrailPoint)newPoint).RandomVec = Random.onUnitSphere * RandomForceScale;
        }

        protected override void UpdatePoint(PCTrailPoint point, float deltaTime)
        {
            point.Position += ConstantForce*deltaTime;
        }
    }

    
}
