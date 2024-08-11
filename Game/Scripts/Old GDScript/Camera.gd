extends Camera2D

@warning_ignore("unused_parameter")
func _physics_process( delta ):
	
	global_position = get_node( "../MainCharacter" ).global_position
	force_update_scroll()
