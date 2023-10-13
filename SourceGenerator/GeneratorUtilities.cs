namespace SourceGenerator;

public static class GeneratorUtilities
{
    public static class Realtime
    {
        public static class Networks
        {
            public const string INetworkPayload =
                nameof(Realtime) + "." + nameof(Networks) + "." + nameof(INetworkPayload);
        }

        public static class Modules
        {
            public const string ModulesNamespace = nameof(Realtime) + "." + nameof(Modules);
            public const string RealtimeModule = nameof(RealtimeModule);
            public const string RealtimeModuleFullname = ModulesNamespace + "." + RealtimeModule;
        }
        public static class Objects
        {
            public const string ObjectsNamespace = nameof(Realtime) + "." + nameof(Objects);
            public const string NetworkObject = nameof(NetworkObject);
            public const string NetworkObjectFullname = ObjectsNamespace + "." + NetworkObject;
            public const string INetworkBehaviour = nameof(INetworkBehaviour);
            public const string INetworkBehaviourFullname = ObjectsNamespace + "." + nameof(INetworkBehaviour);

        }
        public static class Handlers
        {
            public const string HandlersNamespace = nameof(Realtime) + "." + nameof(Handlers);

            public static class Impl
            {
                public const string ImplNamespace = HandlersNamespace + "." + nameof(Impl);
                public const string MatchContext = nameof(MatchContext);
                public const string MatchContextFullname = ImplNamespace + "." + nameof(MatchContext);
            }
        }
        public static class Data
        {
            public const string DataNamespace = nameof(Realtime) + "." + nameof(Data);
            public const string PlayerData = DataNamespace + "." + nameof(PlayerData);
            public const string MatchData = DataNamespace + "." + nameof(MatchData);
            public const int MatchDataGenericCount = 3;
        }
    }
}