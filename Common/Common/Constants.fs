namespace Common.Constants

open Godot

module CommonColors =
    let WarningColor = Color(255f, 255f, 0f)
    let IdleColor = Color(140f, 140f, 140f)
    let AggroColor = Color(255f, 0f, 0f)

module InputActions =

    [<Literal>]
    let MoveBack = "move_back"

    [<Literal>]
    let MoveForward = "move_forward"

    [<Literal>]
    let MoveLeft = "move_left"

    [<Literal>]
    let MoveRight = "move_right"

    [<Literal>]
    let Left = "left"

    [<Literal>]
    let Right = "right"

    [<Literal>]
    let Up = "up"

    [<Literal>]
    let Down = "down"


    [<Literal>]
    let Jump = "jump"

    [<Literal>]
    let Sprint = "sprint"

    [<Literal>]
    let Interact = "interact"

    [<Literal>]
    let UICancel = "ui_cancel"

    [<Literal>]
    let ChangeMouseInput = "change_mouse_input"

    [<Literal>]
    let Pause = "pause"

    let AllInputs =
        [| MoveBack
           MoveForward
           MoveLeft
           MoveRight
           Jump
           Sprint
           Interact
           UICancel
           ChangeMouseInput |]

module SignalUtil =
    let CustomSignalArray = [| "dialogic_signal"; "hit" |]

    let MasterSignalArray =
        [| "script_changed"
           "on_request_permissions_result"
           "ready"
           "renamed"
           "tree_entered"
           "tree_exited"
           "tree_exiting"
           "animation_changed"
           "animation_finished"
           "animation_started"
           "caches_cleared"
           "finished"
           "draw"
           "hide"
           "item_rect_changed"
           "visibility_changed "
           "focus_entered"
           "focus_exited"
           "gui_input"
           "minimum_size_changed"
           "modal_closed"
           "mouse_entered"
           "mouse_exited"
           "resized"
           "size_flags_changed"
           "color_changed"
           "picker_created"
           "popup_closed"
           "about_to_show"
           "item_focused"
           "item_selected"
           "button_down"
           "button_up"
           "pressed"
           "toggled"
           "sort_children"
           "color_changed"
           "preset_added"
           "preset_removed"
           "close_request"
           "dragged"
           "offset_changed"
           "raise_request"
           "resize_request"
           "scroll_ended"
           "scroll_started"
           "dragged"
           "pre_popup_pressed"
           "tab_changed"
           "tab_selected"
           "_begin_node_move"
           "_end_node_move"
           "connection_from_empty"
           "connection_request"
           "connection_to_empty"
           "copy_nodes_request"
           "delete_nodes_request"
           "disconnection_request"
           "duplicate_nodes_request"
           "node_selected"
           "node_unselected"
           "paste_nodes_request"
           "popup_request"
           "scroll_offset_changed"
           "item_activated"
           "item_rmb_selected"
           "item_selected"
           "multi_selected"
           "nothing_selected"
           "rmb_clicked"
           "text_change_rejected"
           "text_changed"
           "text_entered"
           "texture_changed"
           "about_to_show"
           "popup_hide"
           "changed"
           "value_changed"
           "changed"
           "meta_clicked"
           "meta_hover_ended"
           "meta_hover_started"
           "reposition_active_tab_request"
           "right_button_pressed"
           "tab_changed"
           "tab_clicked"
           "tab_close"
           "tab_hover"
           "breakpoint_toggled"
           "cursor_changed"
           "info_clicked"
           "request_completion"
           "symbol_lookup"
           "text_changed"
           "button_pressed"
           "cell_selected"
           "column_title_pressed"
           "custom_popup_edited"
           "empty_rmb"
           "empty_tree_rmb_selected"
           "item_activated"
           "item_collapsed"
           "item_custom_button_pressed"
           "item_double_clicked"
           "item_edited"
           "item_rmb_edited"
           "item_rmb_selected"
           "item_selected"
           "multi_selected"
           "nothing_selected"
           "finished"
           "animation_finished"
           "frame_changed"
           "finished"
           "input_event"
           "mouse_entered"
           "mouse_exited"
           "area_entered"
           "area_exited"
           "area_shape_entered"
           "area_shape_exited"
           "body_entered"
           "body_exited"
           "body_shape_entered"
           "body_shape_exited"
           "body_entered"
           "body_exited"
           "body_shape_entered"
           "body_shape_exited"
           "sleeping_state_changed"
           "texture_changed"
           "texture_changed"
           "bone_setup_changed"
           "frame_changed"
           "texture_changed"
           "settings_changed"
           "pressed"
           "released"
           "screen_entered"
           "screen_exited"
           "viewport_entered"
           "viewport_exited"
           "request_completed"
           "visibility_changed"
           "mesh_updated"
           "button_pressed"
           "button_release"
           "mesh_updated"
           "finished"
           "input_event"
           "mouse_entered"
           "mouse_exited"
           "area_entered"
           "area_exited"
           "area_shape_entered"
           "area_shape_exited"
           "body_entered"
           "body_exited"
           "body_shape_entered"
           "body_shape_exited"
           "body_entered"
           "body_exited"
           "body_shape_entered"
           "body_shape_exited"
           "sleeping_state_changed"
           "cell_size_changed"
           "curve_changed"
           "broadcast"
           "camera_entered"
           "camera_exited"
           "screen_entered"
           "screen_exited"
           "frame_changed"
           "frame_changed"
           "timeout"
           "tween_all_completed"
           "tween_completed"
           "tween_started"
           "tween_step"
           "gui_focus_changed"
           "size_changed"
           "connected_to_server"
           "connection_failed"
           "files_dropped"
           "global_menu_action"
           "idle_frame"
           "network_peer_connected"
           "network_peer_disconnected"
           "node_added"
           "node_configuration_warning_changed"
           "node_removed"
           "node_renamed"
           "physics_frame"
           "screen_resized"
           "server_disconnected"
           "tree_changed" |]

    let AllSignals = Array.append CustomSignalArray MasterSignalArray
