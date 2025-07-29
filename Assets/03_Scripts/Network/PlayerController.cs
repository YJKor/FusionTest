using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    // Animator 컴포넌트를 인스펙터에서 할당
    [SerializeField] private Animator _animator;

    // IsWalking 상태를 네트워크로 동기화할 변수
    [Networked]
    public NetworkBool IsWalking { get; set; }


    // 실제 VR 컨트롤러 (로컬 플레이어만 참조)
    [SerializeField] private Transform _leftHandController;
    [SerializeField] private Transform _rightHandController;


    // IK 타겟 (모든 클라이언트가 참조)
    [SerializeField] private Transform _leftHandIKTarget;
    [SerializeField] private Transform _rightHandIKTarget;

    // IK 타겟의 위치와 회전을 동기화할 네트워크 변수
    [Networked] public Vector3 LeftHandIKTargetPos { get; set; }
    [Networked] public Quaternion LeftHandIKTargetRot { get; set; }
    [Networked] public Vector3 RightHandIKTargetPos { get; set; }
    [Networked] public Quaternion RightHandIKTargetRot { get; set; }



    public override void FixedUpdateNetwork()
    {
        // 이 오브젝트에 대한 입력 권한이 없으면(로컬 플레이어가 아니면) 아무것도 하지 않음
        if (GetInput(out NetworkInputData data))
        {
            // 입력(예: 컨트롤러 조이스틱)에 따라 캐릭터를 이동
            // ... (캐릭터 이동 로직) ...

            // 이동 벡터의 크기를 기반으로 걷고 있는지 판단
            bool isCurrentlyWalking = data.moveDirection.magnitude > 0.1f;

            // 로컬에서의 걷기 상태가 네트워크 변수와 다를 경우에만 업데이트
            if (isCurrentlyWalking != IsWalking)
            {
                IsWalking = isCurrentlyWalking;
            }
        }
        if (HasInputAuthority)
        {
            // 실제 컨트롤러의 월드 좌표와 회전값을 네트워크 변수에 기록
            LeftHandIKTargetPos = _leftHandController.position;
            LeftHandIKTargetRot = _leftHandController.rotation;
            RightHandIKTargetPos = _rightHandController.position;
            RightHandIKTargetRot = _rightHandController.rotation;
        }
    }

    // Render는 모든 클라이언트에서 매 프레임 호출됨
    public override void Render()
    {
        // 동기화된 IsWalking 값을 Animator에 적용
        _animator.SetBool("IsWalking", IsWalking);


        _leftHandIKTarget.position = LeftHandIKTargetPos;
        _leftHandIKTarget.rotation = LeftHandIKTargetRot;

        _rightHandIKTarget.position = RightHandIKTargetPos;
        _rightHandIKTarget.rotation = RightHandIKTargetRot;
t;

    }
}