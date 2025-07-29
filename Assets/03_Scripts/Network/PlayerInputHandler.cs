using Fusion;
using UnityEngine;

public class PlayerInputHandler : NetworkBehaviour, IBeforeUpdate
{
    // ...

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        // 키보드나 컨트롤러에서 입력 값을 읽어옵니다.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        data.moveDirection = new Vector3(moveHorizontal, 0, moveVertical);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            data.isJumpPressed = true;
        }

        // 채워진 데이터 구조체를 Fusion의 입력 시스템에 전달합니다.
        input.Set(data);
    }

    // OnInput 콜백을 활성화하기 위해 다른 콜백 인터페이스를 구현해야 할 수 있습니다.
    // 여기서는 IBeforeUpdate를 예시로 사용합니다.
    public void BeforeUpdate() { }
}