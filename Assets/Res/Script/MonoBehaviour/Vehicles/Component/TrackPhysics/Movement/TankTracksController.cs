using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class TankTracksController : MonoBehaviour {
    public GameObject wheelCollider;
    public Transform trackPrefab;

    private WheelCollider colliderFromPrefab;



    public bool autoCOM = true;
    public Transform COM;



    public WheelsAxisSettings wheelsAndBonesAxisSettings;


    public float PushSpeed = 1, BackSpeed = 1;

    public TrackTextureDirectionSettings trackTextireAnimationSettings;
    public GameObject leftTrack;

    public Transform[] leftTrackUpperWheels;
    public Transform[] leftTrackWheels;

    public Transform[] leftTrackBones;

    public GameObject rightTrack;

    public Transform[] rightTrackUpperWheels;
    public Transform[] rightTrackWheels;
    public Transform[] rightTrackBones;

    public VAconfig accelerationConfiguration;


    [System.Serializable]
    public class VAconfig {
        //Dynamics that affect the acceleration and max speed
        //        Motor Torque
        //        |
        //        |
        //        |
        //        |_________________speed km/ph
        public AnimationCurve acceleration = AnimationCurve.Linear(0.0f, 750.0f, 80.0f, 0.0f);

        //Dynamics that affect the brake force when vertical axis is not active
        //        Brake Torque
        //        |
        //        |
        //        |
        //        |_________________speed km/ph
        public AnimationCurve brake = AnimationCurve.Linear(0.0f, 1000.0f, 80.0f, 1100.0f);

    }

    public HAconfig rotationOnStayConfiguration;

    public HAconfig rotationOnAccelerationConfiguration;


    [System.Serializable]
    public class HAconfig {
        //Dynamics that affect the rotate speed
        //        Rotate Vector Y coord
        //        |
        //        |
        //        |
        //        |_________________speed km/ph
        public AnimationCurve rotateSpeed = AnimationCurve.Linear(0.0f, 6.5f, 80.0f, 0f);
        //4.5
        //Dynamics that affect the brake when rotate
        //        Brake Torque
        //        |
        //        |
        //        |
        //        |_________________speed km/ph
        public AnimationCurve brake = AnimationCurve.Linear(0.0f, 0.0f, 80.0f, 1000.0f);
    }

    //Dynamics that affect the rotate damper
    //        Rotate Vector Z coord
    //        |
    //        |
    //        |
    //        |_________________speed km/ph
    public AnimationCurve rotationDamper = AnimationCurve.Linear(0.0f, 0.0f, 80.0f, 20.0f);

    public AnimationCurve rotationByRotateDamper = AnimationCurve.Linear(0, 0, 0, 0);

    public float sidewaysFrictionExtremumFactor = 2.2f;
    public float sidewaysFrictionAsymptoteFactor = 2.2f;

    public Transform tankTransform;

    public enum Axis {
        X,
        Y,
        Z,
    };

    public enum TexAxis {
        X,
        Y,
    };


    [System.Serializable]
    public class WheelsAxisSettings {
        public Axis wheelsPositionAxis = Axis.Y;
        public bool inverseWheelsPosition = false;

        public Axis bonesPositionAxis = Axis.Y;
        public bool inverseBonesPosition = false;

        public Axis wheelsRotationAxis = Axis.X;
        public bool inverseWheelsRotation = false;





        private int WheelRotationAxisPointer = 0;

        public int WRAxisPointer {
            get { return WheelRotationAxisPointer; }
            set { WheelRotationAxisPointer = value; }
        }

        private int wheelsPositionAxisPointer = 1;

        public int WPAxisPointer {
            get { return wheelsPositionAxisPointer; }
            set { wheelsPositionAxisPointer = value; }
        }

        private int bonesPositionAxisPointer = 1;

        public int BPAxisPointer {
            get { return bonesPositionAxisPointer; }
            set { bonesPositionAxisPointer = value; }
        }


        public static int SwitchAxis(Axis axis) {
            int pointer = 0;
            switch (axis) {
                case Axis.X:
                    pointer = 0;
                    break;

                case Axis.Y:
                    pointer = 1;
                    break;


                case Axis.Z:
                    pointer = 2;
                    break;
            }

            return pointer;

        }
    }


    [System.Serializable]
    public class TrackTextureDirectionSettings {
        public TexAxis trackTextureDirection = TexAxis.Y;
        public bool inverseTextureDirection = false;

        private int trackTextureAxisPointer = 2;

        public int TTAxisPointer {
            get { return trackTextureAxisPointer; }
            set { trackTextureAxisPointer = value; }
        }

        public static int SwitchAxis(TexAxis axis) {
            int pointer = 0;
            switch (axis) {
                case TexAxis.X:
                    pointer = 0;
                    break;

                case TexAxis.Y:
                    pointer = 1;
                    break;

            }

            return pointer;

        }

    }


    public class WheelData {
        public Transform wheelTransform;
        public Vector3 wheelStartPos;
        //public float rotation = 0.0f;
        public Vector3 wheelRotationAngles;
    }

    public class WheelDataExt : WheelData {
        public WheelCollider col;
    }


    public WheelDataExt[] leftTrackWheelData;
    public WheelDataExt[] rightTrackWheelData;


    public WheelData[] leftTrackUpperWD;
    public WheelData[] rightTrackUpperWD;



    protected Vector2 leftTrackTextureOffset = Vector2.zero;
    protected Vector2 rightTrackTextureOffset = Vector2.zero;

    public float AdvanceTrackOffSet = 0;

    public bool OnlyReceiveControlActions = false;

    public float CurrentSpeed {
        get {
            if (GetComponent<Rigidbody>()) {
                return Vector3.Dot(transform.forward, GetComponent<Rigidbody>().velocity) * 3.6f;
            }
            else
                return 0;
        }
    }


    [HideInInspector]
    public float leftTrackMiddleRPM = 0.0f;
    [HideInInspector]
    public float rightTrackMiddleRPM = 0.0f;

    [HideInInspector]
    public float leftTrackSpeed = 0.0f, rightTrackSpeed = 0.0f;

    public float LeftTrackRPM = 0, RightTrackRPM = 0;

    private int wheelsCount = 0;


    [HideInInspector]
    public float AngularVelocity;

    public float steerG = 0.0f;

    public float accelG = 0.0f;

    public bool isBrakeing = false;
    //Input
    public bool isBrakeingAble = false;
    //Control


    public bool Visible = true;
    public bool TrackVisible = true;

    public bool isBot = false;

    private WheelSkidmarks leftWheelSkidmarks, rightWheelSkidmarks;

    private bool isUsingWheelSkidmarks = false;

    public Rigidbody rigidBody;

    public float maxAngularVelocity = 35;

    public int MaxSpeed = 100, MinSpeed = -20;

    private float t = 0;


    public void SetWheels(bool usePhysicModel, bool Client, bool _isUsingWheelSkidmarks) {
        colliderFromPrefab = wheelCollider.GetComponent<WheelCollider>();

        wheelsCount = leftTrackWheels.Length + rightTrackWheels.Length;

        leftTrackWheelData = new WheelDataExt[leftTrackWheels.Length];
        rightTrackWheelData = new WheelDataExt[rightTrackWheels.Length];

        leftTrackUpperWD = new WheelData[leftTrackUpperWheels.Length];
        rightTrackUpperWD = new WheelData[rightTrackUpperWheels.Length];

        if (Client) {
            for (int i = 0; i < leftTrackWheels.Length; i++) {
                if (i < 10)
                    leftTrackWheelData[i] = SetupWheels(leftTrackWheels[i]);
            }

            for (int i = 0; i < rightTrackWheels.Length; i++) {
                if (i < 10)
                    rightTrackWheelData[i] = SetupWheels(rightTrackWheels[i]);
            }
            for (int i = 0; i < rightTrackUpperWheels.Length; i++) {
                rightTrackUpperWD[i] = SetupUpperWheels(rightTrackUpperWheels[i]);
            }
            for (int i = 0; i < leftTrackUpperWheels.Length; i++) {
                leftTrackUpperWD[i] = SetupUpperWheels(leftTrackUpperWheels[i]);
            }
        }
        Vector3 offset = transform.position;
        offset.z += 0.01f;
        transform.position = offset;
        SetupAxis();

        if (_isUsingWheelSkidmarks) {
            GameObject LeftTailObj = new GameObject("Track");
            LeftTailObj.transform.SetParent(leftTrackWheels[leftTrackWheels.Length - 1].transform);
            LeftTailObj.transform.localPosition = Vector3.zero;
            LeftTailObj.transform.position -= new Vector3(0, wheelCollider.GetComponent<WheelCollider>().radius, 0);
            LeftTailObj.transform.SetParent(LeftTailObj.transform.parent.parent);
            leftWheelSkidmarks = LeftTailObj.AddComponent<WheelSkidmarks>();

            GameObject RightTailObj = new GameObject("Track");
            RightTailObj.transform.SetParent(rightTrackWheels[rightTrackWheels.Length - 1].transform);
            RightTailObj.transform.localPosition = Vector3.zero;
            RightTailObj.transform.position -= new Vector3(0, wheelCollider.GetComponent<WheelCollider>().radius, 0);
            RightTailObj.transform.SetParent(RightTailObj.transform.parent.parent);
            rightWheelSkidmarks = RightTailObj.AddComponent<WheelSkidmarks>();

            isUsingWheelSkidmarks = true;
        }
    }

 


    public virtual void Start() {
        if (!autoCOM)
            GetComponent<Rigidbody>().centerOfMass = COM.localPosition;

        rotationByRotateDamper = AnimationCurve.Linear(0, maxAngularVelocity * 0.2f, MaxSpeed, maxAngularVelocity * 0.15f);
        rotationByRotateDamper.preWrapMode = WrapMode.Clamp;
        rigidBody = GetComponent<Rigidbody>();
    }

    public virtual void FixedUpdate() {

    }


    private void SetupAxis() {
        wheelsAndBonesAxisSettings.WRAxisPointer =
            WheelsAxisSettings.SwitchAxis(wheelsAndBonesAxisSettings.wheelsRotationAxis);

        wheelsAndBonesAxisSettings.WPAxisPointer =
            WheelsAxisSettings.SwitchAxis(wheelsAndBonesAxisSettings.wheelsPositionAxis);

        wheelsAndBonesAxisSettings.BPAxisPointer =
            WheelsAxisSettings.SwitchAxis(wheelsAndBonesAxisSettings.bonesPositionAxis);

        trackTextireAnimationSettings = new TrackTextureDirectionSettings();

        trackTextireAnimationSettings.TTAxisPointer =
            TrackTextureDirectionSettings.SwitchAxis(trackTextireAnimationSettings.trackTextureDirection);

    }

    public WheelDataExt SetupWheels(Transform wheel) {
        WheelDataExt result = new WheelDataExt();
        GameObject go = new GameObject(wheel.name);
        go.transform.parent = transform;
        go.transform.position = wheel.position;
        go.transform.localRotation = Quaternion.Euler(0, wheel.localRotation.y, 0);

        WheelCollider col = (WheelCollider)go.AddComponent(typeof(WheelCollider));
        col.mass = colliderFromPrefab.mass;
        col.center = colliderFromPrefab.center;
        col.radius = colliderFromPrefab.radius;
        col.suspensionDistance = colliderFromPrefab.suspensionDistance;
        col.suspensionSpring = colliderFromPrefab.suspensionSpring;
        col.forwardFriction = colliderFromPrefab.forwardFriction;
        col.sidewaysFriction = colliderFromPrefab.sidewaysFriction;
        col.forceAppPointDistance = colliderFromPrefab.forceAppPointDistance;

        result.wheelTransform = wheel;
        result.col = col;
        result.wheelStartPos = wheel.transform.localPosition;
        result.wheelRotationAngles = wheel.localEulerAngles;
        return result;
    }

    private WheelData SetupUpperWheels(Transform wheel) {
        WheelData result = new WheelData();

        result.wheelTransform = wheel;
        result.wheelStartPos = wheel.transform.localPosition;
        result.wheelRotationAngles = wheel.localEulerAngles;

        return result;

    }

    private Vector3 rotationVector = Vector3.zero;
    float steerGByAngularVelocity = 0;

    public float WheelDamper = 100;

    public void UpdateTrackByAngularVelocity(float accelG, float steerG, bool UsePhysic) {

        if (leftTrackWheelData == null)
            return;

        WheelDamper = 100;

        float DamperRate = 100 / (leftTrackWheelData.Length * 2);

        for (int i = 0; i < leftTrackWheelData.Length; i++) {
            if (!leftTrackWheelData[i].col.isGrounded) {
                WheelDamper -= DamperRate;
            }
            if (!rightTrackWheelData[i].col.isGrounded) {
                WheelDamper -= DamperRate;
            }
        }

        WheelDamper /= 100;



        if (isBrakeingAble) {
            if (rigidBody.angularVelocity.magnitude / Mathf.Deg2Rad < maxAngularVelocity * 2) {
                rigidBody.AddRelativeTorque((Vector3.up * steerG) * rotationByRotateDamper.Evaluate(CurrentSpeed) * WheelDamper, ForceMode.Acceleration);
            }
        }
        else {
            if (rigidBody.angularVelocity.magnitude / Mathf.Deg2Rad < maxAngularVelocity) {
                rigidBody.AddRelativeTorque((Vector3.up * steerG) * rotationByRotateDamper.Evaluate(CurrentSpeed) * WheelDamper, ForceMode.Acceleration);
            }
        }

    }

    #region Rpm转化为KMPH

    public float RPMtoKMPH(float radius, float rpm) {
        float length = 2.0f * Mathf.PI * radius;

        float result = rpm * length * 60.0f / 1000.0f; //km/ph;

        return result;
    }

    #endregion




    private float CalculateSmoothRpm(WheelDataExt[] w) {
        float rpm = 0.0f;
        if (w == null)
            return rpm;

        List<int> grWheelsInd = new List<int>();


        for (int i = 0; i < w.Length; i++) {
            if (w[i].col.isGrounded) {
                grWheelsInd.Add(i);
            }
        }

        if (grWheelsInd.Count == 0) {
            for (int i = 0; i < w.Length; i++) {
                rpm += w[i].col.rpm;
            }
            rpm /= w.Length;

        }
        else {
            for (int i = 0; i < grWheelsInd.Count; i++) {
                rpm += w[grWheelsInd[i]].col.rpm;
            }
            rpm /= grWheelsInd.Count;
        }

        return rpm;
    }


    public class RFRD {

        public float rotationForce = 0.0f;
        public float rotationDamper = 0.0f;

        public RFRD() {
            rotationForce = 0.0f;
            rotationDamper = 0.0f;
        }

        public RFRD(float rf, float rd) {
            rotationForce = rf;
            rotationDamper = rd;
        }

        public static RFRD operator +(RFRD m1, RFRD m2) {
            return new RFRD(m1.rotationForce + m2.rotationForce, m1.rotationDamper + m2.rotationDamper);

        }



    }


    private RFRD CalculateMotorForce(WheelCollider col, float accel, float steer) {

        WheelFrictionCurve fc = colliderFromPrefab.sidewaysFriction;

        RFRD rfrd = new RFRD();

        float wheelSpeed = Mathf.Abs(RPMtoKMPH(col.radius, col.rpm));

        float motorTorque = 0.0f;
        float brakeTorque = 0.0f;

        if (accel == 0 && steer == 0) {
            brakeTorque = accelerationConfiguration.brake.Evaluate(wheelSpeed);
            motorTorque = 0.0f;

            rfrd.rotationForce = 0.0f;
            rfrd.rotationDamper = 0.0f;
        }
        else if (accel == 0.0f) {

            if (!col.isGrounded) {
                motorTorque = steer * accelerationConfiguration.acceleration.Evaluate(wheelSpeed);

                rfrd.rotationForce = 0.0f;
                rfrd.rotationDamper = 0.0f;
            }
            else {

                rfrd.rotationForce = rotationOnStayConfiguration.rotateSpeed.Evaluate(wheelSpeed) / wheelsCount;
                rfrd.rotationDamper = rotationDamper.Evaluate(wheelSpeed) / wheelsCount;

                motorTorque = 0.0f;

                fc.asymptoteValue *= sidewaysFrictionAsymptoteFactor;
                fc.extremumValue *= sidewaysFrictionExtremumFactor;
            }
            //brakeTorque = accelerationConfiguration.brake.Evaluate (wheelSpeed);

            brakeTorque = rotationOnStayConfiguration.brake.Evaluate(wheelSpeed);

        }
        else {

            if (steer != 0.0f)
                if (!col.isGrounded) {

                    rfrd.rotationForce = 0.0f;
                    rfrd.rotationDamper = 0.0f;
                }
                else {

                    rfrd.rotationForce = rotationOnAccelerationConfiguration.rotateSpeed.Evaluate(wheelSpeed) / wheelsCount;
                    rfrd.rotationDamper = rotationDamper.Evaluate(wheelSpeed) / wheelsCount;

                    fc.asymptoteValue *= sidewaysFrictionAsymptoteFactor;
                    fc.extremumValue *= sidewaysFrictionExtremumFactor;
                }

            motorTorque = accel * accelerationConfiguration.acceleration.Evaluate(wheelSpeed);

            if (col.rpm > 0 && accel < 0) {
                brakeTorque = accelerationConfiguration.brake.Evaluate(wheelSpeed);
            }
            else if (col.rpm < 0 && accel > 0) {
                brakeTorque = accelerationConfiguration.brake.Evaluate(wheelSpeed);
            }
            else {
                if (steer != 0.0f)
                    brakeTorque = rotationOnAccelerationConfiguration.brake.Evaluate(wheelSpeed);
                else
                    brakeTorque = 0.0f;
            }



        }

        //col.suspensionSpring = js;

        col.motorTorque = motorTorque;
        col.brakeTorque = brakeTorque;


        col.sidewaysFriction = fc;

        return rfrd;


    }

    public void UpdateWheels(float accel, float steer) {
        if (leftTrackWheelData == null && rightTrackWheelData == null)
            return;
        
        if (steer != 0 && !isBrakeingAble) {
            if ((accel != 1 || accel != -1) && (rigidBody.velocity.magnitude * 3.6f < 5)) {

                if (rigidBody.angularVelocity.magnitude / Mathf.Deg2Rad < 10) {
                    if (accel >= 0) {
                        accel = PushSpeed;
                    }
                    else {
                        accel = -PushSpeed;
                    }
                    steer = 0;
                }
                else if (rigidBody.angularVelocity.magnitude / Mathf.Deg2Rad < maxAngularVelocity - 10) {
                    if (accel >= 0) {
                        accel = PushSpeed;
                    }
                    else {
                        accel = -PushSpeed;
                    }
                }

            }
        }

        leftTrackSpeed = RPMtoKMPH(leftTrackWheelData[0].col.radius, leftTrackMiddleRPM) + steerGByAngularVelocity * 10;
        rightTrackSpeed = RPMtoKMPH(rightTrackWheelData[0].col.radius, rightTrackMiddleRPM) - steerGByAngularVelocity * 10;

        RFRD rfrd = new RFRD();
        LeftTrackRPM = TrackRPM(leftTrackWheelData);
        RightTrackRPM = TrackRPM(leftTrackWheelData);

        t += Time.fixedDeltaTime;

        rfrd = TrackUpdate(accel, steer, leftTrackWheelData, leftTrack, 1, ref leftTrackTextureOffset, leftTrackUpperWD,ref leftTrackMiddleRPM);
        rfrd += TrackUpdate(accel, -steer, rightTrackWheelData, rightTrack, -1, ref rightTrackTextureOffset, rightTrackUpperWD, ref rightTrackMiddleRPM);

        rotationVector.y = steer * rfrd.rotationForce;
        rotationVector.z = -steer * rfrd.rotationDamper;

        //
        //Debug.Log(rigidbody.angularVelocity);
    }


    private float TrackRPM(WheelDataExt[] WD) {
        return CalculateSmoothRpm(WD);
    }


    private RFRD TrackUpdate(float accel, float steer, WheelDataExt[] WD, GameObject track, int RotateDir, ref Vector2 trackTextureOffset, WheelData[] upperWheels, ref float middleRPM) {
        RFRD rfrd = new RFRD();
        float delta = Time.fixedDeltaTime;

        float trackRpm = 0.0f;
        trackRpm = CalculateSmoothRpm(WD);
        trackRpm += (AngularVelocity * RotateDir * 0.25f) / Time.deltaTime;
        middleRPM = trackRpm;

        float RPMtoDeg = delta * trackRpm * 360.0f / 60.0f;

        if (wheelsAndBonesAxisSettings.inverseWheelsRotation)
            RPMtoDeg *= -1.0f;

        if (isUsingWheelSkidmarks) {
            if (RotateDir > 0) {
                leftWheelSkidmarks.gameObject.SetActive(WD[0].col.isGrounded);
                if (!WD[0].col.isGrounded) {
                    leftWheelSkidmarks.lastSkidmark = -1;
                }
            }
            else {
                rightWheelSkidmarks.gameObject.SetActive(WD[0].col.isGrounded);
                if (!WD[0].col.isGrounded) {
                    rightWheelSkidmarks.lastSkidmark = -1;
                }
            }
        }

        for (int i = 0; i < WD.Length; i++) {
            WD[i].wheelRotationAngles[wheelsAndBonesAxisSettings.WRAxisPointer] = Mathf.Repeat(WD[i].wheelRotationAngles[wheelsAndBonesAxisSettings.WRAxisPointer] + RPMtoDeg, 360.0f);
            WD[i].wheelTransform.localEulerAngles = WD[i].wheelRotationAngles;
            rfrd += CalculateMotorForce(WD[i].col, accel, steer);
        }
        for (int i = 0; i < upperWheels.Length; i++) {
            upperWheels[i].wheelRotationAngles[wheelsAndBonesAxisSettings.WRAxisPointer] = Mathf.Repeat(upperWheels[i].wheelRotationAngles[wheelsAndBonesAxisSettings.WRAxisPointer] + RPMtoDeg, 360.0f);
            upperWheels[i].wheelTransform.localEulerAngles = upperWheels[i].wheelRotationAngles;
        }
       
        if(track!=null){
            trackTextureOffset += new Vector2(0, RPMtoDeg * delta);
            track.GetComponent<MeshRenderer>().material.mainTextureOffset = trackTextureOffset;
        }
        return rfrd;

    }
}
