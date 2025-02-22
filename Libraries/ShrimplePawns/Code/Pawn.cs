namespace ShrimplePawns;

/// <summary>
/// The base pawn that you should inherit off of.
/// </summary>
public abstract class Pawn : Component
{
	/// <summary>
	/// Called when the pawn has been assigned.
	/// </summary>
	public virtual void OnAssign( Client client )
	{

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
