public interface IPlayerState
{
    IPlayerState Execute(PlayerController player);
    void Enter(PlayerController player);
    void Exit(PlayerController player);
}

