﻿using UnityEngine;
using System.Collections;

public class ZombieBasicBehaviour : MonoBehaviour, IKillable, ISwitchedByExtTrigger
{
		public static PlayerReferences PlayerRefs;

		public Transform HeadTransform;

		public int MaxHealth;
		public float WanderSpeed;
		public float RunSpeed;
		public float ReactionTime;
		public float WaitUntilRunTime;
		public float SecondsBetweenRaycasts;

		protected float	health;
		protected NavMeshAgent myNavMeshAgent;
		protected Vector3 myDestination;

		protected bool busy;
		protected bool chasingPlayer;

		//IKillable members
		public int _MaxHealth {
				get {
						return MaxHealth;
				}
				set {
						MaxHealth = value;
				}
		}

		public float _Health {
				get {
						return health;
				}

				set {
						health = value;
				}
		}

		public void _TakeDamage (float a_damage, Vector3 a_hitPoint)
		{
				print ("ayayayay");
				health -= a_damage;

				if (health <= 0)
						_Die ();
		}

		public void _Die ()
		{
				health = 0; //In case it was negative -not that important anyway-

				//Scream or something
		}
		//END OF IKillable MEMBERS

		//ISwitchedByExtTrigger members

		public void _TriggerEnterSwitch (int a_index, Collider a_collider)
		{
				
		}

		public void _TriggerStaySwitch (int a_index, Collider a_collider)
		{
				switch (a_index) {
				case 0: //Vision trigger
						if (a_collider == PlayerRefs.Player.collider)
								checkPlayerVisibility ();
						break;
				}
		}

		public void _TriggerExitSwitch (int a_index, Collider a_collider)
		{

		}
		//END OF ISwitchedByExtTrigger MEMBERS


		// Use this for initialization
		void Start ()
		{
				PlayerRefs = GameObject.FindGameObjectWithTag (Tags.GAME_CONTROLLER).GetComponent<PlayerReferences> ();
				health = MaxHealth;
				myNavMeshAgent = GetComponent<NavMeshAgent> ();
				myDestination = transform.position;
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				//Wander around
		}

		void OnTriggerStay (Collider a_collider)
		{
				//Zombie gets hit by light that could be being managed by player
				if (!busy && a_collider.tag == Tags.ATTRACTIVE_LIGHT) {

						//Is the light managed by a player?
						IPickable t_light = a_collider.gameObject.Ext_GetClosestBehaviourWithInterfaceInHierarchy<IPickable> ();
						if (t_light != null && t_light._Equiped == true) {

								//Direct vision with player?
								if (gameObject.Ext_DirectRay (HeadTransform.position, PlayerRefs.Player.transform.position, PlayerRefs.PlayerCollider)) {
										busy = true;
										StopCoroutine ("attentionTowards"); //Stops all previous attention
										StartCoroutine ("attentionTowards", a_collider.transform.parent.position); //Real center of the light is in the parent of the light
								}
						}
				}
		}

		void checkPlayerVisibility ()
		{
				//Already chasing the player?
				if (chasingPlayer)
						return; //Yes, return

				//Direct vision with player? Go chase that little bitch
				if (gameObject.Ext_DirectRay (HeadTransform.position, PlayerRefs.Player.transform.position, PlayerRefs.Player.collider)) {
						StartCoroutine (chasePlayer ());
				}
		}

		protected IEnumerator attentionTowards (Vector3 a_destination)
		{
				//Act weird
				yield return new WaitForSeconds (ReactionTime);

				setDestination (a_destination, 0.05f); //Only able to look at the target, barely translates
				yield return new WaitForSeconds (WaitUntilRunTime);

				run ();
		}
		
		protected void setDestination (Vector3 a_destination, float a_newSpeed)
		{
				myDestination = a_destination;
				myNavMeshAgent.SetDestination (a_destination);
				myNavMeshAgent.speed = a_newSpeed;
		}

		protected void run ()
		{
				myNavMeshAgent.speed = RunSpeed;
		}

		protected IEnumerator chasePlayer ()
		{
				busy = true;
				chasingPlayer = true;
				run ();

				while (true) {
						//Is the player still visible?
						if (gameObject.Ext_DirectRay (HeadTransform.position, PlayerRefs.Player.transform.position, PlayerRefs.Player.collider)) {
								//Update target
								myNavMeshAgent.SetDestination (PlayerRefs.FeetTransform.position);
								yield return new WaitForSeconds (SecondsBetweenRaycasts);
						} else {
								//Player lost, zombie will continue running towards target, but will update the target no more
								break;
						}
				}

				chasingPlayer = false;
				busy = false;
		}
}