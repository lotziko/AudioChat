using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioChat.Pose
{
    [Serializable]
    public class HandInfo
    {
        public Vector3 AttachPosition = Vector3.zero;
        public Quaternion AttachRotation = Quaternion.identity;
		[UnityEngine.Serialization.FormerlySerializedAs("FingerRotations")]
        public List<Quaternion> JointRotations = new List<Quaternion>();

		public void Save(Editor.PreviewHand hand)
		{
			AttachPosition = hand.transform.localPosition;
			AttachRotation = hand.transform.localRotation;
			JointRotations = hand.GetJointRotations();
		}
    }
}
