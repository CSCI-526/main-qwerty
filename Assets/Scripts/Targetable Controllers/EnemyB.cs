using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyB : EnemyController
{
    // This enemy will spawn projectiles that need to be typed in reversed order
    // color - magenta, special code - 1

    protected override void ShootWord(string word)
    {
        int randomAttack = Random.Range(0, 10);
        if (randomAttack < 3)
        {
            ShootWordA(word);
        }
        else if (randomAttack < 7)
        {
            ShootWordB(word);
        }
        else if (randomAttack < 9) {
            ShootWordC(word);
        }
        else
        {
            ShootWordD(word);
        }
    }

}
