namespace ShrimplePawns.Example;

public sealed class Game : Component, Component.INetworkListener
{
	[Property]
	public PrefabFile ClientPrefab { get; set; }

	[Property]
	public PrefabFile SpectatePrefab { get; set; }

	protected override void OnStart()
	{
		if ( !Networking.IsActive )
			Networking.CreateLobby( new Sandbox.Network.LobbyConfig() );
	}

	// This gets called only on the host!
	public void OnActive( Connection channel )
	{
		var clientObj = SceneUtility.GetPrefabScene( ClientPrefab ).Clone();
		clientObj.NetworkSpawn( channel );

		var client = clientObj.Components.Get<Client>();
		client.AssignConnection( channel );

		// Option #1
		// The pawn component MUST have PawnAttribute.
		client.AssignPawn<SpectatePawn>();

		// Option #2
		// The pawn component does NOT need PawnAttribute in this case.
		// client.AssignPawn<SpectatePawn>(SpectatePrefab);
	}
}
