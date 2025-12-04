using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyD : EnemyController
{
    // The enemy may shoot words to multiple players at the same time
    // No special code needed

    protected override void ShootWord(string word)
    {
        int randomAttack = Random.Range(0, 10);
        if (randomAttack < 5)
        {
            ShootWordD(word);
        }
        else if (randomAttack < 8)
        {
            ShootWordB(word);
        }
        else
        {
            ShootWordC(word);
        }
    }
}
