extends Node2D

@onready var popup_manager = $PopUpManager

# 如果需要在外部控制弹窗
func show_popup():
	if popup_manager:
		popup_manager.show_popup()

func hide_popup():
	if popup_manager:
		popup_manager.hide_popup()
