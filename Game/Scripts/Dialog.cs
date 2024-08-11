using Godot;
using System;

public partial class Dialog : Control {

	private RichTextLabel dialogLabel;
	private TextureRect dialogImage;
	private Timer timer;
	private String jsonName;
	private Godot.Collections.Array jsonArray;
	private AnimationPlayer dialogAnimationPlayer;

	private bool talking = false;
	private bool finish = false;
	private int arrayNum = 0;

	public override void _Ready() {
	
		SetProcessUnhandledKeyInput( false );

		dialogLabel = GetNode<RichTextLabel>( "MarginContainer/VBoxContainer/DialogTexture/MarginContainer/HBoxContainer/DialogLabel" );
		dialogImage = GetNode<TextureRect>( "MarginContainer/VBoxContainer/DialogTexture/MarginContainer/HBoxContainer/TextureRect" );
		timer = GetNode<Timer>( "MarginContainer/VBoxContainer/DialogTexture/MarginContainer/HBoxContainer/Timer" );
		dialogAnimationPlayer = GetNode<AnimationPlayer>( "DialogAnimations" );
	
	}

	public async override void _UnhandledKeyInput( InputEvent @event ) {

		if ( @event.IsActionPressed( "Confirm" ) ) {

			switch ( true ) {

				case true when finish:
				
					if ( ( ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] ).Keys.Contains( "save" ) ) ) {

						var save = ( bool ) ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "save" ];

						if ( save ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Data[ "dialogues" ] )[ jsonName ] = ( int ) arrayNum + 1;

						dialogAnimationPlayer.Play( "Close" );

						await ToSignal( dialogAnimationPlayer, "animation_finished" );

						GetTree().Paused = false;
						SetProcessUnhandledKeyInput( false );

						ProcessMode = Node.ProcessModeEnum.Disabled;
						
						arrayNum = 0;
						GD.Print( GetNode<Global>( "/root/Global" ).Data );

						break;

					}
				
					arrayNum += 1;
					dialogLabel.VisibleCharacters = 0;
					timer.WaitTime = ( float ) ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "speed" ];
					dialogLabel.Text = ( String ) ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "text" ];
					dialogImage.Texture = ( Texture2D ) ResourceLoader.Load( $"res://assets/DialogFace/{( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "emotion" ]}.png" );
					
					playDialog();
					
					break;

				case true when talking: talking = !talking; break;

			}

		}

		GC.Collect();

	}

	async public void open( Godot.Variant json, String name ) {

		ProcessMode = Node.ProcessModeEnum.Always;

		jsonArray = ( Godot.Collections.Array ) json;
		jsonName = name;

		if ( ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Data[ "dialogues" ] ).Keys.Contains( name ) ) arrayNum = ( int ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Data[ "dialogues" ] )[ name ];

		dialogLabel.VisibleCharacters = 0;
		timer.WaitTime = ( float ) ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "speed" ];
		dialogLabel.Text = ( String ) ( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "text" ];
		dialogImage.Texture = ( Texture2D ) ResourceLoader.Load( $"res://assets/DialogFace/{( ( Godot.Collections.Dictionary ) jsonArray[ arrayNum ] )[ "emotion" ]}.png" );

		GetTree().Paused = true;

		dialogAnimationPlayer.Play( "Open" );

		await ToSignal( dialogAnimationPlayer, "animation_finished" );

		SetProcessUnhandledKeyInput( true );

		playDialog();

	}

	async void playDialog() {
		
		finish = false;
		talking = true;

		timer.Start();
		await ToSignal( timer, "timeout" );

		for ( float i = 0; i < dialogLabel.Text.Length; i++ ) {

			if ( !talking ) {

				dialogLabel.VisibleCharacters = -1;
				break;

			}

			dialogLabel.VisibleCharacters += 1;

			timer.Start();
			await ToSignal( timer, "timeout" );

		}

		finish = true;
		talking = false;

	}

}
