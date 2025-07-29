using UnityEngine;
using Fusion;

// 네트워크로 동기화된 Transform 정보를 아바타의 IK에 적용합니다.
[RequireComponent(typeof(Animator))]
public class AvatarIKController : NetworkBehaviour
{
    private Animator _animator;
    private AvatarHardwareRig _hardwareRig; // 동기화된 데이터에 접근하기 위한 참조

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _hardwareRig = GetComponent<AvatarHardwareRig>();
    }

    // OnAnimatorIK는 Unity의 애니메이션 콜백이며, FixedUpdateNetwork가 아닙니다.
    // 이 함수는 매 프레임 렌더링 직전에 호출됩니다.
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator == null || _hardwareRig == null) return;

        // 머리 IK 설정
        //_animator.SetIKPositionWeight(AvatarIKGoal.Head, 1.0f);
        //_animator.SetIKRotationWeight(AvatarIKGoal.Head, 1.0f);
        //_animator.SetIKPosition(AvatarIKGoal.Head, _hardwareRig.HeadsetPos);
        //_animator.SetIKRotation(AvatarIKGoal.Head, _hardwareRig.HeadsetRot);

        // 왼손 IK 설정
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        _animator.SetIKPosition(AvatarIKGoal.LeftHand, _hardwareRig.LeftHandPos);
        _animator.SetIKRotation(AvatarIKGoal.LeftHand, _hardwareRig.LeftHandRot);

        // 오른손 IK 설정
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _hardwareRig.RightHandPos);
        _animator.SetIKRotation(AvatarIKGoal.RightHand, _hardwareRig.RightHandRot);
    }
}