namespace Octamino
{
    public interface IPlayerInput
    {
        PlayerAction? GetPlayerAction();
        void Update();
        void Cancel();
    }
}
