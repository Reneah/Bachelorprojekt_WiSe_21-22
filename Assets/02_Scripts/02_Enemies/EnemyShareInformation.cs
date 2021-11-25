using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyShareInformation
{
    // When an Enemy reached the noisy item all others nearby will get the same information to start searching and not check the sound origin as well
    public static bool ReachedNoisyItem;

    //I need the global information if the player is spotted or not to play the flee animation
    public static bool PlayerIsVisible;
}
