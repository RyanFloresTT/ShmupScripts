using System;
using System.Collections.Generic;
using AbilitySystem;

namespace _Project.Scripts.Enemy {
    public abstract class BTNode {
        public abstract bool Execute(global::Enemy enemy);
    }

    public class Sequence : BTNode {
        readonly List<BTNode> children = new();

        public void AddChild(BTNode child) {
            this.children.Add(child);
        }

        public override bool Execute(global::Enemy enemy) {
            bool anyExecuted = false;
            foreach (BTNode child in this.children)
                if (child.Execute(enemy))
                    anyExecuted = true;

            return anyExecuted;
        }
    }

    public class Selector : BTNode {
        readonly List<BTNode> children = new();

        public void AddChild(BTNode child) {
            this.children.Add(child);
        }

        public override bool Execute(global::Enemy enemy) {
            bool anyExecuted = false;
            foreach (BTNode child in this.children)
                if (child.Execute(enemy))
                    anyExecuted = true;

            return anyExecuted;
        }

        public class ConditionNode : BTNode {
            readonly ConditionSO condition;

            public ConditionNode(ConditionSO condition) {
                this.condition = condition;
            }

            public override bool Execute(global::Enemy enemy) {
                return this.condition.CheckCondition(enemy);
            }
        }

        public class ActionNode : BTNode {
            readonly Action<global::Enemy> action;

            public ActionNode(Action<global::Enemy> action) {
                this.action = action;
            }

            public override bool Execute(global::Enemy enemy) {
                this.action.Invoke(enemy);
                return true;
            }
        }

        public class AbilityNode : BTNode {
            ConditionSO condition;
            int abilityIndex;
            EnemyAbilitySystem abilitySystem;

            public AbilityNode(ConditionSO condition, int abilityIndex, EnemyAbilitySystem abilitySystem) {
                this.condition = condition;
                this.abilityIndex = abilityIndex;
                this.abilitySystem = abilitySystem;
            }

            public override bool Execute(global::Enemy enemy) {
                if (this.condition.CheckCondition(enemy)) {
                    this.abilitySystem.OnAbilityPerformed(this.abilityIndex);
                    return true;
                }

                return false;
            }
        }
    }
}