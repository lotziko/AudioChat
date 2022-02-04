using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Animations;

namespace AudioChat
{
	public class VRPlayerAvatar : GenericPlayerAvatar
	{
		[SerializeField] private VRIK _ik;
		[SerializeField] private IKConstraints _ikConstraints;

		// ===============================================================

		protected override void Initialize(bool isMine)
		{
			base.Initialize(isMine);
			if (isMine)
			{
				_ikConstraints.Head.constraintActive = true;
				_ikConstraints.LeftHand.constraintActive = true;
				_ikConstraints.RightHand.constraintActive = true;
			}
		}

		[System.Serializable]
		public class IKConstraints
		{
			[SerializeField] private ParentConstraint _headTarget;
			[SerializeField] private ParentConstraint _leftHandTarget;
			[SerializeField] private ParentConstraint _rightHandTarget;

			public ParentConstraint Head { get { return _headTarget; } }
			public ParentConstraint LeftHand { get { return _leftHandTarget; } }
			public ParentConstraint RightHand { get { return _rightHandTarget; } }
		}
	}
}
