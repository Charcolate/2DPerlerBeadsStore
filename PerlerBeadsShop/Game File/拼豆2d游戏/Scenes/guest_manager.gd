extends Node

var main_scene: Node2D
var guests: Array = []
var table_occupied: Array = []
var table_positions: Array = []
var guest_scenes: Array = []
var spawn_timer: Timer
var is_active: bool = true

const SPAWN_INTERVAL = 3.0

func initialize(main: Node2D):
	print("=== GuestManager 初始化开始 ===")
	main_scene = main
	
	# 加载所有客人场景 - 确认路径正确！
	var guest_paths = [
		"res://Scenes/Guest/guest_1.tscn",
		"res://Scenes/Guest/guest_2.tscn",
		"res://Scenes/Guest/guest_3.tscn",
	]
	
	for path in guest_paths:
		var scene = load(path)
		if scene:
			guest_scenes.append(scene)
			print("✅ 加载成功: ", path)
		else:
			print("❌ 加载失败: ", path)
	
	if guest_scenes.size() == 0:
		print("❌ 错误: 没有加载到任何客人场景")
		return
	
	print("✅ 共加载 ", guest_scenes.size(), " 个客人外观")
	
	# 获取桌子位置
	var tables_node = main_scene.get_node("Tables")
	if not tables_node:
		print("❌ 错误: 找不到 Tables 节点")
		return
	
	table_positions = []
	for table in tables_node.get_children():
		var spot = table.get_node("CustomerSpot")
		if spot:
			table_positions.append(spot.global_position)
			print("✅ 找到位置: ", spot.global_position)
		else:
			table_positions.append(table.global_position)
			print("⚠️ 使用桌子位置: ", table.global_position)
	
	if table_positions.size() == 0:
		print("❌ 错误: 没有找到任何位置")
		return
	
	table_occupied.resize(table_positions.size())
	table_occupied.fill(false)
	
	spawn_timer = Timer.new()
	spawn_timer.wait_time = SPAWN_INTERVAL
	spawn_timer.timeout.connect(_on_spawn_timer_timeout)
	add_child(spawn_timer)
	spawn_timer.start()
	
	# 初始生成客人
	for i in range(randi_range(1, 2)):
		spawn_guest()
	
	print("=== GuestManager 初始化完成 ===")

func _on_spawn_timer_timeout():
	if is_active:
		check_and_spawn()

func check_and_spawn():
	var empty = find_empty_table()
	if empty != -1:
		spawn_guest()

func find_empty_table() -> int:
	for i in range(table_occupied.size()):
		if not table_occupied[i]:
			return i
	return -1

func spawn_guest():
	var empty_table = find_empty_table()
	if empty_table == -1:
		return
	
	var target_pos = table_positions[empty_table]
	
	# 随机选择客人外观
	var random_index = randi() % guest_scenes.size()
	var selected_guest = guest_scenes[random_index]
	print("生成客人外观 ", random_index + 1, " 在桌子 ", empty_table)
	
	var guest = selected_guest.instantiate()
	if not guest:
		print("❌ 无法实例化 Guest")
		return
	
	var guest_container = main_scene.get_node("GuestContainer")
	if not guest_container:
		print("❌ 找不到 GuestContainer")
		return
	
	guest_container.add_child(guest)
	guest.set_target(target_pos, empty_table)
	guest.global_position = Vector2(-100, target_pos.y)
	
	guest.guest_departed.connect(_on_guest_departed)
	guest.popup_requested.connect(_on_popup_requested)
	
	table_occupied[empty_table] = true
	guests.append(guest)
	print("✅ 新客人生成，当前客人: ", guests.size())

func _on_guest_departed(table_idx: int):
	if table_idx >= 0 and table_idx < table_occupied.size():
		table_occupied[table_idx] = false
	
	for i in range(guests.size() - 1, -1, -1):
		if guests[i].get_table_index() == table_idx:
			guests.remove_at(i)
			break

func _on_popup_requested(guest: Node):
	print("客人请求弹窗")
	var popup_manager = main_scene.get_node("PopUpManager")
	if popup_manager:
		popup_manager.show_popup(guest)
	else:
		print("❌ 找不到 PopUpManager")

func stop_spawning():
	is_active = false
	if spawn_timer:
		spawn_timer.stop()
