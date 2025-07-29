using UnityEngine;
using Fusion;

// CharacterController를 이용한 아바타 이동을 담당합니다.
[RequireComponent(typeof(CharacterController))]
public class AvatarMovementController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    private CharacterController _characterController;


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        // 상태 권한(State Authority)이 없으면 입력을 처리하지 않습니다.
        // 다른 클라이언트에서는 NetworkCharacterController가 자동으로 위치를 동기화합니다.
        if (!HasStateAuthority)
        {
            return;
        }

        // GetInput()을 통해 네트워크 입력을 받아옵니다.
        if (GetInput(out AvatarHardwareRig.NetworkInputData data))
        {
            Vector3 moveDirection = transform.forward * data.joystickInput.y + transform.right * data.joystickInput.x;
            _characterController.Move(moveDirection * moveSpeed * Runner.DeltaTime);
        }
    }
}