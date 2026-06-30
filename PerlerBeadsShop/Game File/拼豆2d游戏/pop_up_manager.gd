extends Node

@onready var panel_base = get_node("/root/Main/CanvasLayer/Control/PanelBase")
@onready var popup_button = get_node("/root/Main/CanvasLayer/Control/PopUpButton")
@onready var popup_window = get_node("/root/Main/CanvasLayer/Control/PopUpWindow")

var is_animating: bool = false
var current_guest: Node = null

func _ready():
	if panel_base:
		panel_base.visible = false
		panel_base.modulate = Color(0, 0, 0, 0)
		panel_base.mouse_filter = Control.MOUSE_FILTER_STOP
		panel_base.gui_input.connect(_on_panel_base_clicked)
	
	if popup_window:
		popup_window.visible = false
		popup_window.mouse_filter = Control.MOUSE_FILTER_STOP
		popup_window.modulate = Color(1, 1, 1, 1)
		
		# 隐藏所有子节点中的按钮（如果有）
		for child in popup_window.get_children():
			if child is Button or child is TextureButton:
				child.visible = false
				print("隐藏按钮: ", child.name)
	
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
	
	# 显示半透明背景
	panel_base.visible = true
	panel_base.modulate = Color(0, 0, 0, 0.6)
	
	# 显示弹窗
	popup_window.visible = true
	popup_window.modulate = Color(1, 1, 1, 1)
	
	# 居中弹窗
	var screen_size = get_viewport().get_visible_rect().size
	var window_size = popup_window.size
	popup_window.position = (screen_size - window_size) / 2
	
	# 从上方掉落
	popup_window.position.y = -window_size.y
	
	var tween = create_tween()
	var target_y = (screen_size.y - window_size.y) / 2
	tween.tween_property(popup_window, "position:y", target_y, 0.4).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	
	tween.finished.connect(func(): 
		is_animating = false
	)

func hide_popup():
	if not panel_base or not panel_base.visible or is_animating:
		return
	
	is_animating = true
	
	var tween = create_tween()
	tween.tween_property(popup_window, "position:y", -popup_window.size.y, 0.3).set_ease(Tween.EASE_IN)
	
	tween.finished.connect(func(): 
		panel_base.visible = false
		popup_window.visible = false
		panel_base.modulate = Color(0, 0, 0, 0)
		popup_window.modulate = Color(1, 1, 1, 1)
		is_animating = false
		
		if current_guest and current_guest.has_method("on_popup_closed"):
			current_guest.on_popup_closed()
			current_guest = null
	)

func _on_panel_base_clicked(event):
	# 点击 PanelBase（半透明背景）时关闭弹窗
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		hide_popup()

func _input(event):
	# ESC 键关闭
	if event is InputEventKey and event.pressed and event.keycode == KEY_ESCAPE:
		if panel_base and panel_base.visible:
			hide_popup()
	
	# 点击游戏场景中的任何地方关闭弹窗
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		if panel_base and panel_base.visible:
			# 检查是否点击在 PopUpWindow 内部
			if popup_window and popup_window.visible:
				var mouse_pos = get_viewport().get_mouse_position()
				var window_rect = Rect2(popup_window.global_position, popup_window.size)
				if window_rect.has_point(mouse_pos):
					return  # 点击在弹窗内部，不关闭
			# 点击在弹窗外部，关闭
			hide_popup()

func is_popup_visible() -> bool:
	return panel_base and panel_base.visible
