using Enemy.Controller;
using UnityEngine;

namespace Enemy.ShareInformation
{
    public static class EnemyShareInformation
    {
        // When an Enemy reached the noisy item all others nearby will get the same information to start searching and not check the sound origin as well
        public static bool ReachedNoisyItem;
        
        // When two enemies are talking, no further enemy should come to talk
        public static float EnemyTalkingNumber = 0;

        // When a enemy is looting a specific place, no further enemy can loot at the same place
        // When a enemy is looting, this bool prevent that the enemies are able to talk
        public static bool IsLooting;

        // is for the player chase, specific for the high ground chase when the player is on the high ground
        // when the first enemy reached the closest point to the player, this enemy will be taken as orientation to stop the other ones
        public static EnemyController EnemyInstance;
        // when the first enemy reached the closest point to the player, he will stop and signalize the other one that they have to stop around the destination to not collide and not to play the run animation anymore
        public static bool FirstEnemyReachedDestination;

        public static float CheckSound;
    }
}


