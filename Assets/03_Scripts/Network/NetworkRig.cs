using UnityEngine;
using Fusion;

public class NetworkRig : NetworkBehaviour
{
    // 인스펙터에서 아바타의 각 파츠를 연결해줍니다.
    [SerializeField] private Transform head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private HardwareRig hardwareRig; // 씬에 있는 XR Origin을 참조할 변수
    private NetworkCharacterController _ncc; // NetworkCharacterController 참조

    void Awake()
    {
        // 컴포넌트 참조를 미리 가져옵니다.
        _ncc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        // 이 아바타가 '내' 아바타일 경우에만 실행됩니다.
        if (Object.HasInputAuthority)
        {
            // 씬에 있는 단 하나의 HardwareRig를 찾습니다.
            hardwareRig = FindObjectOfType<HardwareRig>();
            if (hardwareRig == null) Debug.LogError("씬에서 HardwareRig를 찾을 수 없습니다!");
        }
    }


    public override void FixedUpdateNetwork()
    {
        // 입력 권한이 있는 플레이어(로컬 플레이어)만 입력을 받아 움직입니다.
        if (GetInput(out PlayerNetworkInput input))
        {
            // 입력 값에 따라 이동 벡터를 계산합니다.
            Vector3 moveDirection = transform.forward * input.Movement.y + transform.right * input.Movement.x;

            // Move 함수를 호출하여 캐릭터를 움직입니다. (물리 충돌 적용됨)
            _ncc.Move(moveDirection * Runner.DeltaTime * 5f); // 5f는 이동 속도
        }
    }




    public override void Render()
    {
        // Render에서는 더 이상 몸통의 위치(transform.position)를 제어하지 않습니다.
        // 오직 머리와 손의 렌더링 위치만 맞춥니다.
        if (Object.HasInputAuthority && hardwareRig != null)
        {
            // 몸통의 회전은 여전히 머리를 따라갑니다.
            transform.rotation = Quaternion.Euler(0, hardwareRig.head.rotation.eulerAngles.y, 0);

            // 머리와 손의 위치/회전 맞추기
            head.position = hardwareRig.head.position;
            //head.rotation = hardwareRig.head.rotation;

            leftHand.position = hardwareRig.leftHand.position;
            //leftHand.rotation = hardwareRig.leftHand.rotation;

            rightHand.position = hardwareRig.rightHand.position;
            //rightHand.rotation = hardwareRig.rightHand.rotation;
        }
    }
}