namespace Kadinche.Kassets
{
    public static class MenuHelper
    {
        public const string DefaultCommandMenu = "Kassets/Commands/";
        public const string DefaultGameEventMenu = "Kassets/Game Events/";
        public const string DefaultVariableMenu = "Kassets/Variables/";
        public const string DefaultJsonableVariableMenu = "Kassets/Jsonable Variables/";
        public const string DefaultCollectionMenu = "Kassets/Collections/";
        public const string DefaultTransactionMenu = "Kassets/Transactions/";
        public const string DefaultObjectPoolMenu = "Kassets/Object Pools/";
        public const string DefaultOtherMenu = "Kassets/Others/";
    }
    
    public enum ValueEventType
    {
        ValueAssign,
        ValueChange
    }
}