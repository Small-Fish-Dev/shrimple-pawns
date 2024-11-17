namespace ShrimplePawns;

/// <summary>
/// The base pawn that you should inherit off of.
/// </summary>
public abstract class Pawn : Component
{
	[HostSync]
	protected Client Owner { get; set; }

	/// <summary>
	/// Get the owner only if it is valid.
	/// </summary>
	public bool TryGetOwner<T>( out T owner ) where T : Client
	{
		owner = GetOwner<T>();
		return owner.IsValid();
	}

	/// <summary>
	/// Get the owner.
	/// </summary>
	public T GetOwner<T>() where T : Client
	{
		return Owner as T;
	}

	/// <summary>
	/// Get the owner.
	/// </summary>
	public Client GetClient()
	{
		return Owner;
	}

	/// <summary>
	/// Called when the pawn has been assigned.
	/// </summary>
	public virtual void OnAssign( Client client )
	{
		Owner = client;
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
