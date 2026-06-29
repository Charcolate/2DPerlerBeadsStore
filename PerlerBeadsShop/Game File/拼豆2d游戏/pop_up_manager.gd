extends Node

@onready var panel_base = $"../ColorRect/PanelBase"
@onready var popup_button = $"../ColorRect/PopUpButton"
@onready var popup_window = $"../ColorRect/PopUpWindow"

var is_animating: bool = false
var current_guest: Node = null

func _ready():
	if panel_base:
		panel_base.visible = false
		panel_base.modulate = Color(1, 1, 1, 0)
		panel_base.mouse_filter = Control.MOUSE_FILTER_STOP
		panel_base.gui_input.connect(_on_panel_base_clicked)
	
	if popup_window:
		popup_window.visible = false
		popup_window.mouse_filter = Control.MOUSE_FILTER_STOP
		popup_window.modulate = Color(1, 1, 1, 1)
	
	if popup_button:
		popup_button.pressed.connect(_on_popup_button_pressed)

func _on_popup_button_pressed():
	if not is_animating and panel_base and not panel_base.visible:
		show_popup(null)

func show_popup(guest: Node = null):
	if not panel_base or is_animating:
		return
	
	current_guest = guest
	is_animating = true
	panel_base.visible = true
	popup_window.visible = true
	popup_window.modulate = Color(1, 1, 1, 1)
	
	popup_window.position.y = -popup_window.size.y
	
	var tween = create_tween()
	tween.set_parallel(true)
	
	tween.tween_property(panel_base, "modulate:a", 1, 0.3)
	
	var target_y = (panel_base.size.y - popup_window.size.y) / 2
	tween.tween_property(popup_window, "position:y", target_y, 0.4).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	
	tween.finished.connect(func(): 
		is_animating = false
		popup_window.modulate = Color(1, 1, 1, 1)
	)

func hide_popup():
	if not panel_base or not panel_base.visible or is_animating:
		return
	
	is_animating = true
	
	var tween = create_tween()
	tween.set_parallel(true)
	
	tween.tween_property(panel_base, "modulate:a", 0, 0.2)
	tween.tween_property(popup_window, "position:y", -popup_window.size.y, 0.3).set_ease(Tween.EASE_IN)
	
	tween.finished.connect(func(): 
		panel_base.visible = false
		popup_window.visible = false
		popup_window.modulate = Color(1, 1, 1, 1)
		is_animating = false
		
		# 通知客人弹窗已关闭
		if current_guest and current_guest.has_method("on_popup_closed"):
			current_guest.on_popup_closed()
			current_guest = null
	)

func _on_panel_base_clicked(event):
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		var mouse_pos = get_viewport().get_mouse_position()
		var window_rect = Rect2(popup_window.global_position, popup_window.size)
		
		if not window_rect.has_point(mouse_pos):
			hide_popup()

func _input(event):
	if event is InputEventKey and event.pressed and event.keycode == KEY_ESCAPE:
		if panel_base and panel_base.visible:
			hide_popup()

func is_popup_visible() -> bool:
	return panel_base and panel_base.visible
