Feature: control
  Player could use any control 

Scenario Outline: up
  Given a new game 
  When going up using <CONTROL>
  Then game go up

  Examples:
    | CONTROL  |
    | touch    |
    | keyboard |
    | gamepad  |



Feature: auto-start
  As a player, I want to directly start a game, So that I could play now without using the menu

Rules:
  Start last level

Scenario

