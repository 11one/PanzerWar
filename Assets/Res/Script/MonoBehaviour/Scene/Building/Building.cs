using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//多人模式下 onDestroyed 由 MasterBuildingManagerModule 监听 处理 。 MasterBuildingManagerModule 会让 MasterClient RaiseEvent 告知客户端 倒塌建筑的ID 与 倒塌方向
//离线模式 缺省
public class Building : SceneObjectWithSerializedID
{
	public System.Action<Building> onDestroyed;

	private GameObject MainModel;

	private GameObject DestroyedModel;

	private GameObject CollisionTrigger;


	[Header("损坏动画")]
	public Animator CollapseAnimation;
	[Header("损坏方式")]
	public BuildingDestroyType DestroyType;
	[Header("损坏粒子")]
	public BuildingDestroyEffectType DestroyEffectType = BuildingDestroyEffectType.None;
	[Header("房屋血量")]
	public int Health;

	public bool isMasterControl = false;

	public bool isMasterServerObject = false;


	[HideInInspector]
	public Vector3 DestroyedDir = new Vector3 ();

	private bool isDestroyed = false;


	void Start ()
	{
		MainModel = transform.GetChild (0).Find ("MainModel").gameObject;
		DestroyedModel = transform.GetChild (0).Find ("DestroyedModel").gameObject;
		CollisionTrigger = transform.GetChild (0).Find ("CollisionTrigger").gameObject;

		DestroyedModel.SetActive (false);

	}
		
	public void ApplyAmmoHit (int Damage)
	{
		if (!isMasterControl)
			return;

		if (isDestroyed)
			return;
		
		Health -= Damage;
		if (Health <= 0) {
			if (DestroyType == BuildingDestroyType.ByModelReplace) {
				ShowCollapseModel ();
			}
		}
	}

	private void OnTriggerEnter (Collider other)
	{
		if (!isMasterControl)
			return;
		
		if (isDestroyed)
			return;
		
		if (other.transform.root.GetComponent<BaseInitSystem> () == null)
			return;
		
		if (DestroyType == BuildingDestroyType.ByCollapseAnimation) {
			TankTracksController TTC = other.transform.root.GetComponentInChildren<TankTracksController> ();
			if (TTC != null) {
				Vector3 Dir = TTC.GetComponent<Rigidbody> ().velocity;
				float Velocity = TTC.CurrentSpeed;
				PlayCollapseAnimation (Dir,Velocity);
			}
		}
		if (DestroyType == BuildingDestroyType.ByModelReplace) {
			ShowCollapseModel ();
		}
	}
	//当BuildingDestroyType 为 使用 损坏模型时候触发  可受到 BuildingModule 的控制
	public void ShowCollapseModel ()
	{
		isDestroyed = true;

		MainModel.SetActive (false);
		DestroyedModel.SetActive (true);
		CollisionTrigger.SetActive (false);

		if (!isMasterServerObject) {
			ShowParticle ();
		}

		if(onDestroyed!=null)
			onDestroyed (this);
		
	}
	//当BuildingDestroyType 为 使用 动画时候触发  可受到 BuildingModule 的控制
	public void PlayCollapseAnimation (Vector3 Dir,float Velocity)
	{
		isDestroyed = true;


		DestroyedDir = Dir;

			
		if (!isMasterServerObject) {
			bool isPlayReverseAnimation = Vector3.Dot (Dir, transform.forward) < 0;

			if (isPlayReverseAnimation)
				Dir = -Dir;
			
			CollisionTrigger.SetActive (false);

			transform.rotation = Quaternion.LookRotation (Dir);

			CollapseAnimation.enabled = true;

			if (Velocity < 10) {
				if (isPlayReverseAnimation) {
					CollapseAnimation.Play ("ReverseNatureCollapse");
				} else {
					CollapseAnimation.Play ("NatureCollapse");
				}
			} else {
				if (isPlayReverseAnimation) {
					CollapseAnimation.Play ("ReverseFastCollapse");
				} else {
					CollapseAnimation.Play ("FastCollapse");
				}
			}

			ShowParticle ();

		} else {
			MainModel.SetActive (false);
			DestroyedModel.SetActive (true);
			CollisionTrigger.SetActive (false);
		}

		if(onDestroyed!=null)
			onDestroyed (this);


	}




	//显示倒塌的效果
	private void ShowParticle(){
		if (DestroyEffectType != BuildingDestroyEffectType.None) {
			PoolManager.CreateObject (DestroyEffectType.ToString(), transform.position, transform.eulerAngles);
		}
	}
}
