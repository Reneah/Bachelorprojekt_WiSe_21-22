
namespace Enemy.ShareInformation
{
    public static class EnemyShareInformation
    {
        // When an Enemy reached the noisy item all others nearby will get the same information to start searching and not check the sound origin as well
        public static bool ReachedNoisyItem;
        
        // when the first enemy reached the closest point to the player, he will stop and signalize the other one that they have to stop around the destination
        public static bool FirstEnemyReachedDestination;
    }
}


