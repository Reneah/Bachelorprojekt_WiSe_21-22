using UnityEngine;

namespace Enemy.ShareInformation
{
    public static class EnemyShareInformation
    {
        // When an Enemy reached the noisy item all others nearby will get the same information to start searching and not check the sound origin as well
        public static bool ReachedNoisyItem;

        //I need the global information if the player is spotted or not to play the flee animation
        public static bool PlayerIsVisible;

        // When two enemies are talking, no further enemy should come to talk
        public static float EnemyTalkingNumber = 0;

        // When a enemy is looting a specific place, no further enemy can loot at the same place
        // When a enemy is looting, this bool prevent that the enemies are able to talk
        public static bool IsLooting;
    }
}


