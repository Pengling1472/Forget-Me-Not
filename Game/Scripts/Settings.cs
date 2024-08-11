using Godot;
using System;

public partial class Settings : Control {

	private Theme customTheme;
	private Button buttonNode;
	private InputEvent inputEvent;

	private Godot.Collections.Dictionary inputMaps = new Godot.Collections.Dictionary() {

		{ "Up", "ui_up" },
		{ "Left", "ui_left" },
		{ "Down", "ui_down" },
		{ "Right", "ui_right" },
		{ "Confirm", "ui_accept" },
		{ "Menu", "ui_cancel" }

	};
	private bool buttonPressed = false;
	private int windowScale = 1;

	public Button focusButton;
	public TextureRect focusCursor;

	private MarginContainer videoContainer;
	private MarginContainer audioContainer;
	private MarginContainer controlsContainer;

	public override void _Ready() {

		var data = GetNode<Global>( "/root/Global" ).Settings;
		customTheme = ResourceLoader.Load<Theme>( "res://Themes/Theme.tres" );

		foreach ( HBoxContainer i in GetNode<VBoxContainer>( "MarginContainer/TabContainer/ControlsContainer/VBoxContainer" ).GetChildren() ) {

			if ( i.GetChildren()[ 1 ] is not Button ) continue;

			var button = i.GetChildren()[ 1 ] as Button;

			button.Text = ( ( InputEventKey ) InputMap.ActionGetEvents( button.Name )[ 0 ] ).AsText();
			( ( RichTextLabel ) button.GetChildren()[ 0 ] ).Text = $"[wave amp=20.0 freq=3.0]{InputMap.ActionGetEvents( button.Name )[ 0 ].AsText()}[/wave]";

			button.Pressed += () => onPressed( button );
			button.FocusEntered += () => buttonFocus( true, button );
			button.FocusExited += () => buttonFocus( false, button );

		}

		foreach ( Button button in GetNode<HBoxContainer>( "MarginContainer/Tabs/HBoxContainer" ).GetChildren() ) {

			button.FocusEntered += () => buttonFocus( true, button );
			button.FocusEntered += () => changeTab( button );
			button.FocusExited += () => buttonFocus( false, button );

		}

		foreach ( HBoxContainer i in GetNode<VBoxContainer>( "MarginContainer/TabContainer/VideoContainer/VBoxContainer" ).GetChildren() ) {

			var button = i.GetChildren()[ 1 ] as Button;

			button.FocusEntered += () => buttonFocus( true, button );
			button.FocusExited += () => buttonFocus( false, button );

		}

		videoContainer = GetNode<MarginContainer>( "MarginContainer/TabContainer/VideoContainer" );
		audioContainer = GetNode<MarginContainer>( "MarginContainer/TabContainer/AudioContainer" );
		controlsContainer = GetNode<MarginContainer>( "MarginContainer/TabContainer/ControlsContainer" );

		audioContainer.GetNode<Label>( "VBoxContainer/MusicContainer/MusicPercent" ).Text = $"{ ( ( Godot.Collections.Dictionary ) data[ "audio" ] )[ "music" ] }%";
		audioContainer.GetNode<Slider>( "VBoxContainer/MusicContainer/MusicSlider" ).Value = ( ( int ) ( ( Godot.Collections.Dictionary ) data[ "audio" ] )[ "music" ] );

		audioContainer.GetNode<Label>( "VBoxContainer/SoundContainer/SoundPercent" ).Text = $"{ ( ( Godot.Collections.Dictionary ) data[ "audio" ] )[ "sound" ] }%";
		audioContainer.GetNode<Slider>( "VBoxContainer/SoundContainer/SoundSlider" ).Value = ( ( int ) ( ( Godot.Collections.Dictionary ) data[ "audio" ] )[ "sound" ] );

		GetNode<Button>( "MarginContainer/TabContainer/VideoContainer/VBoxContainer/FullscreenContainer/Fullscreen" ).Text = $"{ ( ( bool ) ( ( Godot.Collections.Dictionary ) data[ "display" ] )[ "fullscreen" ] ? "ON" : "OFF" ) }";

	}

	public override void _Input( InputEvent @event ) {

		if ( @event is InputEventKey eventKey ) {

			if ( eventKey.IsActionPressed( "Left" ) || eventKey.IsActionPressed( "Right" ) ) {

				var button = GetViewport().GuiGetFocusOwner();

				if ( button is Button && button.Name == "WindowScale" ) {

					windowScale = ( ( int ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "display" ] )[ "windowscale" ] );
					windowScale += ( int )( eventKey.GetActionStrength( "Right" ) - eventKey.GetActionStrength( "Left" ) );
					windowScale = windowScale == 0 ? 1 : windowScale == 11 ? 10 : windowScale;

					( customTheme.DefaultFont as SystemFont ).Oversampling = windowScale > 1 ? 5 : 0;

					( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "display" ] )[ "windowscale" ] = windowScale;

					var screenSize = DisplayServer.ScreenGetSize();

					DisplayServer.WindowSetPosition( new Vector2I( screenSize.X / 2 - ( 640 * windowScale ) / 2, screenSize.Y / 2 - ( 480 * windowScale ) / 2 ) );
					DisplayServer.WindowSetSize( new Vector2I( 640 , 480 ) * windowScale );

					GetTree().Root.ContentScaleSize = ( new Vector2I( 640, 480 ) * windowScale ); 
					GetTree().Root.GetNode<Control>( "CanvasLayer/Game" ).Scale = Vector2.One * windowScale;

					( ( Button ) button ).Text = $"< {windowScale}x >";
					( button.GetChildren()[ 0 ] as RichTextLabel ).Text = $"[wave amp=20.0 freq=3.0]< {windowScale}x >[/wave]";

				}

			}

		}

	}

	public override void _UnhandledKeyInput( InputEvent @event ) {
	
		if ( @event is InputEventKey eventKey ) {

			if ( eventKey.IsActionPressed( "Menu" ) && !buttonPressed && GetNode<AnimationPlayer>( "SettingAnimations" ).CurrentAnimation == "" ) { close(); return; }

			if ( eventKey.Pressed && buttonPressed ) {

				foreach ( HBoxContainer i in GetNode<VBoxContainer>( "MarginContainer/TabContainer/ControlsContainer/VBoxContainer" ).GetChildren() ) {

					if ( i.Name == "TabHBoxContainer" ) continue;

					var button = i.GetChildren()[ 1 ] as Button;

					if ( button.Text == eventKey.AsText() && button.Name != buttonNode.Name ) {

						button.Text = buttonNode.Text;
						( ( RichTextLabel ) button.GetChildren()[ 0 ] ).Text = $"[wave amp=20.0 freq=3.0]{buttonNode.Text}[/wave]";
						
						InputMap.ActionEraseEvent( button.Name, @event );
						InputMap.ActionAddEvent( button.Name, inputEvent );

						if ( button.Name != "Run" ) {
							
							InputMap.ActionEraseEvent( ( ( String ) inputMaps[ button.Name ] ), @event );
							InputMap.ActionAddEvent( ( ( String ) inputMaps[ button.Name ] ), inputEvent );

						}

						break;

					}

				}

				InputMap.ActionEraseEvent( buttonNode.Name, inputEvent );
				InputMap.ActionAddEvent( buttonNode.Name, @event );

				if ( buttonNode.Name != "Run" ) {
					
					InputMap.ActionEraseEvent( ( ( String ) inputMaps[ buttonNode.Name ] ), inputEvent );
					InputMap.ActionAddEvent( ( ( String ) inputMaps[ buttonNode.Name ] ), @event );

				}

				buttonNode.Text = eventKey.AsText();
				( ( RichTextLabel ) buttonNode.GetChildren()[ 0 ] ).Text = $"[wave amp=20.0 freq=3.0]{eventKey.AsText()}[/wave]";
				buttonNode.GrabFocus();

				GetNode<Panel>( "Panel" ).Visible = false;
				buttonPressed = false;

			}

		}

		GC.Collect();

	}

	private void changeTab( Button button ) {

		if ( GetNode<MarginContainer>( $"MarginContainer/TabContainer/{button.Name}Container" ).Visible ) return;

		foreach ( var childNode in GetNode<TextureRect>( "MarginContainer/TabContainer" ).GetChildren() ) if ( ( ( MarginContainer ) childNode ).Visible ) ( ( MarginContainer ) childNode ).Visible = false;

		GetNode<MarginContainer>( $"MarginContainer/TabContainer/{button.Name}Container" ).Visible = true;

	}

	private void OnSliderValueChanged( int value, String sliderName ) {
		
		GetNode<Label>( $"MarginContainer/TabContainer/AudioContainer/VBoxContainer/{sliderName}Container/{sliderName}Percent" ).Text = $"{value}%";
		
		( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "audio" ] )[ $"{sliderName.ToLower()}" ] = GetNode<Slider>( $"MarginContainer/TabContainer/AudioContainer/VBoxContainer/{sliderName}Container/{sliderName}Slider" ).Value;

		AudioServer.SetBusVolumeDb( AudioServer.GetBusIndex( sliderName ), ( 0.3f * value ) - 30 );

		if ( value == 0 ) {

			AudioServer.SetBusMute( AudioServer.GetBusIndex( sliderName ), true ); 

		} else { AudioServer.SetBusMute( AudioServer.GetBusIndex( sliderName ), false ); }
		
	}

	private void OnDisplayButtonPressed() {

		( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "display" ] )[ "fullscreen" ] = !( bool )( ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "display" ] )[ "fullscreen" ] );

		var screenSize = DisplayServer.ScreenGetSize();
		var displayData = ( bool ) ( ( Godot.Collections.Dictionary ) GetNode<Global>( "/root/Global" ).Settings[ "display" ] )[ "fullscreen" ];
		var windowedResolution = new Vector2I( 640, 480 ) * windowScale;
 
		if ( displayData ) DisplayServer.WindowSetPosition( new Vector2I( screenSize.X / 2 - ( 640 * windowScale ) / 2, screenSize.Y / 2 - ( 480 * windowScale ) / 2 ) );

		DisplayServer.WindowSetMode( ( bool ) displayData ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed );
		DisplayServer.WindowSetSize( ( bool ) displayData ? screenSize : windowedResolution );

		GetTree().Root.ContentScaleSize = ( bool ) displayData ? screenSize : windowedResolution; 
		GetTree().Root.GetNode<Control>( "CanvasLayer/Game" ).Scale = ( bool ) displayData ? ( screenSize.X > screenSize.Y ? Vector2.One * ( screenSize.X / 960f ) : Vector2.One * ( screenSize.Y / 540f ) ) : Vector2.One * windowScale;

		GetNode<HBoxContainer>( "MarginContainer/TabContainer/VideoContainer/VBoxContainer/WindowScaleContainer" ).Visible = !displayData;
		GetNode<Button>( "MarginContainer/TabContainer/VideoContainer/VBoxContainer/FullscreenContainer/Fullscreen" ).Text = $"{ ( ( bool ) displayData ? "ON" : "OFF" ) }";
		GetNode<RichTextLabel>( "MarginContainer/TabContainer/VideoContainer/VBoxContainer/FullscreenContainer/Fullscreen/RichTextLabel" ).Text = $"[wave amp=20.0 freq=3.0]{ ( ( bool ) displayData ? "ON" : "OFF" ) }[/wave]";

	}

	private void onPressed( Button button ) {

		buttonPressed = true;
		GetNode<Panel>( "Panel" ).Visible = true;

		button.ReleaseFocus();
		buttonNode = button;
		inputEvent = InputMap.ActionGetEvents( button.Name )[ 0 ];

	}

	private void buttonFocus( bool focus, Button button ) {

		button.GetNode<RichTextLabel>( "RichTextLabel" ).Modulate = Color.Color8( 255, 255, 255, ( byte )( focus ? 255 : 0 ) );
		button.SelfModulate = Color.Color8( 255, 255, 255, ( byte )( focus ? 0 : 255 ) );

	}

	public async void open( Button button ) {

		ProcessMode = Node.ProcessModeEnum.Always;

		GetTree().Paused = true;

		if ( button is Button ) button.ReleaseFocus();

		GetTree().Root.GetNode<AnimationPlayer>( "CanvasLayer/Game/Scenes/AnimationPlayer" ).Play( "toggle_blur" );
		GetNode<AnimationPlayer>( "SettingAnimations" ).Play( "Open" );

		await ToSignal( GetNode<AnimationPlayer>( "SettingAnimations" ), "animation_finished" );

		focusButton = button is Button ? button : null;

		GetNode<Button>( "MarginContainer/Tabs/HBoxContainer/Video" ).GrabFocus();

	}

	public async void close() {

		GetViewport().GuiReleaseFocus();

		GetTree().Root.GetNode<AnimationPlayer>( "CanvasLayer/Game/Scenes/AnimationPlayer" ).PlayBackwards( "toggle_blur" );
		GetNode<AnimationPlayer>( "SettingAnimations" ).PlayBackwards( "Open" );

		await ToSignal( GetNode<AnimationPlayer>( "SettingAnimations" ), "animation_finished" );

		if ( focusButton is Button ) focusButton.GrabFocus();

		GetTree().Paused = false;

		ProcessMode = Node.ProcessModeEnum.Disabled;

	}

}