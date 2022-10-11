namespace GameStudio.HunterGatherer.Networking.Events
{
    /// <summary>Defines the different types of networking events that can happen. Each has its own type of content to return</summary>
    public enum NetworkingEventType : byte
    {
        /// <summary>Used when the lobby state ends. When sending, content should be null. When listening to this, the content will also be null</summary>
        LobbyEnd = 1, //starts at one because zero is used as a wildcard for clearing the event cache

        /// <summary>Used when a client wants to modify another clients stat system. When sending or listening, content should be StatModificationInfo.</summary>
        StatSystemModified,

        /// <summary>Used by the host to let clients know an item source has been modified. When sending or listening content is be ItemSourceModificationInfo</summary>
        ItemSourceModified,

        /// <summary>Used to notify a client it won the game. When sending or listening, content should an empty object</summary>
        PlayerWinGame,

        /// <summary>Used by a client to let the other clients know a unit aquipped a weapon. When sending or listening content should be ItemEquipInfo</summary>
        WeaponEquiped,

        /// <summary>Used by a client to let the other clients know a unit aquipped a weapon. When sending or listening content should be ArmorEquipInfo</summary>
        ArmorEquiped,

        /// <summary>Used when a client wants to instantiate a scene object but hasn't the rights to (isn't the host). Content is SceneObjectSpawnInfo</summary>
        SceneObjectSpawn,

        /// <summary>Used when a client wants to instantiate a static networked object. Content is SceneObjectSpawnInfo</summary>
        StaticObjectSpawn,

        /// <summary>Used when a client wants to destroy a static networked object. Content is the targets view id</summary>
        StaticObjectDestroy,

        /// <summary>Used when a client wants to share interaction info. Content is InteractionInfoSpawnData</summary>
        InteractionInfoSpawn,

        /// <summary>Used by a client to let the other clients know a unit equipped a packed item. When sending or listening content should be PackedItemInfo</summary>
        PackedItemEquiped,

        /// <summary>Used by the host to know if an tent has been occupied by x amount of units. When sending or listening content is be TentModificationInfo</summary>
        TentModified,

        /// <summary>Used when a client wants to let others know his unit did an animation. Content is UnitAnimationInfo</summary>
        UnitAnimated,

        /// <summary>Used when a client sets or changes the divisions type</summary>
        DivisionSetType,

        /// <summary> Used when a client sets or changes the divisions move target </summary>
        DivisionSetMoveTarget,

        /// <summary>Used when a client sets or changes the division of a unit</summary>
        UnitSetDivision,

        /// <summary>Used when a client's unit hits another unit</summary>
        UnitHit,

        /// <summary>Used when a client's unit fires a projectile at another unit</summary>
        UnitFireProjectile,

        /// <summary>Used when a client's unit kills another unit</summary>
        ManPowerResourceAdded,

        /// <summary>Used when a client recieves god favor</summary>
        GodFavorResourceAdded,

        /// <summary>Used when a client's unit changes movementspeed</summary>
        UnitSetMovementSpeed,

        /// <summary>Used when a client selects a god</summary>
        GodSet,
        /// <summary>Called when a structures use method gets called</summary>
        UseStructure,
        /// <summary>Syncs the division data across all clients, mostly called when upgrading a division</summary>
        SyncDivisionData,
        /// <summary>Sets health of a specific unit </summary>
        SyncHealth,

        Destroy
    }

    /// <summary>Defines the different types of stat modification types that can happen</summary>
    public enum StatModificationType : byte
    {
        /// <summary>Used when an object wants to attack another object</summary>
        Attack = 0
    }

    /// <summary>The group or person that will receive a networking event</summary>
    public enum EventReceivers
    {
        /// <summary>All other clients, so All excluding myself</summary>
        Other = 0,

        /// <summary>All clients including myself</summary>
        All = 1,

        /// <summary>The client hosting this room</summary>
        Host = 2
    }

    /// <summary>Defines the different types of networking request dicisions a host can make based on NetworkingRequestTypes</summary>
    public enum NetworkingRequestDecision : byte
    {
        /// <summary>Used when the host has decided on which player gets the item.</summary>
        ItemPickupDecision = 50, //starts at 50 to avoid conflict with NetworkingEventType

        /// <summary>Used when the host has decided on which player gets the berry. </summary>
        ItemSourceDecision,

        /// <summary>Used when the host decided if you lost, won or draw the game.</summary>
        LoseDecision,

        /// <summary>Used when the host failed processing the request of the client. Content will contain a message regarding what went wrong.
        /// !important requestCallback will still be called but content will be null</summary>
        DecisionFailed,
    }

    /// <summary>Defines the different types of networking requests that can happen. Each has its own type of content to return</summary>
    public enum NetworkingRequestType : byte
    {
        /// <summary>Used when the client wants to check if it lost the game</summary> //TODO: State what the content needs to contain!!!
        Lose,
    }

    /// <summary>The amount of ticks a request will wait to add other request to the same request heap to check what request was first</summary>
    public enum NetworkingRequestDelay
    {
        /// <summary>The most used delay type is fast. Use this one for normal behaviour</summary>
        Fast = 2,

        /// <summary>Slow is used when you need to make sure a lot of requests are taken into account when checking which one was first</summary>
        Slow = 20
    }

    /// <summary>Defines the different types of responses the LoseDecision Reequst can give</summary>
    public enum NetworkingLoseDesicionType
    {
        /// <summary>Used when the player lost</summary>
        Lost,

        /// <summary>Used when the player wins</summary>
        Win,

        /// <summary>Used when the player ends in a draw</summary>
        Draw
    }

    /// <summary>Enumeration stat stores all the equippableWeapons. Used to send events over network</summary>
    public enum EquippableWeapons
    {
        /// <summary>Used when no weapon is equiped</summary>
        None,

        /// <summary>Used when the spear is equiped</summary>
        Spear
    }

    /// <summary>Enumeration stat stores all the equippableArmors. Used to send events over network</summary>
    public enum EquippableArmors
    {
        /// <summary>Used when no armor is equiped</summary>
        None,

        /// <summary>Used when leather armor is equiped</summary>
        LeatherArmor
    }

    /// <summary>Defines all types of PackedItem. Used to send events over network</summary>
    public enum PackedItemType
    {
        /// <summary>Used when no PackedItem is equipped</summary>
        None,

        /// <summary>Used when the packed tent is equipped</summary>
        Tent
    }
}