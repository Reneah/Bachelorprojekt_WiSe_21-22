using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.SearchArea
{
    public class SearchAreaOverview : MonoBehaviour
    {
        [Tooltip("the amount of enemies that can search at once after the player")]
        [SerializeField] private int _enemySearchMaxAmount;

        public int EnemySearchMaxAmount
        {
            get => _enemySearchMaxAmount;
            set => _enemySearchMaxAmount = value;
        }

        // the amount of enemies that are searching currently
        private int _enemySearchAmount = 0;

        public int EnemySearchAmount
        {
            get => _enemySearchAmount;
            set => _enemySearchAmount = value;
        } 
    }
}
