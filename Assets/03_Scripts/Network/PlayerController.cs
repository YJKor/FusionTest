using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : NetworkBehaviour
{

    // Animator 컴포넌트를 인스펙터에서 할당
    [SerializeField] private Animator _animator;
    // IK 타겟 (모든 클라이언트가 참조)
    [SerializeField] private Transform _leftHandIKTarget;
    [SerializeField] private Transform _rightHandIKTarget;

    // IsWalking 상태를 네트워크로 동기화할 변수
    [Networked] public NetworkBool IsWalking { get; set; }

    // IK 타겟의 위치와 회전을 동기화할 네트워크 변수
    [Networked] public Vector3 LeftHandIKTargetPos { get; set; }
    [Networked] public Quaternion LeftHandIKTargetRot { get; set; }
    [Networked] public Vector3 RightHandIKTargetPos { get; set; }
    [Networked] public Quaternion RightHandIKTargetRot { get; set; }

    // --- 더 이상 [SerializeField]를 사용하지 않고, 런타임에 찾아서 할당할 변수 ---
    private Transform _localXRRig; // XR Origin을 담을 변수 (선택 사항)
    private Transform _leftHandController;
    private Transform _rightHandController;


    public override void Spawned()
    {
        // Object.HasInputAuthority는 이 오브젝트가 로컬 플레이어의 것인지를 확인합니다.
        // 이 코드는 오직 '나'의 컴퓨터에서, '나'의 아바타에 대해서만 실행됩니다.
        if (Object.HasInputAuthority)
        {
            // 디버깅을 위해 로그를 남깁니다.
            Debug.Log("Spawned: 로컬 플레이어의 아바타가 생성되었습니다. 컨트롤러를 찾습니다.");

            // 씬에 있는 모든 XRController를 찾습니다.
            // 좀 더 안정적인 방법은 XR Origin에 특정 태그나 관리 스크립트를 두는 것이지만,
            // 이 방법이 가장 간단하고 직관적입니다.
            var controllers = FindObjectsOfType<XRController>();
            foreach (var controller in controllers)
            {
                // 컨트롤러의 Action 이름을 기반으로 왼손/오른손을 구분합니다.
                // (XR Default Input Actions 기준)
                if (controller.name.ToLower().Contains("left"))
                {
                    _leftHandController = controller.transform;
                }
                else if (controller.name.ToLower().Contains("right"))
                {
                    _rightHandController = controller.transform;
                }
            }

            // 제대로 찾아졌는지 확인합니다.
            if (_leftHandController == null || _rightHandController == null)
            {
                Debug.LogError("컨트롤러를 찾을 수 없습니다! XR Origin의 컨트롤러 오브젝트 이름을 확인해주세요.");
            }
        }
        else
        {
            // 다른 사람의 컴퓨터에 생성된 나의 아바타, 또는 내 컴퓨터에 생성된 다른 사람의 아바타입니다.
            // 이 경우엔 컨트롤러를 찾을 필요가 없습니다.
            Debug.Log("Spawned: 원격 플레이어의 아바타가 생성되었습니다.");
        }
    }


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


    }
}