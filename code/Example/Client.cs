namespace ShrimplePawns.Example;

public class Client : ShrimplePawns.Client
{
	public static Client Local { get; private set; }

	protected override void OnStart()
	{
		if ( !IsProxy )
			Local = this;
	}
}
