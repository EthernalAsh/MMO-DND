public class Player : Character
{
    public override void Move()
    {
        MoveToTarget();
    }

    public override void SetTarget(bool useQuickPath = false)
    {
        // raycast to a position

        // Check if we can stand on this position

        // Decide a safest and quickest path to the position

        // Check if we can move to this position without moving through dangerous positions
        //      if we do have to do this roll dex safe for moving. On fail we move as close as we can to the target position.
        //      if we dont set target position.
    }
}