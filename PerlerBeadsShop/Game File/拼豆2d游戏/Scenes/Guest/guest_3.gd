extends CharacterBody2D

signal guest_departed(table_index: int)
signal popup_requested(guest: Node)

# 客人属性
var target_position: Vector2 = Vector2.ZERO  # 初始化为 (0,0)
var table_index: int = -1
var is_sitting: bool = false
var is_leaving: bool = false
var is_active: bool = true
var has_triggered_popup: bool = false

# 移动参数
const WALK_SPEED = 250.0
const SPAWN_X = -100

# 节点引用
@onready var sprite = $Appearance if has_node("Appearance") else null
@onready var action_button = $"! Button" if has_node("! Button") else null

var timer: Timer

func _ready():
	print("Guest: _ready 被调用")
	print("Guest: target_position = ", target_position)
	
	# 创建 Timer
	timer = Timer.new()
	timer.wait_time = 0.1
	timer.one_shot = true
	timer.timeout.connect(_on_timer_timeout)
	add_child(timer)
	
	if not sprite:
		print("⚠️ 警告: 没有 Appearance 节点")
	else:
		if not sprite.texture:
			sprite.modulate = Color(1, 0.5, 0.5)
	
	if not action_button:
		print("⚠️ 警告: 没有 '! Button' 节点")
	else:
		if action_button is BaseButton:
			action_button.hide()
			action_button.pressed.connect(_on_button_pressed)
	
	# 从左侧出现（Y 使用 target_position 的 Y）
	if target_position != Vector2.ZERO:
		global_position = Vector2(SPAWN_X, target_position.y)
	else:
		global_position = Vector2(SPAWN_X, 0)
	print("Guest: 初始位置 = ", global_position)
	print("Guest: 目标位置 = ", target_position)

func randomize_appearance():
	if not sprite:
		return
	
	var colors = [
		Color(1, 0.4, 0.4),
		Color(0.4, 0.7, 1),
		Color(0.4, 1, 0.4),
		Color(1, 1, 0.4),
		Color(1, 0.4, 1),
		Color(1, 0.6, 0.2),
	]
	sprite.modulate = colors[randi() % colors.size()]
	
	var scale_val = randf_range(0.7, 1.3)
	sprite.scale = Vector2(scale_val, scale_val)
	
	if randi() % 2 == 0:
		sprite.flip_h = true

func _physics_process(_delta):
	if not is_active:
		return
	
	if is_leaving:
		velocity = Vector2(-WALK_SPEED, 0)
		move_and_slide()
		
		if global_position.x < -200:
			print("Guest: 离开屏幕")
			guest_departed.emit(table_index)
			queue_free()
		return
	
	if not is_sitting:
		# 确保目标位置有效
		if target_position == Vector2.ZERO:
			print("Guest: 警告 - target_position 是 (0,0)")
			return
		
		var direction = (target_position - global_position).normalized()
		velocity = Vector2(direction.x * WALK_SPEED, 0)
		move_and_slide()
		
		if abs(global_position.x - target_position.x) < 10:
			print("Guest: 到达桌子")
			arrive_at_table()

func arrive_at_table():
	print("Guest: 坐下")
	is_sitting = true
	velocity = Vector2.ZERO
	global_position = Vector2(target_position.x, global_position.y)
	
	if timer:
		var wait_time = randf_range(3.0, 7.0)
		print("Guest: 等待 ", wait_time, " 秒")
		timer.wait_time = wait_time
		timer.one_shot = true
		timer.start()

func _on_timer_timeout():
	print("Guest: 定时器触发")
	
	if is_leaving or not is_sitting:
		return
	
	if has_triggered_popup:
		print("Guest: 已触发过弹窗，离开")
		start_leaving()
		return
	
	print("Guest: 显示按钮")
	show_button()

func show_button():
	if is_leaving or not action_button:
		return
	
	action_button.show()
	action_button.global_position = global_position + Vector2(0, -140)

func _on_button_pressed():
	print("Guest: 按钮被点击")
	if is_leaving:
		return
	
	if action_button:
		action_button.hide()
	
	has_triggered_popup = true
	print("Guest: 请求弹窗")
	popup_requested.emit(self)

func on_popup_closed():
	print("Guest: 弹窗已关闭")
	if is_leaving or not is_sitting:
		return
	
	start_leaving()

func start_leaving():
	if is_leaving:
		return
	
	print("Guest: 开始离开")
	is_leaving = true
	is_sitting = false
	
	if action_button:
		action_button.hide()

func set_target(pos: Vector2, table_idx: int):
	print("Guest set_target 被调用")
	print("  传入位置: ", pos)
	print("  桌子索引: ", table_idx)
	target_position = pos
	table_index = table_idx
	print("  保存的目标位置: ", target_position)
	
	# 如果已经在场景中，更新初始位置
	if is_inside_tree():
		global_position = Vector2(SPAWN_X, target_position.y)
		print("  更新初始位置为: ", global_position)

func get_table_index() -> int:
	return table_index
