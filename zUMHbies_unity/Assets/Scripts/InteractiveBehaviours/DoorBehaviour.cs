﻿using UnityEngine;
using System.Collections;

public class DoorBehaviour : GeneralInteractiveBehaviour
{
		public bool Open;
		public bool Locked;
		public Animator MyAnimator;

		private HingeJoint myJoint;
		private JointMotor motor;
		private float defaultJointVelocity;

		// Use this for initialization
		protected override void Start ()
		{
				base.Start ();

				myJoint = GetComponent<HingeJoint> ();
				defaultJointVelocity = myJoint.motor.targetVelocity;
				motor = myJoint.motor;
				constrain ();
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (myJoint.useMotor) {
						if (Open && Mathf.Abs (transform.rotation.eulerAngles.y - 90) < 0.1)
								myJoint.useMotor = false;
						if (!Open && Mathf.Abs (transform.rotation.eulerAngles.y) < 0.1) {
								myJoint.useMotor = false;
								constrain ();
						}
				}
		}

		public override void _Activate ()
		{
				if (!Locked) {
						Open = !Open;
						motor.targetVelocity = defaultJointVelocity * (Open ? 1 : -1);
						myJoint.motor = motor;
						myJoint.useMotor = true;

						if (Open) {
								constrain ();
								if (MyAnimator)
										MyAnimator.SetTrigger (HashIDs.Open);
						}
				}
		}	

		void constrain ()
		{
				//So the door can't be pushed around while closed
				if (Open)
						rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				else
						rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		}
}
