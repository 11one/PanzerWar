using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;



public class EnemyAiState : BasePlayerState
{
    private enum State {
        Idle,
        Patorl,
        Sniper,
        Warrior
    }

    private enum TargetType {
        Vechcile,
        PatorlPoint
    }
    public static Dictionary<GameObject, List<EnemyAiState>> PatorlPoints = new Dictionary<GameObject, List<EnemyAiState>>();

    public int MAX_FIND_DISTANT = 500;

	public PunTeams.Team MyTeam = PunTeams.Team.none;

	[HideInInspector]
	public GameObject TankScript;


	private GameObject Target;

	private bool SendDeadMesssageToTankFire = false, EnableMove = true;

    private NavMeshAgent navmeshAgent;

    private PlayerTracksController playerTracksController;

	private bool AllowRotate = true, AllowForward = true;

	private float angle, Side, isFornt;

	private bool GodMode = true;

	private State state = State.Idle;

	private TankFire tankFire;


	private bool ControlByLocal = false;

	private int MaxHealth=0;

	private bool isTurretLess = false;


	private GameObject ViewPoint;


	GameObject FindRandomPatorlPoint ()
	{
		List<GameObject> rootPatrolPoint = new List<GameObject>();
		foreach (GameObject patorlPoint in  GameObject.FindGameObjectsWithTag ("PatorlPoint")) {
			if (patorlPoint.transform.root == patorlPoint.transform) {
				rootPatrolPoint.Add (patorlPoint);
			}
		}
		return rootPatrolPoint [Random.Range (0,rootPatrolPoint.Count)];
	}

	GameObject FindClosestEnemy ()
	{
		List<Identity> Identities = new List<Identity> ();
		Identity Closest = null;
		foreach (Identity id in GameObject.FindObjectsOfType<Identity>()) {
			if (!GodMode)
				if (Vector3.Angle (transform.forward, id.transform.position - transform.position) > 165)
					continue;
			
			Vector3 Dir = id.transform.position - transform.position;

			RaycastHit[] raycastHits = Physics.RaycastAll (ViewPoint.transform.position, Dir, Dir.magnitude, 1 << LayerMask.NameToLayer ("HitBox") | 1 << LayerMask.NameToLayer ("Building") | 1 << LayerMask.NameToLayer ("Terrian"));
			bool CantSee = false;

			if (raycastHits.Length > 0) {
				foreach (RaycastHit raycastHit in raycastHits) {
					if (raycastHit.collider.tag != "HitBox" && raycastHit.collider.tag != "Default") {
						CantSee = true;
					}
				}
			}

			if (id.identity != GetComponent<Identity> ().identity && !CantSee) {
				Identities.Add (id);
			}
		}
		if (Identities.Count == 0) {
			return null;
		}
		float distance = Mathf.Infinity;
		Vector3 position = transform.position;
		foreach (Identity Identity in Identities) {
			Vector3 diff = Identity.transform.position - position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance) {
				Closest = Identity;
				distance = curDistance;
			}
		}
		return Closest.gameObject;
	}

	void Awake ()
	{
//		myPhotonView.ObservedComponents.Add(this);
//		myPhotonView.synchronization = ViewSynchronization.Unreliable;
	}
	void Start ()
	{
		GameObject NavMesh = new GameObject ("NavMesh");
		NavMesh.transform.position = transform.position;
		navmeshAgent = NavMesh.AddComponent<NavMeshAgent> ();
        navmeshAgent.Warp(transform.position);

		//nav =new GameObject("navAgent",typeof(NavMeshAgent)).GetComponent<NavMeshAgent>();
		//nav.transform.position = transform.position;

        if (GameDataManager.GetCurrentDifficulty () == Difficulty.Easy) {
			MAX_FIND_DISTANT = 150;
		}
		MaxHealth = Health;

		//EAS.Health = Health;
		//EAS.MyTeam = Team;
		//EAS.TankName = TankName;
		//EAS.TankScript = TankScript;
		TankScript.GetComponent<TankFire>().IsOnLine = true;
		TankScript.GetComponent<TankFire> ().SetAiMode ();
		//TankScript.GetComponent<MouseTurret> ().AITurret = true;
		transform.parent.GetComponentInChildren<TankFire> ().InitSystem ();

		tankFire = TankScript.GetComponent<TankFire> ();
		tankFire.UseGravity = false;

		playerTracksController = GetComponent<PlayerTracksController> ();
		playerTracksController.enableUserInput = false;
		playerTracksController.isBot = true;

  //      if (GameDataManager.GetVehicleType (TankName) == VehicleType.TD) {
		//	isTurretLess = true;
		//}

		Identity MyIdentity = GetComponent<Identity> ();
        if (GameDataManager.OfflineMode) {
			ViewPoint = CreateViewPoint ();
			ViewPoint.transform.SetParent (transform);

			Invoke ("SwtichToNormalMode", 5);

			navmeshAgent.radius = 4.5f;
			navmeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;

			ControlByLocal = true;

			InvokeRepeating ("AIState", 0, 1);

            if (GameDataManager.OfflineMode) {
                MyIdentity.Init (transform.root.GetComponent<TankInitSystem> ().ownerTeam, InstanceNetType.GameNetWorkBotClient);
				playerTracksController.SetWheels (true, true, true);
			} else {
				playerTracksController.SetWheels (true, false, false);
			}
		} else {
			ControlByLocal = false;
			playerTracksController.SetWheels (false, false, false);
		}
		gameObject.SendMessage ("GetOwnerInfo");
		StartCoroutine (AttackPlayer ());
	}
	Vector3 TargetPosition ()
	{
		if (Target != null) {
			return Target.transform.position;
		} else {
			return Vector3.zero;
		}
	}

	void SetTargetPosition (Vector3 _v3)
	{
		if (TankScript.GetComponent<TurretController> ().target == null) {
			TankScript.GetComponent<TurretController> ().target = new GameObject ().transform;
		}
		TankScript.GetComponent<TurretController> ().target.transform.position = _v3;
	}
	void Update ()
	{
		if (ControlByLocal) {
			navmeshAgent.transform.transform.position = this.transform.position;
			Move ();
		}
	}

	void Caculate ()
	{
		if (Target == null) {
			state = State.Idle;
			AIState ();
			return;
		}
		
		Vector3 _Target;

		if (InSniperStateInWarriorState) {
			_Target = Target.transform.position;
		} else {
			_Target = navmeshAgent.steeringTarget;
		}
        Vector3 Dir = navmeshAgent.desiredVelocity;

		Vector3 LocalDir = transform.InverseTransformDirection (Dir);

		Debug.DrawRay (_Target, Vector3.up * 500);

		Side = LocalDir.x;          

		angle = Vector3.Angle (new Vector3 (Dir.x, 0, Dir.z), new Vector3 (transform.forward.x, 0, transform.forward.z));

		isFornt = LocalDir.z;

		if (isFornt < 0 || angle >= 5) {
			if (Side < 0) {
				Side = -1;
			} else {
				Side = 1;
			}
		} else {
			Side = 0;
		}
	}

	void Move ()
	{
		if (!EnableMove)
			return;
		
		
		Caculate ();
		playerTracksController.steerG = Side;

		if (AllowForward) {
			if (angle < 5) {
				playerTracksController.accelG = playerTracksController.PushSpeed;
			} else {
				playerTracksController.accelG = 0;
			}
		} else {
			playerTracksController.accelG = 0;
            if (GameDataManager.GetCurrentDifficulty () == Difficulty.Easy&&!isTurretLess) {
				playerTracksController.steerG = 0;
			}
		}
	}


	public void OnDeadActionCalled ()
	{
		Destroy (GetComponent<Identity> ());

		StopAllCoroutines ();

        if (GameDataManager.OfflineMode) {
			playerTracksController.UpdateWheels (0, 0);
			playerTracksController.enabled = false;
		}



		
		foreach (HitBox temp in transform.root.GetComponentsInChildren<HitBox>()) {
			Destroy (temp);
		}
		#if ClientCode

		#region 炮塔飞天
		TankScript.GetComponent<TurretController> ().Turret.gameObject.AddComponent<Rigidbody> ().AddForce (Vector3.up * 500);
		TankScript.GetComponent<TurretController> ().Turret.GetComponent<Rigidbody> ().drag = 0.3f;
		TankScript.GetComponent<TurretController> ().Turret.GetComponent<Rigidbody> ().mass = 1000;
		TankScript.GetComponent<TurretController> ().Turret.GetComponent<Rigidbody> ().useGravity = true;
		#endregion

		PoolManager.CreateObject ("DestroyEffect", transform.position, transform.eulerAngles);
		#endif
		foreach (HitBox _HitBox in transform.root.GetComponentsInChildren<HitBox>()){
			if(_HitBox){
				_HitBox.enabled =false;
			}
		}

		//TankScript.GetComponent<MouseTurret> ().target = null;

		if (ControlByLocal) {
			#region PTC关闭
			playerTracksController.accelG = 0;
			playerTracksController.steerG = 0;
			playerTracksController.UpdateWheels(0, 0);
			playerTracksController.enabled = false;
			#endregion

			#region Nav
			navmeshAgent.enabled = false;
			Destroy(navmeshAgent.gameObject);
			#endregion
		}

		#region TankScript
		TankScript.GetComponent<TankFire>().NowTime =0;
		TankScript.GetComponent<TankFire>().enabled =false;
		TankScript.GetComponent<TurretController>().enabled =false;

		TankScript.SetActive(false);
		#endregion

        Destroy (transform.root.gameObject, 10f);

		this.enabled = false;
	}



	GameObject AttackPlayerTarget;
	List <HitBox> HitBoxList = new List<HitBox> ();

	IEnumerator AttackPlayer ()
	{
		while (true) {
			yield return new WaitForSeconds (1);
			#region 非本地控制 返回
			if (!ControlByLocal)
				yield break;
			#endregion

			if (Health <= 0) {
				yield break;
			}
			if (state != State.Warrior) {
				continue;
			}
			if (Target == null) {
				StartAttackingPlayer = false;

				state = State.Idle;
				AttackPlayerTarget = null;

				continue;
			}

			if (Vector3.Distance (transform.position, Target.transform.position)>MAX_FIND_DISTANT) {
				continue;
			}

			if (AttackPlayerTarget != Target) {
				AttackPlayerTarget = Target;
				HitBoxList = new List<HitBox> ();
				foreach (HitBox hitBox in Target.GetComponentsInChildren<HitBox>()) {
					HitBoxList.Add (hitBox);
				}
				for (int i = 0; i < HitBoxList.Count - 1; i++) { 
					for (int j = i + 1; j < HitBoxList.Count; j++) { 
						if (HitBoxList [i].transform.position.y < HitBoxList [j].transform.position.y) { 
							HitBox Temp = HitBoxList [i]; 
							HitBoxList [i] = HitBoxList [j]; 
							HitBoxList [j] = Temp; 
						} 
					} 
				}

			}
			RaycastHit hit;
			HitBox TempHitBox = null;
			for (int i = 0; i < HitBoxList.Count - 1; i++) { 
				if (Physics.Raycast (tankFire.FFPoint.transform.position, HitBoxList [i].CenterBound.transform.position - tankFire.FFPoint.position, out hit, 800, 1 << LayerMask.NameToLayer ("HitBox") | 1 << LayerMask.NameToLayer ("Building") | 1 << LayerMask.NameToLayer ("Terrian"))) {
					if (hit.collider.tag == "HitBox") {
						if (TempHitBox != null) {
							if (TempHitBox.CenterBound.transform.position.y < HitBoxList [i].CenterBound.transform.position.y)
								continue;
						} 

						TempHitBox = HitBoxList [i];
					}
				}
			}
			if (TempHitBox != null) {
				StartCoroutine (PredictPostion (TempHitBox.CenterBound.transform));
			} else {
				continue;
			}



            if (GameDataManager.GetCurrentDifficulty() == Difficulty.Easy) {
				yield return new WaitForSeconds (5);
            } else if (GameDataManager.GetCurrentDifficulty() == Difficulty.Normal) {
					yield return new WaitForSeconds (2.5f);
			}



			if (TempHitBox != null) {
				if (Vector3.Angle (tankFire.turretController.gun.forward, Target.transform.position - tankFire.turretController.gun.position) < 25) {
                    tankFire.FireActionToClient();
				}
				StopCoroutine (PredictPostion (TempHitBox.CenterBound.transform));
			}
//			if (tankFire.HasMahineGun) {
//				tankFire.machineGun.OnFire ();
//			}
//
			yield return new WaitForSeconds (2f);

//			if (tankFire.HasMahineGun) {
//				tankFire.machineGun.OnIdle ();
//			}
		}
	}
	public IEnumerator PredictPostion(Transform aimedMovingTarget){
		Vector3 PreviousAimedMovingTargetPostion = aimedMovingTarget.position;
		while (true) {
			if (aimedMovingTarget == null)
				yield break;
			
			if (Target.transform.root != aimedMovingTarget.root)
				yield break;
			
			Vector3 CurrentAimedMovingTargetPostion = aimedMovingTarget.transform.position;
			Vector3 OffSet = CurrentAimedMovingTargetPostion - PreviousAimedMovingTargetPostion;

			PreviousAimedMovingTargetPostion = CurrentAimedMovingTargetPostion;
			SetFireTarget (aimedMovingTarget.transform.position+OffSet*2 +  (aimedMovingTarget.transform.position - tankFire.turretController.gun.position) * 1000);
			//tankFire.mouseTurret.PostionOffSet = Vector3.zero;

			yield return new WaitForSeconds (0.5f);
		}
	}
	public override void ApplyHitBoxDamage(int Damage,float Degree,BaseFireSystem Owner,HitBox _hitBox){
		base.ApplyHitBoxDamage (Damage,Degree,Owner,_hitBox);

		if (GodMode) {
			_hitBox.BasePlayerStateGodModeCallBack ();
			return;
		}
		if (GetComponent<Identity> ().identity == Owner.transform.root.GetComponentInChildren<Identity> ().identity) {
			_hitBox.BasePlayerStateHitFriendCallBack ();
			return;
		} else {
			_hitBox.BasePlayerStateHittedCallBack (Damage,Health);
		}


		//if (GameNetwork.OfflineMode) {

		//} else {
		//	syncActionPhotonView.RPC ("ApplyDamageByOtherPlayer", PhotonTargets.OthersBuffered, Damage);
		//}

		Health -= Damage;


		OnAttacked (Owner.transform.root.GetComponentInChildren<Identity> ().gameObject);

		if (Health <= 0 && SendDeadMesssageToTankFire == false) {
			SendDeadMesssageToTankFire = true;
			Owner.KillTank (TankName);
			Owner.AddDamage (Damage, TankName);
		}
		if (SendDeadMesssageToTankFire == false) {
			Owner.AddDamage (Damage, TankName);
		}

		if (Health <= 0) {
			OnDeadActionCalled ();
		}
		#if ClientCode
		GetComponent<Identity>().UpdateHealth(Health);
		#endif
	}

	void AIState ()
	{
		if (Health <= 0)
			return;
		


		if (navmeshAgent.enabled == true && Target != null) {
			navmeshAgent.SetDestination (Target.transform.position);
		} 

		switch (state) {
			case State.Idle:
				PatorlNodes = new List<GameObject> ();
				AllowForward = true;
				AllowRotate = true;
				state = State.Patorl;
				break;
			case State.Patorl:
				Patorl ();
				ActionObserve ();
				break;
			case State.Sniper:
				break;
			case State.Warrior:
				if (Target == null || AttackPlayerTarget == null) {
					state = State.Idle;
				}

				Warrior ();
				break;
		}

	}

	List<GameObject> PatorlNodes = new List<GameObject> ();

	void Patorl ()
	{
		#region 非本地控制
		if (!ControlByLocal)
			return;
		#endregion
		AllowRotate = true;
		AllowForward = true;

		bool HasPatorl = false;
		if (Target != null) {
			if (PatorlNodes.Count > 0)
				HasPatorl = true;
		}
		#region 没有巡航点
		if (HasPatorl == false) {
			#region 初始化路径节点
			GameObject patorlPoint = FindRandomPatorlPoint ();

			Transform LastNode = patorlPoint.transform;
			while (LastNode.childCount != 0) {
				LastNode = LastNode.GetChild (0);
			}


			bool ForwardAdd = (Vector3.Distance (transform.position, LastNode.position) > Vector3.Distance (transform.position, patorlPoint.transform.GetChild (0).position));
			if (ForwardAdd) {
				Transform TempNode = patorlPoint.transform.GetChild (0);
				while (TempNode.childCount != 0) {
					PatorlNodes.Add (TempNode.gameObject);
					TempNode = TempNode.GetChild (0);
				}
			} else {
				Transform TempNode = LastNode;
				while (TempNode.parent != null) {
					PatorlNodes.Add (TempNode.gameObject);
					TempNode = TempNode.parent;
				}
			}
			#endregion
			SetTarget (PatorlNodes [0]);
		}
		#endregion
		#region 有巡航点
		if (HasPatorl == true) {
			Vector2 PatorlNodesV2 = new Vector2 (PatorlNodes [0].transform.position.x, PatorlNodes [0].transform.position.z);
			Vector2 transformNodesV2 = new Vector2 (navmeshAgent.transform.position.x, navmeshAgent.transform.position.z);

			if (Vector2.Distance (PatorlNodesV2, transformNodesV2) < 30) {
				PatorlNodes.RemoveAt (0);
				if (PatorlNodes.Count > 0) {
					SetTarget (PatorlNodes [0]);
				}
			}
		}
		#endregion
	}

	bool InSniperStateInWarriorState = false;
	bool StartAttackingPlayer = false;

	void Warrior ()
	{
		if (Target == null) {
			state = State.Idle;
			PatorlNodes = new List<GameObject> ();
			return;
		}

		


		InSniperStateInWarriorState = false; // 黑枪阶段

		if (navmeshAgent.nextPosition == navmeshAgent.destination) {
			InSniperStateInWarriorState = true;
		} 
		Vector3 Dir = Target.transform.position - transform.position;

		RaycastHit[] raycastHits = Physics.RaycastAll (ViewPoint.transform.position, Dir, Dir.magnitude, 1 << LayerMask.NameToLayer ("HitBox") | 1 << LayerMask.NameToLayer ("Building") | 1 << LayerMask.NameToLayer ("Terrian"));
		bool CantSee = false;

		if (raycastHits.Length > 0) {
			foreach (RaycastHit raycastHit in raycastHits) {
				if (raycastHit.collider.tag != "HitBox" && raycastHit.collider.tag != "Default") {
					CantSee = true;
				}
			}
		}
		if (Vector3.Angle (tankFire.turretController.Turret.forward, Target.transform.position - transform.position) > 8) {
			CantSee = true;
		}

		if (!CantSee) {
			InSniperStateInWarriorState = true;
		}

		if (Vector3.Distance (Target.transform.position, transform.position) < 15) {
			InSniperStateInWarriorState = true;
		}


			
		if (InSniperStateInWarriorState) {
			AllowRotate = true;
			AllowForward = false;
		} else {
			AllowRotate = true;
			AllowForward = true;
		}

	}

	void SetFireTarget (Vector3 target)
	{
		if (Health <= 0) {
			return;
		}
		if (TankScript.GetComponent<TurretController> ().target == null) {
			TankScript.GetComponent<TurretController> ().target = new GameObject ("EnemyAIMouseTarget").transform;
		}
		TankScript.GetComponent<TurretController> ().target.position = target;
	}

	#region 当受到攻击

	void OnAttacked (GameObject Attacker)
	{
		if (ControlByLocal) {
			if (Attacker == null) {//攻击者死亡
				return;
			}
			if (Health <= 0) {
				return;
			}

			if (state == State.Warrior) {
				if (Target.tag != "PatorlPoint")
					if ((Attacker.transform.position - transform.position).sqrMagnitude >= (Target.transform.position - transform.position).sqrMagnitude) {
						return;
					}
			}

			SetTarget (Attacker.transform.parent.GetComponentInChildren<Identity> ().gameObject);
			state = State.Warrior;
		}
	}

	#endregion

	void SwtichToNormalMode ()
	{
		GodMode = false;
	}

	void SetTarget (GameObject TargetObject)
	{
		if (ControlByLocal) {
			Target = TargetObject;
		}
	}

	void ActionObserve ()
	{
		GameObject MyTarget = FindClosestEnemy ();
		if (MyTarget == null)
			return;
		
		float distance = Vector3.Distance (MyTarget.transform.position, transform.position);
		if (distance < MAX_FIND_DISTANT) {
			OnAttacked (MyTarget);
		}
	}





	void GetOwnerInfo ()
	{
		GetComponent<Identity> ().SetOwnerInfo ("ID" + transform.root.GetComponentInParent<TankInitSystem> ().PlayerID + "[Bot]", uGUI_Localsize.GetContent (TankName), Health);
	}

	GameObject CreateViewPoint ()
	{
		HitBox[] TurretHitBoxes = null;
//		if (AccountDataManager.GetVehicleType (TankName) == VehicleType.TD) {
			TurretHitBoxes =  transform.Find ("MainHitBox").GetComponentsInChildren<HitBox> ();
		//} else {
		//	TurretHitBoxes = transform.Find ("TurretTransform/TurretHitBox").GetComponentsInChildren<HitBox> ();
		//}
		float Y = -Mathf.Infinity;
		HitBox TheTopHitBox = null;
		foreach (HitBox hitBox in TurretHitBoxes) {
			if (hitBox.transform.localPosition.y > Y) {
				TheTopHitBox = hitBox;
				Y = hitBox.transform.localPosition.y;
			}
		}
		GameObject _ViewPoint = new GameObject ("ViewPoint");
		if (TheTopHitBox != null) {
			_ViewPoint.transform.position = TheTopHitBox.transform.position;
		} else {
			_ViewPoint.transform.position = transform.position;
		}
		return _ViewPoint;
	}
}
