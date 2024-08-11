extends Node2D

signal changeScene( sceneName )

func _ready():
	
	$MenuButtons/GridContainer/Start.grab_focus()
	
	$MenuButtons/GridContainer/Start.text = "New Game"
	$MenuButtons/GridContainer/Reset.visible = false
	
	get_viewport().connect( "gui_focus_changed", _on_focus_changed )

func _on_start_pressed(): emit_signal( "changeScene", "World_01" )

func _on_settings_pressed(): emit_signal( "changeScene", "Settings" )

func _on_reset_pressed(): pass

func _on_quit_pressed(): get_tree().quit()

func _on_focus_changed( control ):
	
	print( control )
