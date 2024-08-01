using System;
using Kadinche.Kassets.Variable;
using UnityEngine;

namespace Kadinche.Kassets.Sample
{
    [Serializable]
    public class Enemy
    {
        public string name;
        public int maxHP;
        public int atk;
        public int def;
    }
    
    [CreateAssetMenu(fileName = "AddToScriptableObjectTest", menuName = MenuHelper.DefaultOtherMenu + "AddToScriptableObjectTest", order = 100)]
    public class AddToScriptableObjectTest : VariableCore<Enemy>
    {
        [SerializeField] private IntVariable enemyMaxHPVariable;
        [SerializeField] private IntVariable enemyHPVariable;
        [SerializeField] private IntVariable enemyAtkVariable;
        [SerializeField] private IntVariable enemyDefVariable;

        private void OnValidate()
        {
            Value.name = name;
            
            if (enemyMaxHPVariable != null)
            {
                enemyMaxHPVariable.Value = Value.maxHP;
            }
            
            if (enemyHPVariable != null)
            {
                enemyHPVariable.Value = Value.maxHP;
            }
            
            if (enemyAtkVariable != null)
            {
                enemyAtkVariable.Value = Value.atk;
            }
            
            if (enemyDefVariable != null)
            {
                enemyDefVariable.Value = Value.def;
            }
        }
        
#if UNITY_EDITOR

        private const string MaxHPVariableName = "Max HP";
        private const string HPVariableName = "HP";
        private const string AtkVariableName = "Attack";
        private const string DefVariableName = "Defense";

        // Add
        [ContextMenu("Add" + MaxHPVariableName + " Variable")]
        private void AddEnemyMaxHPVariable()
        {
            enemyMaxHPVariable = this.Add<IntVariable>(MaxHPVariableName);
        }

        [ContextMenu("Add" + MaxHPVariableName + " Variable", true)]
        private bool AddEnemyMaxHPVariableValidate()
        {
            return !this.ValidateExistence<IntVariable>(MaxHPVariableName);
        }

        [ContextMenu("Add" + HPVariableName + " Variable")]
        private void AddEnemyHPVariable()
        {
            enemyHPVariable = this.Add<IntVariable>(HPVariableName);
        }

        [ContextMenu("Add" + HPVariableName + " Variable", true)]
        private bool AddEnemyHPVariableValidate()
        {
            return !this.ValidateExistence<IntVariable>(HPVariableName);
        }

        [ContextMenu("Add" + AtkVariableName + " Variable")]
        private void AddEnemyAtkVariable()
        {
            enemyAtkVariable = this.Add<IntVariable>(AtkVariableName);
        }

        [ContextMenu("Add" + AtkVariableName + " Variable", true)]
        private bool AddEnemyAtkVariableValidate()
        {
            return !this.ValidateExistence<IntVariable>(AtkVariableName);
        }

        [ContextMenu("Add " + DefVariableName + " Variable")]
        private void AddDefVariable()
        {
            enemyDefVariable = this.Add<IntVariable>(DefVariableName);
        }

        [ContextMenu("Add " + DefVariableName + " Variable", true)]
        private bool AddDefVariableValidate()
        {
            return !this.ValidateExistence<IntVariable>(DefVariableName);
        }

        // Remove
        [ContextMenu("Remove " + MaxHPVariableName + " Variable")]
        private void RemoveEnemyMaxHPVariable()
        {
            this.Remove<IntVariable>(MaxHPVariableName);
            enemyMaxHPVariable = null;
        }
        
        [ContextMenu("Remove " + MaxHPVariableName + " Variable", true)]
        private bool RemoveEnemyMaxHPVariableValidate()
        {
            return this.ValidateExistence<IntVariable>(MaxHPVariableName);
        }

        [ContextMenu("Remove " + HPVariableName + " Variable")]
        private void RemoveEnemyHPVariable()
        {
            this.Remove<IntVariable>(HPVariableName);
            enemyHPVariable = null;
        }
        
        [ContextMenu("Remove " + HPVariableName + " Variable", true)]
        private bool RemoveEnemyHPVariableValidate()
        {
            return this.ValidateExistence<IntVariable>(HPVariableName);
        }

        [ContextMenu("Remove " + AtkVariableName + " Variable")]
        private void RemoveEnemyAtkVariable()
        {
            this.Remove<IntVariable>(AtkVariableName);
            enemyAtkVariable = null;
        }
        
        [ContextMenu("Remove " + AtkVariableName + " Variable", true)]
        private bool RemoveEnemyAtkVariableValidate()
        {
            return this.ValidateExistence<IntVariable>(AtkVariableName);
        }

        [ContextMenu("Remove " + DefVariableName + " Variable")]
        private void RemoveDefVariable()
        {
            this.Remove<IntVariable>(DefVariableName);
            enemyDefVariable = null;
        }

        [ContextMenu("Remove " + DefVariableName + " Variable", true)]
        private bool RemoveDefVariableValidate()
        {
            return this.ValidateExistence<IntVariable>(DefVariableName);
        }

#endif
    }
}