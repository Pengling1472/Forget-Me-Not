extends Node2D

@onready var customTheme = load( "res://Themes/CustomTheme.tres" )

var buttonPressed

signal changeScene( sceneName )

func _ready():
	
	for i in [ "Up", "Left", "Down", "Right", "Sprint", "Inventory", "Interact" ]:
		
		var node = Button.new()
		
		node.text = "%s: %s" % [ i, OS.get_keycode_string( InputMap.action_get_events( i )[ 0 ].get_physical_keycode() ) ]
		node.name = i
		node.theme = customTheme
		node.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		
		node.pressed.connect( _on_pressed.bind( node ) )
		
		$SettingsButtons/MarginContainer/VBoxContainer/GridContainer.add_child( node )
	
	set_process_unhandled_input( false )
	
	$SettingsButtons/MarginContainer/VBoxContainer/MusicContainer/MusicPercent.text = "%d%%" % Global.data.music
	$SettingsButtons/MarginContainer/VBoxContainer/MusicContainer/MusicSlider.value = Global.data.music
	
	$SettingsButtons/MarginContainer/VBoxContainer/SoundContainer/SoundPercent.text = "%d%%" % Global.data.sound
	$SettingsButtons/MarginContainer/VBoxContainer/SoundContainer/SoundSlider.value = Global.data.sound
	
	$SettingsButtons/MarginContainer/VBoxContainer/Fullscreen.text = "Fullscreen: ON" if Global.data.windowSettings.mode else "Fullscreen: OFF"

@warning_ignore( "unused_parameter" )
func _on_music_drag_ended( value_changed ): Global.data.music = $SettingsButtons/MarginContainer/VBoxContainer/MusicContainer/MusicSlider.value

func _on_music_value_changed( value ): $SettingsButtons/MarginContainer/VBoxContainer/MusicContainer/MusicPercent.text = "%d%%" % value

@warning_ignore( "unused_parameter" )
func _on_sound_drag_ended( value_changed ): Global.data.sound = $SettingsButtons/MarginContainer/VBoxContainer/SoundContainer/SoundSlider.value

func _on_sound_value_changed( value ): $SettingsButtons/MarginContainer/VBoxContainer/SoundContainer/SoundPercent.text = "%d%%" % value

func _on_fullscreen_pressed():
	
	DisplayServer.window_set_mode( DisplayServer.WINDOW_MODE_FULLSCREEN if !Global.data.windowSettings.mode else DisplayServer.WINDOW_MODE_WINDOWED )
	Global.data.windowSettings.mode = !Global.data.windowSettings.mode
	
	$SettingsButtons/MarginContainer/VBoxContainer/Fullscreen.text = "Fullscreen: ON" if Global.data.windowSettings.mode else "Fullscreen: OFF"

func _on_return_pressed(): emit_signal( "changeScene", "Main" )

func _on_pressed( button ): 
	
	set_process_unhandled_input( true )
	$SettingsButtons/Panel.visible = true
	buttonPressed = button

func _unhandled_input( event ):
	
	if event.is_action_type():
		
		InputMap.action_erase_events( buttonPressed.name )
		InputMap.action_add_event( buttonPressed.name, event )
		
		buttonPressed.text = "%s: %s" % [ buttonPressed.name, OS.get_keycode_string( event.keycode ) ]
		
		$SettingsButtons/Panel.visible = false
		set_process_unhandled_input( false )
