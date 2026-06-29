extends Node2D

@onready var guest_manager = $GuestManager
@onready var popup_manager = $PopUpManager

func _ready():
	guest_manager.initialize(self)

func get_table_positions() -> Array:
	var positions = []
	for table in $Tables.get_children():
		positions.append(table.global_position)
	return positions

func get_guest_container() -> Node2D:
	return $GuestContainer
