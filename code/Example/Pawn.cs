namespace ShrimplePawns.Example;

public abstract class Pawn : ShrimplePawns.Pawn
{
	[Sync( SyncFlags.FromHost )]
	public Client Owner { get; private set; }

	public override void OnAssign( ShrimplePawns.Client client )
	{
		Owner = client as Client;
	}
}
