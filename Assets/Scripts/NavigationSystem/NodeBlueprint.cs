using UnityEngine;

namespace Map
{
    public enum NodeType
    {
        RestEvent,
        FightEvent,
        StoreEvent,
        GatherEvent,
        MysteryEvent,
        TreasureEvent,
        FinalEvent
    }
}

namespace Map
{
    [CreateAssetMenu]
    public class NodeBlueprint : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}