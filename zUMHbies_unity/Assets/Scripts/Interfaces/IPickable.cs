﻿using UnityEngine;
using System.Collections;

public interface IPickable
{
		int _Size {
				get;
				set;
		}

		int _AnimID {
				get;
		}

		string _Name {
				get;
		}

		bool _Equiped {
				get;
				set;
		}

		Texture2D _Picture {
				get;
				set;
		}

		Vector3 _EquipPosition {
				get;
		}

		Quaternion _DropRotation {
				get;
		}

		Quaternion _EquipRotation {
				get;
		}

		void _BeStored ();
		void _BeRetrieved ();
		void _BeUsed ();
		void _Place (Transform a_parent, Vector3 a_coordinates, bool a_beKinematic = false, int a_handIndex = 0);
}