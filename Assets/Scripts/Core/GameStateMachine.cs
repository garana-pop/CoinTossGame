using UnityEngine;

/// <summary>
/// �Q�[���̏�ԑJ�ڂ�Ǘ�����N���X�B
/// ��Ԃ̒ǉ��E�J�ڃ��[���̕ύX�͂��̃N���X�݂̂�C������B
/// </summary>
public class GameStateMachine
{
    // �Q�[���̏�Ԃ�\���񋓌^
    public enum State
    {
        Idle,         // �ҋ@��
        Launching,    // �R�C��������
        Judging,      // ���n���蒆
        ShowingScore, // �X�R�A�\����
        PowerUp,      // �p���[�A�b�v�I��
        WaveTransit   // �E�F�[�u�ڍs��
    }

    public State Current { get; private set; } = State.Idle; // ���݂̏��

    /// <summary>�w�肵����Ԃ֑J�ڂ���B�s���J�ڂ̓��O��o���ăX�L�b�v����B</summary>
    /// <param name="next">�J�ڐ�̏��</param>
    public void Transition(State next)
    {
        if (Current == next) return;

        if (!IsValidTransition(Current, next))
        {
            Debug.LogWarning($"{nameof(GameStateMachine)}: �����ȑJ�� {Current} �� {next}");
            return;
        }

        // �J�ڃ��O�̏o��
        Debug.Log($"{nameof(GameStateMachine)}: {Current} �� {next}");
        Current = next;
    }

    /// <summary>�J�ڂ̍��@������؂���B�V������Ԃ�ǉ�����ꍇ�͂����ɒǋL����B</summary>
    /// <param name="from">�J�ڌ��̏��</param>
    /// <param name="to">�J�ڐ�̏��</param>
    /// <returns>�J�ڂ����@�ł���� true�A�s���ł���� false</returns>
    private bool IsValidTransition(State from, State to) => (from, to) switch
    {
        (State.Idle, State.Launching) => true,
        (State.Launching, State.Judging) => true,
        (State.Judging, State.ShowingScore) => true,
        (State.ShowingScore, State.Launching) => true,
        (State.ShowingScore, State.PowerUp) => true,
        (State.PowerUp, State.WaveTransit) => true,
        (State.WaveTransit, State.Launching) => true,
        _ => false
    };
}