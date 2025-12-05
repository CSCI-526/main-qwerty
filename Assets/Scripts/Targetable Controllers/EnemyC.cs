using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyC : EnemyController
{
    // This enemy will spawn projectiles that can only be seen by the target player
    // color - red, special code - 2

    protected override void ShootWord(string word)
    {
        int randomAttack = Random.Range(0, 10);
        if (randomAttack < 3)
        {
            ShootWordA(word);
        }
        else if (randomAttack < 7)
        {
            ShootWordC(word);
        }
        else if (randomAttack < 9)
        {
            ShootWordB(word);
        }
        else
        {
            ShootWordD(word);
        }
    }
}
