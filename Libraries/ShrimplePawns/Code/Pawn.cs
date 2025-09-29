namespace ShrimplePawns;

/// <summary>
/// The base pawn that you should inherit off of.
/// </summary>
public abstract class Pawn : Component
{
	private string _spawnedName;

	protected override void OnStart()
	{
		_spawnedName = GameObject.Name;
	}

	/// <summary>
	/// Called when the pawn has been assigned.
	/// </summary>
	public virtual void OnAssign( Client client )
	{
		GameObject.Name = $"{client.Connection.DisplayName} - {_spawnedName} PAWN";
	}

	/// <summary>
	/// Called when the pawn has been unassigned.
	/// </summary>
	public virtual void OnUnassign()
	{
		if ( GameObject.IsValid() )
			GameObject.Destroy();
	}
}
