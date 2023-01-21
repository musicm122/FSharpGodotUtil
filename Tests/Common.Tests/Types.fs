module Types

open Common.Types
open Moq
open Xunit
open FsUnit.Xunit
open Godot


module MoveDirection =
    module GetVelocityInMoveDirection =

        type TestDirectionData =
            { InputDirection: MoveDirection
              ExpectedVelocity: Vector2 }

        let sampleDirectionData =
            [ [| { TestDirectionData.InputDirection = MoveDirection.Up
                   TestDirectionData.ExpectedVelocity = Vector2(0f, -1f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.Down
                   TestDirectionData.ExpectedVelocity = Vector2(0f, 1f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.Left
                   TestDirectionData.ExpectedVelocity = Vector2(-1f, 0f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.Right
                   TestDirectionData.ExpectedVelocity = Vector2(1f, 0f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.UpLeft
                   TestDirectionData.ExpectedVelocity = Vector2(-0.5f, -0.5f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.UpRight
                   TestDirectionData.ExpectedVelocity = Vector2(0.5f, -0.5f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.DownLeft
                   TestDirectionData.ExpectedVelocity = Vector2(-0.5f, 0.5f) } |]
              [| { TestDirectionData.InputDirection = MoveDirection.DownRight
                   TestDirectionData.ExpectedVelocity = Vector2(0.5f, 0.5f) } |] ]

        [<Theory>]
        [<MemberData(nameof sampleDirectionData)>]
        let ``Should return correct velocity when associated direction is provided``
            (directionData: TestDirectionData)
            =
            let velocity = Vector2(0f, 0f)
            let speed = 1.0f

            directionData.InputDirection.GetVelocityInMoveDirection velocity speed
            |> should equal directionData.ExpectedVelocity

