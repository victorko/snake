module Snake.Core.Driver

open Snake.Core.Game

type Key = KeyUp | KeyDown | KeyLeft | KeyRight | KeySpace

type GameState =
    | GameStop
    | GameContinue of Board
    | GamePause of Board

type GameEvent =
    | KeyEvent of Key
    | TickEvent
