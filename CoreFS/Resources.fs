namespace CoreFS.Resources

open Godot

[<Tool>]
type WeaponConfigFS() =
    inherit Resource()
    [<Export>]
    member val nameKey = "name" with get,set

    [<Export>]
    member val range = 100f with get,set
    
    [<Export>]
    member val rateOfFire = 0.5f with get,set

    [<Export>]
    member val damage = 1.f with get,set
    
    [<Export>]
    member val equipAnimation = "" with get,set

    [<Export>]
    member val unEquipAnimation = "" with get,set
    
    [<Export>]
    member val fireAnimation = "" with get,set

    [<Export>]
    member val reloadAnimation = "" with get,set
    static member Default() : WeaponConfigFS =
        new WeaponConfigFS()        



[<Tool>]
type PlayerConfigFS() =
    inherit Resource()
    [<Export>]
    member val gravityMultiplier = 3.0f with get, set

    [<Export>]
    member val speed = 10.0f with get, set

    [<Export>]
    member val acceleration = 8.0f with get, set

    [<Export>]
    member val deceleration = 10.0f with get, set
    
    [<Export(PropertyHint.Range, "0.01,100,0.01")>]
    member val airControl = 0.3f with get, set
    
    [<Export>]
    member val jumpHeight = 10.0f with get, set

    static member Default() : PlayerConfigFS =
        new PlayerConfigFS()        
        