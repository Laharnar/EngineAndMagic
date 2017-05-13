using System.Collections.Generic;
using UnityEngine;

public static class BasicBehaviours {

    public static TreeNode BasicEnemyFinder(Ai ai, Transform transform) {
        return // search for enemies
            new DataTransferSequence().SetChildren(
                new FindNearestTarget().Init(new Dictionary<string, object>() {
                        { "transform", transform },
                        { "alliance" , ai.alliance }
                    }),
                // move near found target
                new MoveNearDestination().SetChecks(new CheckIsInRange())
                .Init(new Dictionary<string, object>() {
                        { "range" , 1.0f },
                        { "ai" , ai }
                    })
            );
    }

    public static TreeNode BasicAttackRepeater(Weapon weapon) {
        return // attack infinetly, keeping note on attack rate
            new InfiniteRepeater().SetChild(
                new DataTransferSequence().SetChildren(AnAttack(weapon))
            );
    }

    public static TreeNode[] AnAttack(Weapon weapon) {
        return new TreeNode[] {
                    new Timer().Init(new Dictionary<string, object>() {
                        { "ctime", Time.time }
                    }),
                    new SetTime().Init(new Dictionary<string, object>() {
                        { "rate", weapon.atkRate }
                    }),
                    new SetTriggerAnimation().Init(new Dictionary<string, object>() {
                        { "triggerChoice" , "Attack" },
                        { "animator", weapon.anim },
                        { "active", true }// todo: it will check if animation exists. it will allows to preset actions for animation, but have it turned off
                    }),
                };
    }
}