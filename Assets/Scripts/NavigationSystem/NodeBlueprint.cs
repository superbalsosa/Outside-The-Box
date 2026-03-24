using UnityEngine;

namespace Map
{
    public enum NodeType
    {
        FightEvent,
        EliteFightEvent,
        RestEvent,
        TreasureEvent,
        StoreEvent,
        FinalEvent,
        MysteryEvent
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