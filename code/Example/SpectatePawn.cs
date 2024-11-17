namespace ShrimplePawns.Example;

[Pawn( "spectate.prefab" )]
public sealed class SpectatePawn : Pawn
{
	[RequireComponent]
	public CameraComponent CameraComponent { get; set; }

	private Angles EyeAngles { get; set; }

	protected override void OnStart()
	{
		CameraComponent.Enabled = !IsProxy;
	}

	protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( EyeAngles.pitch.Clamp( -90, 90 ) );
		WorldRotation = EyeAngles.ToRotation();
		WorldPosition += Input.AnalogMove * WorldRotation;
	}
}
