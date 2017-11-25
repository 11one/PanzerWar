using UnityEngine;
using System.Collections;

public class AnimationSpline
{
	public AnimationCurve _curveX;
	public AnimationCurve _curveY;
	public AnimationCurve _curveZ;

	public AnimationCurve curveX {
		get { return this._curveX; }
	}

	public AnimationCurve curveY {
		get { return this._curveY; }
	}

	public AnimationCurve curveZ {
		get { return this._curveZ; }
	}

	private SplineKeyframe[] _keys;
	private float tracksDisplacement = 0.0f;

	public AnimationSpline (WrapMode wrapMode)
	{
		CreateSpline (wrapMode);
	}

	public AnimationSpline (WrapMode wrapMode, SplineKeyframe[] keys)
	{
		CreateSpline (wrapMode);
		SetKeys (keys);
	}

	private void CreateSpline (WrapMode wrapMode)
	{
		_curveX = new AnimationCurve ();
		_curveY = new AnimationCurve ();
		_curveZ = new AnimationCurve ();
		
		SetWrapMode (wrapMode);
	}

	public void SetWrapMode (WrapMode wrapMode)
	{
		_curveX.preWrapMode = wrapMode;
		_curveX.postWrapMode = wrapMode;
		
		_curveY.preWrapMode = wrapMode;
		_curveY.postWrapMode = wrapMode;
		
		_curveZ.preWrapMode = wrapMode;
		_curveZ.postWrapMode = wrapMode;
	}

	public void AddKey (SplineKeyframe valueKeyframe)
	{
		_curveX.AddKey (valueKeyframe.time, valueKeyframe.valueX);
		_curveY.AddKey (valueKeyframe.time, valueKeyframe.valueY);
		_curveZ.AddKey (valueKeyframe.time, valueKeyframe.valueZ);
	}

	public void AddKey (float time, float valueX, float valueY, float valueZ)
	{
		_curveX.AddKey (time, valueX);
		_curveY.AddKey (time, valueY);
		_curveZ.AddKey (time, valueZ);
	}

	public void AddKey (float time, Vector3 valueKeyframe)
	{
		_curveX.AddKey (time, valueKeyframe.x);
		_curveY.AddKey (time, valueKeyframe.y);
		_curveZ.AddKey (time, valueKeyframe.z);
	}

	public void SmoothTangents (int index, float weight)
	{
		_curveX.SmoothTangents (index, weight);
		_curveY.SmoothTangents (index, weight);
		_curveZ.SmoothTangents (index, weight);
	}

	public void Smooth ()
	{
		for (int i = 0; i < this.length; i++)
			this.SmoothTangents (i, 0.0f);
	}

	public Vector3 GetTangentsVector (float time, float kaeficient)
	{
		Vector3 pos0;
		Vector3 pos;
		float delta = kaeficient;
		pos0 = Evaluate (time);
		pos = Evaluate (time + delta); 
		return (1.0f / delta) * (pos - pos0);
	}

	public Vector3 GetNormalVector (float time, float kaeficient)
	{
		Vector3 norm0;
		Vector3 norm;
		float delta = kaeficient;
		norm0 = GetTangentsVector (time, delta).normalized;
		norm = GetTangentsVector (time + delta, delta).normalized; 
		return ((1.0f / delta) * (norm - norm0)).normalized;
	}

	public void RemoveKey (int index)
	{
		_curveX.RemoveKey (index);
		_curveY.RemoveKey (index);
		_curveZ.RemoveKey (index);
	}

	public SplineKeyframe[] keys {
		get { return this._keys; }
		set {
			SetKeys (value);
		}
	}

//	public void InitKey (SplineKeyframe[] valueKeyframe)
//	{
//		this._keys = valueKeyframe;
//		int _length = length;
//		Keyframe[] KeyframeX = new Keyframe[_length];
//		Keyframe[] KeyframeY = new Keyframe[_length];
//		Keyframe[] KeyframeZ = new Keyframe[_length];
//		for (int i = 0; i < _length; i++) {
//			KeyframeX [i] = new Keyframe (0,0,0,0);
//			KeyframeX [i].tangentMode = 0;
//			KeyframeY [i] = new Keyframe (0,0,0,0);
//			KeyframeY [i].tangentMode = 0;
//			KeyframeZ [i] = new Keyframe (0,0,0,0);
//			KeyframeZ [i].tangentMode = 0;
//		}
//		_curveX.keys = KeyframeX;
//		_curveY.keys = KeyframeY;
//		_curveZ.keys = KeyframeZ;
//	}

	private void SetKeys (SplineKeyframe[] valueKeyframe)
	{
		
		this._keys = valueKeyframe;
		int _length = length;

		Keyframe[] KeyframeX = new Keyframe[_length];
		Keyframe[] KeyframeY = new Keyframe[_length];
		Keyframe[] KeyframeZ = new Keyframe[_length];
		for(int i = 0; i < _length; i++)
		{
			KeyframeX[i] = new Keyframe(this._keys[i].time, this._keys[i].valueX, this._keys[i].inTangent, this._keys[i].outTangent);
			KeyframeX[i].tangentMode = this._keys[i].tangentMode;
			KeyframeY[i] = new Keyframe(this._keys[i].time, this._keys[i].valueY, this._keys[i].inTangent, this._keys[i].outTangent);
			KeyframeY[i].tangentMode = this._keys[i].tangentMode;
			KeyframeZ[i] = new Keyframe(this._keys[i].time, this._keys[i].valueZ, this._keys[i].inTangent, this._keys[i].outTangent);
			KeyframeZ[i].tangentMode = this._keys[i].tangentMode;
		}
		_curveX.keys = KeyframeX;
		_curveY.keys = KeyframeY;
		_curveZ.keys = KeyframeZ;

//		for (int i = 0; i < _length; i++) {
////			_curveX.keys [i].time = _keys [i].time;
////			_curveX.keys [i].value = _keys [i].valueX;
////			_curveX.keys [i].inTangent = _keys [i].inTangent;
////			_curveX.keys [i].outTangent = _keys [i].outTangent;
////
////			_curveY.keys [i].time = _keys [i].time;
////			_curveY.keys [i].value = _keys [i].valueY;
////			_curveY.keys [i].inTangent = _keys [i].inTangent;
////			_curveY.keys [i].outTangent = _keys [i].outTangent;
////
////			_curveZ.keys [i].time = _keys [i].time;
////			_curveZ.keys [i].value = _keys [i].valueZ;
////			_curveZ.keys [i].inTangent = _keys [i].inTangent;
////			_curveZ.keys [i].outTangent = _keys [i].outTangent;
//
//			SetKeyFrameHelper (ref _curveX, i, this._keys [i].time, this._keys [i].valueX, this._keys [i].inTangent, this._keys [i].outTangent);
//			SetKeyFrameHelper (ref _curveY, i, this._keys [i].time, this._keys [i].valueY, this._keys [i].inTangent, this._keys [i].outTangent);
//			SetKeyFrameHelper (ref _curveZ, i, this._keys [i].time, this._keys [i].valueZ, this._keys [i].inTangent, this._keys [i].outTangent);
//		}
	}

	private void SetKeyFrameHelper (ref AnimationCurve _keyFrame, int i, float _time, float _value, float _inTangent, float _outTangent)
	{
		_keyFrame.keys [i] = new Keyframe (_time, _value, _inTangent, _outTangent);
	}

	public int length {
		get { return this._keys.Length; }
	}

	public Vector3 Evaluate (float time)
	{
		return new Vector3 (this._curveX.Evaluate (time), this._curveY.Evaluate (time), this._curveZ.Evaluate (time));
	}

	public float EvaluateX (float time)
	{
		return this._curveX.Evaluate (time);
	}

	public float EvaluateY (float time)
	{
		return this._curveY.Evaluate (time);
	}

	public float EvaluateZ (float time)
	{
		return this._curveZ.Evaluate (time);
	}

	SplineKeyframe[] chainKeys;

	/// <summary>
	/// Init the spline.
	/// </summary>
	public void InitSpline (Transform[] nodes)
	{
		int nodesLength = nodes.Length + 1;
		//InitKey (new SplineKeyframe[nodesLength]);
		chainKeys = new SplineKeyframe[nodesLength];
	}

	public float CreateSpline (Transform[] nodes, bool drawLine, float drawDistance)
	{
		int nodesLength = nodes.Length + 1;

		float curveLength = 0.0f;
		
		chainKeys [0] = new SplineKeyframe (0, nodes [0].position);
		
		
		
		for (int i = 1; i < nodesLength - 1; i++) {
			curveLength += Vector3.Distance (nodes [i].position, nodes [i - 1].position);
			chainKeys [i] = new SplineKeyframe (curveLength, nodes [i].position);
		}
		
		curveLength += Vector3.Distance (nodes [0].position, nodes [nodesLength - 2].position);
		chainKeys [nodesLength - 1] = new SplineKeyframe (curveLength, nodes [0].position);

		this.keys = chainKeys;

		this.Smooth ();
		
		if (drawLine) {
			for (float d = 0; d < curveLength; d += drawDistance) {
				Debug.DrawLine (this.Evaluate (d), this.Evaluate (d + drawDistance), Color.red);
			}
		}
		return curveLength;
	}

	public void AnimateTracks (Transform[] trackPoints, Transform[] tracks, float tracksDistance, bool keepDistance, int tracksOrient, float speed, float curveLength)
	{
		float step = 0.0f;
		int tracksCount = tracks.Length;
		int trackPointsCount = trackPoints.Length;
		
		for (int i = 0; i < tracksCount; i++) {
			Transform track = tracks [i];
			float displacement = step + tracksDisplacement;
			Vector3 tangentsVector = this.GetTangentsVector (displacement, 0.1f * tracksDistance);
			Vector3 trackPosition = track.position = this.Evaluate (displacement);
			Transform parent = track.parent;
			Vector3 delta = trackPosition - parent.position;
			Vector3 word = -tracksOrient * (delta).normalized;
			track.LookAt (trackPosition + tracksDistance * tangentsVector, word);	
			step += tracksDistance;			
		}
		
		tracksDisplacement += speed * Time.deltaTime;
		
		if (Mathf.Abs (tracksDisplacement) > curveLength)
			tracksDisplacement = 0.0f;
	}
}

public class SplineKeyframe
{
	private float _time;
	private float _valueX;
	private float _valueY;
	private float _valueZ;
	private Vector3 _valueVector;
	private float _inTangent;
	private float _outTangent;
	private  int _tangentMode;

	public float time {
		get { return this._time; }
		set { this._time = value; }
	}

	public float valueX {
		get { return this._valueX; }
		set { this._valueX = value; }
	}

	public float valueY {
		get { return this._valueY; }
		set { this._valueY = value; }
	}

	public float valueZ {
		get { return this._valueZ; }
		set { this._valueZ = value; }
	}

	public Vector3 valueVector {
		get { return this._valueVector; }
		set { this._valueVector = value; }
	}

	public float inTangent {
		get { return this._inTangent; }
		set { this._inTangent = value; }
	}

	public float outTangent {
		get { return this._outTangent; }
		set { this._outTangent = value; }
	}

	public int tangentMode {
		get { return this._tangentMode; }
		set { this._tangentMode = value; }
	}

	public SplineKeyframe (float time, Vector3 valueVector)
	{
		this._time = time;
		this._valueVector = valueVector;
		this._valueX = valueVector.x;
		this._valueY = valueVector.y;
		this._valueZ = valueVector.z;
		this._inTangent = 0;
		this._outTangent = 0;
	}

}

